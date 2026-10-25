using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Downloader;

public class DownloadPackage : IDisposable, IAsyncDisposable
{
	public bool IsSaving { get; set; }

	public bool IsSaveComplete { get; set; }

	public double SaveProgress { get; set; }

	public DownloadStatus Status { get; set; }

	public string[] Urls { get; set; }

	public long TotalFileSize { get; set; }

	public string FileName { get; set; }

	public Chunk[] Chunks { get; set; }

	public long ReceivedBytesSize => Chunks?.Sum((Chunk chunk) => chunk.Position) ?? 0;

	public bool IsSupportDownloadInRange { get; set; } = true;

	public bool InMemoryStream => string.IsNullOrWhiteSpace(FileName);

	public ConcurrentStream Storage { get; set; }

	public void Clear()
	{
		if (Chunks != null)
		{
			Chunk[] chunks = Chunks;
			for (int i = 0; i < chunks.Length; i++)
			{
				chunks[i].Clear();
			}
		}
		Chunks = null;
	}

	public async Task FlushAsync()
	{
		if (Storage?.CanWrite ?? false)
		{
			await Storage.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public void Validate()
	{
		Chunk[] chunks = Chunks;
		foreach (Chunk chunk in chunks)
		{
			if (!chunk.IsValidPosition() && (Storage?.Length ?? 0) <= chunk.Position)
			{
				chunk.Clear();
			}
			if (!IsSupportDownloadInRange)
			{
				chunk.Clear();
			}
		}
	}

	public void BuildStorage(bool reserveFileSize, long maxMemoryBufferBytes = 0L, ILogger logger = null)
	{
		Storage = (string.IsNullOrWhiteSpace(FileName) ? new ConcurrentStream(maxMemoryBufferBytes, logger) : new ConcurrentStream(FileName, reserveFileSize ? TotalFileSize : 0, maxMemoryBufferBytes, logger));
	}

	public void Dispose()
	{
		Clear();
		Storage?.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		Clear();
		if (Storage != null)
		{
			await Storage.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
