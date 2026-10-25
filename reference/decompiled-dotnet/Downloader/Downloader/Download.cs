using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader;

internal class Download : IDownload, IDisposable, IAsyncDisposable
{
	private readonly IDownloadService _downloadService;

	public string Url { get; }

	public string Folder { get; }

	public string Filename { get; }

	public long DownloadedFileSize => (_downloadService?.Package?.ReceivedBytesSize).GetValueOrDefault();

	public long TotalFileSize => _downloadService?.Package?.TotalFileSize ?? DownloadedFileSize;

	public DownloadPackage Package { get; private set; }

	public DownloadStatus Status => Package?.Status ?? DownloadStatus.None;

	public event EventHandler<DownloadProgressChangedEventArgs> ChunkDownloadProgressChanged
	{
		add
		{
			_downloadService.ChunkDownloadProgressChanged += value;
		}
		remove
		{
			_downloadService.ChunkDownloadProgressChanged -= value;
		}
	}

	public event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted
	{
		add
		{
			_downloadService.DownloadFileCompleted += value;
		}
		remove
		{
			_downloadService.DownloadFileCompleted -= value;
		}
	}

	public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
	{
		add
		{
			_downloadService.DownloadProgressChanged += value;
		}
		remove
		{
			_downloadService.DownloadProgressChanged -= value;
		}
	}

	public event EventHandler<DownloadStartedEventArgs> DownloadStarted
	{
		add
		{
			_downloadService.DownloadStarted += value;
		}
		remove
		{
			_downloadService.DownloadStarted -= value;
		}
	}

	public Download(string url, string path, string filename, DownloadConfiguration configuration)
	{
		_downloadService = new DownloadService(configuration);
		Url = url;
		Folder = path;
		Filename = filename;
		Package = _downloadService.Package;
	}

	public Download(DownloadPackage package, DownloadConfiguration configuration)
	{
		_downloadService = new DownloadService(configuration);
		Package = package;
	}

	public Download(DownloadPackage package, string address, DownloadConfiguration configuration)
	{
		_downloadService = new DownloadService(configuration);
		Package = package;
		Url = address;
	}

	public async Task<Stream> StartAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (string.IsNullOrWhiteSpace(Package?.Urls?.FirstOrDefault()))
		{
			if (!string.IsNullOrWhiteSpace(Filename))
			{
				await _downloadService.DownloadFileTaskAsync(Url, Path.Combine(Folder, Filename), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(Folder))
				{
					return await _downloadService.DownloadFileTaskAsync(Url, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				await _downloadService.DownloadFileTaskAsync(Url, new DirectoryInfo(Folder), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			return Stream.Null;
		}
		if (string.IsNullOrWhiteSpace(Url))
		{
			return await _downloadService.DownloadFileTaskAsync(Package, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		return await _downloadService.DownloadFileTaskAsync(Package, Url, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public void Stop()
	{
		_downloadService.CancelTaskAsync().Wait();
	}

	public void Pause()
	{
		_downloadService.Pause();
	}

	public void Resume()
	{
		_downloadService.Resume();
	}

	public override bool Equals(object obj)
	{
		if (obj is Download download)
		{
			return GetHashCode() == download.GetHashCode();
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (37 * 7 + Url.GetHashCode()) * 7 + DownloadedFileSize.GetHashCode();
	}

	public async ValueTask DisposeAsync()
	{
		await _downloadService.Clear().ConfigureAwait(continueOnCapturedContext: false);
		Package = null;
	}

	public void Dispose()
	{
		_downloadService.Clear().Wait();
		Package = null;
	}
}
