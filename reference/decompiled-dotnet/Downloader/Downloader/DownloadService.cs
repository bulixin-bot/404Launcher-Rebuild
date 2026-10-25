using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Downloader.Extensions.Helpers;
using Microsoft.Extensions.Logging;

namespace Downloader;

public class DownloadService : AbstractDownloadService
{
	public DownloadService(DownloadConfiguration options, ILoggerFactory loggerFactory = null)
		: base(options)
	{
		Logger = loggerFactory?.CreateLogger<DownloadService>();
	}

	public DownloadService(ILoggerFactory loggerFactory = null)
		: this(null, loggerFactory)
	{
	}

	protected override async Task<Stream> StartDownload(bool forceBuildStorage = true)
	{
		_ = 7;
		try
		{
			Logger?.LogInformation("Starting download process with forceBuildStorage={ForceBuildStorage}", forceBuildStorage);
			await SingleInstanceSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
			Request firstRequest = RequestInstances.First();
			Logger?.LogDebug("Getting file size from first request");
			DownloadPackage package = Package;
			package.TotalFileSize = await Client.GetFileSizeAsync(firstRequest).ConfigureAwait(continueOnCapturedContext: false);
			package = Package;
			package.IsSupportDownloadInRange = await Client.IsSupportDownloadInRange(firstRequest).ConfigureAwait(continueOnCapturedContext: false);
			Logger?.LogInformation("File size: {TotalFileSize}, Supports range download: {IsSupportDownloadInRange}", Package.TotalFileSize, Package.IsSupportDownloadInRange);
			if (forceBuildStorage || Package.Storage == null || Package.Storage.IsDisposed || (Package.Storage.Length == 0L && (Package.Chunks?.Any((Chunk c) => c.Position > 0) ?? false)))
			{
				Logger?.LogDebug("Building storage with ReserveStorageSpace={ReserveStorage}, MaxMemoryBuffer={MaxMemoryBuffer}", Options.ReserveStorageSpaceBeforeStartingDownload, Options.MaximumMemoryBufferBytes);
				Package.BuildStorage(Options.ReserveStorageSpaceBeforeStartingDownload, Options.MaximumMemoryBufferBytes);
			}
			ValidateBeforeChunking();
			ChunkHub.SetFileChunks(Package);
			Logger?.LogInformation("Starting download of {FileName} with size {TotalFileSize}", Package.FileName, Package.TotalFileSize);
			OnDownloadStarted(new DownloadStartedEventArgs(Package.FileName, Package.TotalFileSize));
			if (Options.ParallelDownload)
			{
				Logger?.LogDebug("Starting parallel download with {ChunkCount} chunks", Package.Chunks?.Length);
				await ParallelDownload(PauseTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			else
			{
				Logger?.LogDebug("Starting serial download with {ChunkCount} chunks", Package.Chunks?.Length);
				await SerialDownload(PauseTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
			}
			if (Status == DownloadStatus.Running)
			{
				Logger?.LogInformation("Download completed successfully");
				await SendDownloadCompletionSignal(DownloadStatus.Completed).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (OperationCanceledException ex)
		{
			Logger?.LogWarning(ex, "Download was cancelled");
			await SendDownloadCompletionSignal(DownloadStatus.Stopped, ex).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex2)
		{
			Logger?.LogError(ex2, "Download failed with error: {ErrorMessage}", ex2.Message);
			await SendDownloadCompletionSignal(DownloadStatus.Failed, ex2).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			SingleInstanceSemaphore.Release();
			Logger?.LogDebug("Download process completed, semaphore released");
		}
		return Package.Storage?.OpenRead();
	}

	private async Task SendDownloadCompletionSignal(DownloadStatus state, Exception error = null)
	{
		if (Status != DownloadStatus.Failed)
		{
			Status = state;
			bool isCancelled = Status == DownloadStatus.Stopped;
			Package.IsSaveComplete = Status == DownloadStatus.Completed && error == null;
			Package.IsSaving = false;
			await (Package?.Storage?.FlushAsync() ?? Task.FromResult(0)).ConfigureAwait(continueOnCapturedContext: false);
			if (Package?.Storage != null)
			{
				await Task.Delay(100);
			}
			OnDownloadFileCompleted(new AsyncCompletedEventArgs(error, isCancelled, Package));
		}
	}

	private void ValidateBeforeChunking()
	{
		CheckSingleChunkDownload();
		CheckSupportDownloadInRange();
		SetRangedSizes();
		CheckSizes();
	}

	private void SetRangedSizes()
	{
		if (Options.RangeDownload)
		{
			if (!Package.IsSupportDownloadInRange)
			{
				throw new NotSupportedException("The server of your desired address does not support download in a specific range");
			}
			if (Options.RangeHigh < Options.RangeLow)
			{
				Options.RangeLow = Options.RangeHigh - 1;
			}
			if (Options.RangeLow < 0)
			{
				Options.RangeLow = 0L;
			}
			if (Options.RangeHigh < 0)
			{
				Options.RangeHigh = Options.RangeLow;
			}
			if (Package.TotalFileSize > 0)
			{
				Options.RangeHigh = Math.Min(Package.TotalFileSize, Options.RangeHigh);
			}
			Package.TotalFileSize = Options.RangeHigh - Options.RangeLow + 1;
		}
		else
		{
			DownloadConfiguration options = Options;
			long rangeHigh = (Options.RangeLow = 0L);
			options.RangeHigh = rangeHigh;
		}
	}

	private void CheckSizes()
	{
		if (Options.CheckDiskSizeBeforeDownload && !Package.InMemoryStream)
		{
			FileHelper.ThrowIfNotEnoughSpace(Package.TotalFileSize, Package.FileName);
		}
	}

	private void CheckSingleChunkDownload()
	{
		if (Package.TotalFileSize <= 1)
		{
			Package.TotalFileSize = 0L;
		}
		if (Package.TotalFileSize <= Options.MinimumSizeOfChunking)
		{
			SetSingleChunkDownload();
		}
	}

	private void CheckSupportDownloadInRange()
	{
		if (!Package.IsSupportDownloadInRange)
		{
			SetSingleChunkDownload();
		}
	}

	private void SetSingleChunkDownload()
	{
		Options.ChunkCount = 1;
		Options.ParallelCount = 1;
		ParallelSemaphore = new SemaphoreSlim(1, 1);
	}

	private async Task ParallelDownload(PauseToken pauseToken)
	{
		try
		{
			List<Task> chunkTasks = GetChunksTasks(pauseToken).ToList();
			int maxConcurrentTasks = Math.Min(Options.ParallelCount, chunkTasks.Count);
			Logger?.LogDebug("Starting parallel download with {MaxConcurrentTasks} concurrent tasks", maxConcurrentTasks);
			for (int i = 0; i < chunkTasks.Count; i += maxConcurrentTasks)
			{
				await Task.WhenAll(chunkTasks.Skip(i).Take(maxConcurrentTasks)).ConfigureAwait(continueOnCapturedContext: false);
				if (GlobalCancellationTokenSource.Token.IsCancellationRequested)
				{
					Logger?.LogInformation("Download cancelled during batch processing");
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logger?.LogError(ex, "Error during parallel download: {ErrorMessage}", ex.Message);
			throw;
		}
	}

	private async Task SerialDownload(PauseToken pauseToken)
	{
		try
		{
			List<Task> list = GetChunksTasks(pauseToken).ToList();
			Logger?.LogDebug("Starting serial download with {ChunkCount} chunks", list.Count);
			foreach (Task item in list)
			{
				await item.ConfigureAwait(continueOnCapturedContext: false);
				if (GlobalCancellationTokenSource.Token.IsCancellationRequested)
				{
					Logger?.LogInformation("Download cancelled during serial processing");
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logger?.LogError(ex, "Error during serial download: {ErrorMessage}", ex.Message);
			throw;
		}
	}

	private IEnumerable<Task> GetChunksTasks(PauseToken pauseToken)
	{
		for (int i = 0; i < Package.Chunks.Length; i++)
		{
			Request request = RequestInstances[i % RequestInstances.Count];
			yield return DownloadChunk(Package.Chunks[i], request, pauseToken, GlobalCancellationTokenSource);
		}
	}

	private async Task<Chunk> DownloadChunk(Chunk chunk, Request request, PauseToken pause, CancellationTokenSource cancellationTokenSource)
	{
		ChunkDownloader chunkDownloader = new ChunkDownloader(chunk, Options, Package.Storage, Client, Logger);
		chunkDownloader.DownloadProgressChanged += OnChunkDownloadProgressChanged;
		await ParallelSemaphore.WaitAsync(cancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			cancellationTokenSource.Token.ThrowIfCancellationRequested();
			return await chunkDownloader.Download(request, pause, cancellationTokenSource.Token).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (OperationCanceledException)
		{
			await SendDownloadCompletionSignal(DownloadStatus.Stopped).ConfigureAwait(continueOnCapturedContext: false);
			throw;
		}
		catch (Exception error)
		{
			await SendDownloadCompletionSignal(DownloadStatus.Failed, error).ConfigureAwait(continueOnCapturedContext: false);
			cancellationTokenSource.Cancel(throwOnFirstException: false);
			throw;
		}
		finally
		{
			ParallelSemaphore.Release();
		}
		IL_030b:
		Chunk result;
		return result;
	}
}
