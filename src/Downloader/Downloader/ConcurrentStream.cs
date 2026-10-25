using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Downloader;

public class ConcurrentStream : TaskStateManagement, IDisposable, IAsyncDisposable
{
	private ConcurrentPacketBuffer<Packet> _inputBuffer;

	private volatile bool _disposed;

	private Stream _stream;

	private long _position;

	private CancellationTokenSource _watcherCancelSource;

	protected Stream Stream => _stream ?? GetStream();

	public bool IsDisposed => _disposed;

	public string Path { get; set; }

	public byte[] Data
	{
		get
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("Stream");
			}
			if (Stream is MemoryStream memoryStream)
			{
				return memoryStream.ToArray();
			}
			return null;
		}
		set
		{
			if (value != null)
			{
				_stream = new MemoryStream();
				_stream.Write(value, 0, value.Length);
			}
		}
	}

	public bool IsMemoryStream => string.IsNullOrWhiteSpace(Path);

	public bool CanRead => Stream?.CanRead ?? false;

	public bool CanSeek => Stream?.CanSeek ?? false;

	public bool CanWrite => Stream?.CanWrite ?? false;

	public long Length => _stream?.Length ?? 0;

	public long Position
	{
		get
		{
			return _stream?.Position ?? _position;
		}
		set
		{
			if (_stream == null)
			{
				_position = value;
			}
			else
			{
				_stream.Position = value;
			}
		}
	}

	public long MaxMemoryBufferBytes
	{
		get
		{
			return _inputBuffer.BufferSize;
		}
		set
		{
			_inputBuffer.BufferSize = value;
		}
	}

	public ConcurrentStream()
		: this(0L)
	{
	}

	public ConcurrentStream(ILogger logger)
		: this(0L, logger)
	{
	}

	public ConcurrentStream(long maxMemoryBufferBytes = 0L, ILogger logger = null)
		: base(logger)
	{
		Initial(maxMemoryBufferBytes);
	}

	public ConcurrentStream(Stream stream, long maxMemoryBufferBytes = 0L, ILogger logger = null)
		: base(logger)
	{
		_stream = stream;
		Initial(maxMemoryBufferBytes);
	}

	public ConcurrentStream(string filename, long initSize, long maxMemoryBufferBytes = 0L, ILogger logger = null)
		: base(logger)
	{
		Path = filename;
		if (initSize >= 0 && Stream.Length == 0L)
		{
			SetLength(initSize);
		}
		Initial(maxMemoryBufferBytes);
	}

	private void Initial(long maxMemoryBufferBytes, ILogger logger = null)
	{
		_inputBuffer = new ConcurrentPacketBuffer<Packet>(maxMemoryBufferBytes, logger);
		_watcherCancelSource = new CancellationTokenSource();
		Task.Factory.StartNew((Func<Task>)Watcher, _watcherCancelSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
	}

	protected Stream GetStream()
	{
		if (_disposed || _stream != null)
		{
			return _stream;
		}
		lock (this)
		{
			if (_stream != null)
			{
				return _stream;
			}
			_stream = (IsMemoryStream ? ((Stream)new MemoryStream()) : ((Stream)new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)));
			if (_position > 0)
			{
				Seek(_position, SeekOrigin.Begin);
			}
		}
		return _stream;
	}

	public Stream OpenRead()
	{
		Seek(0L, SeekOrigin.Begin);
		return Stream;
	}

	public int Read(byte[] buffer, int offset, int count)
	{
		return OpenRead().Read(buffer, offset, count);
	}

	public async Task WriteAsync(long position, byte[] bytes, int length, bool fireAndForget = true)
	{
		if (bytes.Length < length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		if (IsFaulted && Exception != null)
		{
			throw Exception;
		}
		await _inputBuffer.TryAdd(new Packet(position, bytes, length)).ConfigureAwait(continueOnCapturedContext: false);
		if (!fireAndForget)
		{
			await FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private async Task Watcher()
	{
		try
		{
			StartState();
			while (!_watcherCancelSource.IsCancellationRequested)
			{
				await _inputBuffer.WaitTryTakeAsync(_watcherCancelSource.Token, WritePacketOnFile).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
		{
			Logger?.LogError(ex, "ConcurrentStream: Call CancelState()");
			CancelState();
		}
		catch (Exception exp)
		{
			SetException(exp, "Watcher");
			_watcherCancelSource.Cancel(throwOnFirstException: false);
		}
		finally
		{
			await Task.Yield();
		}
	}

	public long Seek(long offset, SeekOrigin origin)
	{
		if (offset != Position && CanSeek)
		{
			Stream.Seek(offset, origin);
		}
		return Position;
	}

	public void SetLength(long value)
	{
		Stream.SetLength(value);
	}

	private async Task WritePacketOnFile(Packet packet)
	{
		Seek(packet.Position, SeekOrigin.Begin);
		await Stream.WriteAsync(packet.Data).ConfigureAwait(continueOnCapturedContext: false);
		packet.Dispose();
	}

	public async Task FlushAsync()
	{
		await _inputBuffer.WaitToComplete().ConfigureAwait(continueOnCapturedContext: false);
		if (CanRead)
		{
			await Stream.FlushAsync().ConfigureAwait(continueOnCapturedContext: false);
		}
		GC.Collect();
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_watcherCancelSource.Cancel();
			_stream?.Dispose();
			_inputBuffer.Dispose();
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (!_disposed)
		{
			_disposed = true;
			await _watcherCancelSource.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
			await _stream.DisposeAsync().ConfigureAwait(continueOnCapturedContext: false);
			_inputBuffer.Dispose();
		}
	}
}
