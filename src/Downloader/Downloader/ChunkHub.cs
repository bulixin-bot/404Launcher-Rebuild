using System.Linq;

namespace Downloader;

public class ChunkHub
{
	private readonly DownloadConfiguration _config;

	private int _chunkCount;

	private long _chunkSize;

	private long _startOffset;

	public ChunkHub(DownloadConfiguration config)
	{
		_config = config;
	}

	public void SetFileChunks(DownloadPackage package)
	{
		Validate(package);
		if (package.Chunks == null)
		{
			package.Chunks = new Chunk[_chunkCount];
			for (int i = 0; i < _chunkCount; i++)
			{
				long num = _startOffset + i * _chunkSize;
				long end = num + _chunkSize - 1;
				package.Chunks[i] = GetChunk(i.ToString(), num, end);
			}
			package.Chunks.Last().End += package.TotalFileSize % _chunkCount;
		}
		else
		{
			package.Validate();
		}
	}

	private void Validate(DownloadPackage package)
	{
		_chunkCount = _config.ChunkCount;
		_startOffset = _config.RangeLow;
		if (_startOffset < 0)
		{
			_startOffset = 0L;
		}
		if (package.TotalFileSize < _chunkCount)
		{
			_chunkCount = (int)package.TotalFileSize;
		}
		if (_chunkCount < 1)
		{
			_chunkCount = 1;
		}
		_chunkSize = package.TotalFileSize / _chunkCount;
	}

	private Chunk GetChunk(string id, long start, long end)
	{
		return new Chunk(start, end)
		{
			Id = id,
			MaxTryAgainOnFailure = _config.MaxTryAgainOnFailure,
			Timeout = _config.Timeout
		};
	}
}
