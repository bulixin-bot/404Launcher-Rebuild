using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Downloader.Extensions.Helpers;
using Microsoft.Extensions.Logging;

namespace Downloader;

internal class ChunkDownloader
{
	private readonly ILogger _logger;

	private readonly DownloadConfiguration _configuration;

	private readonly int _timeoutIncrement = 10;

	private ThrottledStream _sourceStream;

	private readonly ConcurrentStream _storage;

	private readonly SocketClient _client;

	internal Chunk Chunk { get; set; }

	public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

	public ChunkDownloader(Chunk chunk, DownloadConfiguration config, ConcurrentStream storage, SocketClient client, ILogger logger = null)
	{
		Chunk = chunk;
		_configuration = config;
		_storage = storage;
		_client = client;
		_logger = logger;
		_configuration.PropertyChanged += ConfigurationPropertyChanged;
	}

	private void ConfigurationPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		_logger?.LogDebug("Changed configuration " + e.PropertyName + " property");
		string propertyName = e.PropertyName;
		if ((propertyName == "MaximumBytesPerSecond" || propertyName == "ActiveChunks") ? true : false)
		{
			ThrottledStream sourceStream = _sourceStream;
			if (sourceStream != null && sourceStream.CanRead)
			{
				_sourceStream.BandwidthLimit = _configuration.MaximumSpeedPerChunk;
			}
		}
	}

	public async ValueTask<Chunk> Download(Request downloadRequest, PauseToken pause, CancellationToken cancelToken)
	{
		try
		{
			_logger?.LogDebug("Starting download the chunk " + Chunk.Id + ".");
			await DownloadChunk(downloadRequest, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			return Chunk;
		}
		catch (TaskCanceledException exception) when (!cancelToken.IsCancellationRequested)
		{
			_logger?.LogError(exception, "Task time-outed on download chunk " + Chunk.Id + ". Retry ...");
			return await ContinueWithDelay(downloadRequest, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (ObjectDisposedException exception2) when (!cancelToken.IsCancellationRequested)
		{
			_logger?.LogError(exception2, "Disposed object error on download chunk " + Chunk.Id + ". Retry ...");
			return await ContinueWithDelay(downloadRequest, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (HttpRequestException exception3) when (!cancelToken.IsCancellationRequested && Chunk.CanTryAgainOnFailure())
		{
			_logger?.LogError(exception3, "HTTP request error on download chunk " + Chunk.Id + ". Retry ...");
			return await ContinueWithDelay(downloadRequest, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception ex) when (!cancelToken.IsCancellationRequested && ex.IsMomentumError() && Chunk.CanTryAgainOnFailure())
		{
			_logger?.LogError(ex, "Error on download chunk " + Chunk.Id + ". Retry ...");
			return await ContinueWithDelay(downloadRequest, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception exception4)
		{
			cancelToken.ThrowIfCancellationRequested();
			_logger?.LogCritical(exception4, "Fatal error on download chunk " + Chunk.Id + ".");
			throw;
		}
	}

	private async ValueTask<Chunk> ContinueWithDelay(Request request, PauseToken pause, CancellationToken cancelToken)
	{
		if (cancelToken.IsCancellationRequested)
		{
			return Chunk;
		}
		_logger?.LogDebug("ContinueWithDelay of the chunk " + Chunk.Id);
		await _client.ThrowIfIsNotSupportDownloadInRange(request).ConfigureAwait(continueOnCapturedContext: false);
		await Task.Delay(Chunk.Timeout, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		Chunk.Timeout += _timeoutIncrement;
		return await Download(request, pause, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	private async ValueTask DownloadChunk(Request request, PauseToken pauseToken, CancellationToken cancelToken)
	{
		if (cancelToken.IsCancellationRequested || Chunk.IsDownloadCompleted())
		{
			return;
		}
		_logger?.LogDebug("DownloadChunk of the chunk " + Chunk.Id);
		HttpRequestMessage request2 = request.GetRequest();
		SetRequestRange(request2);
		using HttpResponseMessage responseMsg = await _client.SendRequestAsync(request2, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		_logger?.LogDebug($"Downloading the chunk {Chunk.Id} with response status code: {responseMsg.StatusCode}");
		await using Stream responseStream = await responseMsg.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		_sourceStream = new ThrottledStream(responseStream, _configuration.MaximumSpeedPerChunk);
		await ReadStream(_sourceStream, pauseToken, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		await _sourceStream.DisposeAsync();
	}

	private void SetRequestRange(HttpRequestMessage request)
	{
		long num = Chunk.Start + Chunk.Position;
		if (Chunk.End > 0 && num < Chunk.End && (_configuration.ChunkCount > 1 || Chunk.Position > 0 || _configuration.RangeDownload))
		{
			request.Headers.Range = new RangeHeaderValue(num, Chunk.End);
		}
	}

	internal async Task ReadStream(Stream stream, PauseToken pauseToken, CancellationToken cancelToken)
	{
		int readSize = 1;
		CancellationToken? innerToken = null;
		try
		{
			await using (cancelToken.Register(stream.Close))
			{
				while (readSize > 0 && Chunk.CanWrite)
				{
					cancelToken.ThrowIfCancellationRequested();
					await pauseToken.WaitWhilePausedAsync().ConfigureAwait(continueOnCapturedContext: false);
					byte[] buffer = new byte[_configuration.BufferBlockSize];
					using CancellationTokenSource innerCts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
					innerToken = innerCts.Token;
					innerCts.CancelAfter(Chunk.Timeout);
					await using (innerToken.Value.Register(stream.Close))
					{
						readSize = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), innerToken.Value).ConfigureAwait(continueOnCapturedContext: false);
						_logger?.LogDebug($"Read {readSize}bytes of the chunk {Chunk.Id} stream");
					}
					readSize = (int)Math.Min(Chunk.EmptyLength, readSize);
					if (readSize > 0)
					{
						await _storage.WriteAsync(Chunk.Start + Chunk.Position - _configuration.RangeLow, buffer, readSize).ConfigureAwait(continueOnCapturedContext: false);
						_logger?.LogDebug($"Write {readSize}bytes in the chunk {Chunk.Id}");
						Chunk.Position += readSize;
						_logger?.LogDebug($"The chunk {Chunk.Id} current position is: {Chunk.Position} of {Chunk.Length}");
						OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(Chunk.Id)
						{
							TotalBytesToReceive = Chunk.Length,
							ReceivedBytesSize = Chunk.Position,
							ProgressedByteSize = readSize,
							ReceivedBytes = (_configuration.EnableLiveStreaming ? buffer.Take(readSize).ToArray() : Array.Empty<byte>())
						});
					}
				}
			}
		}
		catch (ObjectDisposedException ex)
		{
			_logger?.LogError(ex, "ReadAsync of the chunk " + Chunk.Id + " stream was canceled or closed forcibly from server");
			cancelToken.ThrowIfCancellationRequested();
			if (innerToken?.IsCancellationRequested ?? false)
			{
				_logger?.LogError(ex, "ReadAsync of the chunk " + Chunk.Id + " stream has been timed out");
				throw new TaskCanceledException("ReadAsync of the chunk " + Chunk.Id + " stream has been timed out", ex);
			}
			throw;
		}
		_logger?.LogDebug("ReadStream of the chunk " + Chunk.Id + " completed successfully");
	}

	private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
	{
		DownloadProgressChanged?.Invoke(this, e);
	}
}
