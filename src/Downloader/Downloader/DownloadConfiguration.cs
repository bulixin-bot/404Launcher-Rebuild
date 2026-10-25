using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Downloader;

public class DownloadConfiguration : ICloneable, INotifyPropertyChanged
{
	private int _activeChunks = 1;

	private int _bufferBlockSize = 1024;

	private int _chunkCount = 1;

	private long _maximumBytesPerSecond = long.MaxValue;

	private int _maximumTryAgainOnFailure = int.MaxValue;

	private long _maximumMemoryBufferBytes;

	private bool _checkDiskSizeBeforeDownload = true;

	private bool _parallelDownload;

	private int _parallelCount;

	private int _timeout = 1000;

	private bool _rangeDownload;

	private long _rangeLow;

	private long _rangeHigh;

	private bool _clearPackageOnCompletionWithFailure;

	private long _minimumSizeOfChunking = 512L;

	private bool _reserveStorageSpaceBeforeStartingDownload;

	private bool _enableLiveStreaming;

	public int ActiveChunks
	{
		get
		{
			return _activeChunks;
		}
		internal set
		{
			OnPropertyChanged(ref _activeChunks, value, "ActiveChunks");
		}
	}

	public int BufferBlockSize
	{
		get
		{
			return (int)Math.Min(MaximumSpeedPerChunk, _bufferBlockSize);
		}
		set
		{
			if ((value < 1 || value > 1048576) ? true : false)
			{
				throw new ArgumentOutOfRangeException("BufferBlockSize", "Buffer block size must be between 1 byte and 1024KB");
			}
			OnPropertyChanged(ref _bufferBlockSize, value, "BufferBlockSize");
		}
	}

	public bool CheckDiskSizeBeforeDownload
	{
		get
		{
			return _checkDiskSizeBeforeDownload;
		}
		set
		{
			OnPropertyChanged(ref _checkDiskSizeBeforeDownload, value, "CheckDiskSizeBeforeDownload");
		}
	}

	public int ChunkCount
	{
		get
		{
			return _chunkCount;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("ChunkCount", "Chunk count must be greater than 0");
			}
			OnPropertyChanged(ref _chunkCount, Math.Max(1, value), "ChunkCount");
		}
	}

	public long MaximumBytesPerSecond
	{
		get
		{
			return _maximumBytesPerSecond;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("MaximumBytesPerSecond", "Maximum bytes per second cannot be negative");
			}
			OnPropertyChanged(ref _maximumBytesPerSecond, (value <= 0) ? long.MaxValue : value, "MaximumBytesPerSecond");
		}
	}

	public long MaximumSpeedPerChunk => MaximumBytesPerSecond / Math.Max(Math.Min(Math.Min(ChunkCount, ParallelCount), ActiveChunks), 1);

	public int MaxTryAgainOnFailure
	{
		get
		{
			return _maximumTryAgainOnFailure;
		}
		set
		{
			OnPropertyChanged(ref _maximumTryAgainOnFailure, value, "MaxTryAgainOnFailure");
		}
	}

	public bool ParallelDownload
	{
		get
		{
			return _parallelDownload;
		}
		set
		{
			OnPropertyChanged(ref _parallelDownload, value, "ParallelDownload");
		}
	}

	public int ParallelCount
	{
		get
		{
			if (_parallelCount > 0)
			{
				return _parallelCount;
			}
			return ChunkCount;
		}
		set
		{
			OnPropertyChanged(ref _parallelCount, value, "ParallelCount");
		}
	}

	public bool RangeDownload
	{
		get
		{
			return _rangeDownload;
		}
		set
		{
			OnPropertyChanged(ref _rangeDownload, value, "RangeDownload");
		}
	}

	public long RangeLow
	{
		get
		{
			return _rangeLow;
		}
		set
		{
			OnPropertyChanged(ref _rangeLow, value, "RangeLow");
		}
	}

	public long RangeHigh
	{
		get
		{
			return _rangeHigh;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("RangeHigh", "Range high cannot be negative");
			}
			OnPropertyChanged(ref _rangeHigh, value, "RangeHigh");
		}
	}

	public RequestConfiguration RequestConfiguration { get; set; } = new RequestConfiguration();

	public int Timeout
	{
		get
		{
			return _timeout;
		}
		set
		{
			if (value < 100)
			{
				throw new ArgumentOutOfRangeException("Timeout", "Timeout must be at least 100 milliseconds");
			}
			OnPropertyChanged(ref _timeout, value, "Timeout");
		}
	}

	public bool ClearPackageOnCompletionWithFailure
	{
		get
		{
			return _clearPackageOnCompletionWithFailure;
		}
		set
		{
			OnPropertyChanged(ref _clearPackageOnCompletionWithFailure, value, "ClearPackageOnCompletionWithFailure");
		}
	}

	public long MinimumSizeOfChunking
	{
		get
		{
			return _minimumSizeOfChunking;
		}
		set
		{
			OnPropertyChanged(ref _minimumSizeOfChunking, value, "MinimumSizeOfChunking");
		}
	}

	public bool ReserveStorageSpaceBeforeStartingDownload
	{
		get
		{
			return _reserveStorageSpaceBeforeStartingDownload;
		}
		set
		{
			OnPropertyChanged(ref _reserveStorageSpaceBeforeStartingDownload, value, "ReserveStorageSpaceBeforeStartingDownload");
		}
	}

	public long MaximumMemoryBufferBytes
	{
		get
		{
			return _maximumMemoryBufferBytes;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("MaximumMemoryBufferBytes", "Maximum memory buffer bytes cannot be negative");
			}
			OnPropertyChanged(ref _maximumMemoryBufferBytes, value, "MaximumMemoryBufferBytes");
		}
	}

	public bool EnableLiveStreaming
	{
		get
		{
			return _enableLiveStreaming;
		}
		set
		{
			OnPropertyChanged(ref _enableLiveStreaming, value, "EnableLiveStreaming");
		}
	}

	public event PropertyChangedEventHandler PropertyChanged = delegate
	{
	};

	protected virtual void OnPropertyChanged<T>(ref T field, T newValue, [CallerMemberName] string name = null)
	{
		if (!field.Equals(newValue))
		{
			field = newValue;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
