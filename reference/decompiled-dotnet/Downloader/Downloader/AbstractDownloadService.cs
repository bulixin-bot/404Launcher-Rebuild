using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Downloader;

public abstract class AbstractDownloadService : IDownloadService, IDisposable, IAsyncDisposable
{
	protected ILogger Logger;

	protected SemaphoreSlim ParallelSemaphore;

	protected readonly SemaphoreSlim SingleInstanceSemaphore = new SemaphoreSlim(1, 1);

	protected CancellationTokenSource GlobalCancellationTokenSource;

	protected TaskCompletionSource<AsyncCompletedEventArgs> TaskCompletion;

	protected readonly PauseTokenSource PauseTokenSource;

	protected ChunkHub ChunkHub;

	protected List<Request> RequestInstances;

	protected readonly Bandwidth Bandwidth;

	protected DownloadConfiguration Options { get; set; }

	public bool IsBusy => Status == DownloadStatus.Running;

	public bool IsCancelled => GlobalCancellationTokenSource?.IsCancellationRequested ?? false;

	public bool IsPaused => PauseTokenSource.IsPaused;

	public DownloadPackage Package { get; set; }

	public DownloadStatus Status
	{
		get
		{
			return Package?.Status ?? DownloadStatus.None;
		}
		set
		{
			Package.Status = value;
		}
	}

	protected SocketClient Client { get; private set; }

	public event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;

	public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

	public event EventHandler<DownloadProgressChangedEventArgs> ChunkDownloadProgressChanged;

	public event EventHandler<DownloadStartedEventArgs> DownloadStarted;

	protected AbstractDownloadService(DownloadConfiguration options)
	{
		PauseTokenSource = new PauseTokenSource();
		Bandwidth = new Bandwidth();
		Options = options ?? new DownloadConfiguration();
		Package = new DownloadPackage();
	}

	public Task<Stream> DownloadFileTaskAsync(DownloadPackage package, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DownloadFileTaskAsync(package, package.Urls, cancellationToken);
	}

	public Task<Stream> DownloadFileTaskAsync(DownloadPackage package, string address, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DownloadFileTaskAsync(package, new string[1] { address }, cancellationToken);
	}

	public virtual async Task<Stream> DownloadFileTaskAsync(DownloadPackage package, string[] urls, CancellationToken cancellationToken = default(CancellationToken))
	{
		Package = package;
		await InitialDownloader(cancellationToken, urls).ConfigureAwait(continueOnCapturedContext: false);
		return await StartDownload(forceBuildStorage: false).ConfigureAwait(continueOnCapturedContext: false);
	}

	public Task<Stream> DownloadFileTaskAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DownloadFileTaskAsync(new string[1] { address }, cancellationToken);
	}

	public virtual async Task<Stream> DownloadFileTaskAsync(string[] urls, CancellationToken cancellationToken = default(CancellationToken))
	{
		await InitialDownloader(cancellationToken, urls).ConfigureAwait(continueOnCapturedContext: false);
		return await StartDownload().ConfigureAwait(continueOnCapturedContext: false);
	}

	public Task DownloadFileTaskAsync(string address, string fileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DownloadFileTaskAsync(new string[1] { address }, fileName, cancellationToken);
	}

	public virtual async Task DownloadFileTaskAsync(string[] urls, string fileName, CancellationToken cancellationToken = default(CancellationToken))
	{
		await InitialDownloader(cancellationToken, urls).ConfigureAwait(continueOnCapturedContext: false);
		await StartDownload(fileName).ConfigureAwait(continueOnCapturedContext: false);
	}

	public Task DownloadFileTaskAsync(string address, DirectoryInfo folder, CancellationToken cancellationToken = default(CancellationToken))
	{
		return DownloadFileTaskAsync(new string[1] { address }, folder, cancellationToken);
	}

	public virtual async Task DownloadFileTaskAsync(string[] urls, DirectoryInfo folder, CancellationToken cancellationToken = default(CancellationToken))
	{
		await InitialDownloader(cancellationToken, urls).ConfigureAwait(continueOnCapturedContext: false);
		string path = await Client.SetRequestFileNameAsync(RequestInstances.First()).ConfigureAwait(continueOnCapturedContext: false);
		string fileName = Path.Combine(folder.FullName, path);
		await StartDownload(fileName).ConfigureAwait(continueOnCapturedContext: false);
	}

	public virtual void CancelAsync()
	{
		GlobalCancellationTokenSource?.Cancel(throwOnFirstException: true);
		Status = DownloadStatus.Stopped;
		Resume();
	}

	public virtual async Task CancelTaskAsync()
	{
		CancelAsync();
		if (TaskCompletion != null)
		{
			await TaskCompletion.Task.ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public virtual void Resume()
	{
		Status = DownloadStatus.Running;
		PauseTokenSource.Resume();
	}

	public virtual void Pause()
	{
		PauseTokenSource.Pause();
		Status = DownloadStatus.Paused;
	}

	public virtual async Task Clear()
	{
		_ = 1;
		try
		{
			if (IsBusy || IsPaused)
			{
				await CancelTaskAsync().ConfigureAwait(continueOnCapturedContext: false);
			}
			await SingleInstanceSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
			ParallelSemaphore?.Dispose();
			GlobalCancellationTokenSource?.Dispose();
			Bandwidth.Reset();
			RequestInstances = null;
			if (TaskCompletion != null)
			{
				if (!TaskCompletion.Task.IsCompleted)
				{
					TaskCompletion.TrySetCanceled();
				}
				TaskCompletion = null;
			}
		}
		finally
		{
			SingleInstanceSemaphore?.Release();
		}
	}

	protected async Task InitialDownloader(CancellationToken cancellationToken, params string[] addresses)
	{
		await Clear().ConfigureAwait(continueOnCapturedContext: false);
		Status = DownloadStatus.Created;
		GlobalCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		TaskCompletion = new TaskCompletionSource<AsyncCompletedEventArgs>();
		Client = new SocketClient(Options.RequestConfiguration);
		RequestInstances = addresses.Select((string url) => new Request(url, Options.RequestConfiguration)).ToList();
		Package.Urls = RequestInstances.Select((Request req) => req.Address.OriginalString).ToArray();
		ChunkHub = new ChunkHub(Options);
		ParallelSemaphore = new SemaphoreSlim(Options.ParallelCount, Options.ParallelCount);
	}

	protected async Task StartDownload(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			Package.FileName = fileName;
			string directoryName = Path.GetDirectoryName(fileName);
			if (!string.IsNullOrWhiteSpace(directoryName))
			{
				Directory.CreateDirectory(directoryName);
				await Task.Delay(100);
			}
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
		}
		await StartDownload().ConfigureAwait(continueOnCapturedContext: false);
	}

	protected abstract Task<Stream> StartDownload(bool forceBuildStorage = true);

	protected void OnDownloadStarted(DownloadStartedEventArgs e)
	{
		Status = DownloadStatus.Running;
		Package.IsSaving = true;
		DownloadStarted?.Invoke(this, e);
	}

	protected void OnDownloadFileCompleted(AsyncCompletedEventArgs e)
	{
		Package.IsSaving = false;
		if (e.Cancelled)
		{
			Status = DownloadStatus.Stopped;
		}
		else if (e.Error != null)
		{
			if (Options.ClearPackageOnCompletionWithFailure)
			{
				Package.Storage?.Dispose();
				Package.Storage = null;
				Package.Clear();
				if (!Package.InMemoryStream)
				{
					File.Delete(Package.FileName);
				}
			}
		}
		else
		{
			Package.Clear();
		}
		if (!Package.InMemoryStream)
		{
			Package.Storage?.Dispose();
			Package.Storage = null;
		}
		TaskCompletion.TrySetResult(e);
		DownloadFileCompleted?.Invoke(this, e);
	}

	private void RaiseProgressChangedEvents(object sender, DownloadProgressChangedEventArgs e)
	{
		if (e.ReceivedBytesSize > Package.TotalFileSize)
		{
			Package.TotalFileSize = e.ReceivedBytesSize;
		}
		Bandwidth.CalculateSpeed(e.ProgressedByteSize);
		Options.ActiveChunks = Options.ParallelCount - ParallelSemaphore.CurrentCount;
		DownloadProgressChangedEventArgs e2 = new DownloadProgressChangedEventArgs("DownloadService")
		{
			TotalBytesToReceive = Package.TotalFileSize,
			ReceivedBytesSize = Package.ReceivedBytesSize,
			BytesPerSecondSpeed = Bandwidth.Speed,
			AverageBytesPerSecondSpeed = Bandwidth.AverageSpeed,
			ProgressedByteSize = e.ProgressedByteSize,
			ReceivedBytes = e.ReceivedBytes,
			ActiveChunks = Options.ActiveChunks
		};
		Package.SaveProgress = e2.ProgressPercentage;
		e.ActiveChunks = e2.ActiveChunks;
		ChunkDownloadProgressChanged?.Invoke(this, e);
		DownloadProgressChanged?.Invoke(this, e2);
	}

	protected void OnChunkDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		RaiseProgressChangedEvents(sender, e);
	}

	public void AddLogger(ILogger logger)
	{
		Logger = logger;
	}

	public void Dispose()
	{
		Clear().Wait();
	}

	public virtual async ValueTask DisposeAsync()
	{
		await Clear().ConfigureAwait(continueOnCapturedContext: false);
	}
}
