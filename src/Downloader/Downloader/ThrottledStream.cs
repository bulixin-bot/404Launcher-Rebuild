using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader;

internal class ThrottledStream : Stream
{
	public const long Infinite = long.MaxValue;

	private readonly Stream _baseStream;

	private long _bandwidthLimit;

	private Bandwidth _bandwidth;

	public long BandwidthLimit
	{
		get
		{
			return _bandwidthLimit;
		}
		set
		{
			_bandwidthLimit = ((value <= 0) ? long.MaxValue : value);
			if (_bandwidth == null)
			{
				_bandwidth = new Bandwidth();
			}
			_bandwidth.BandwidthLimit = _bandwidthLimit;
		}
	}

	public override bool CanRead => _baseStream.CanRead;

	public override bool CanSeek => _baseStream.CanSeek;

	public override bool CanWrite => _baseStream.CanWrite;

	public override long Length => _baseStream.Length;

	public override long Position
	{
		get
		{
			return _baseStream.Position;
		}
		set
		{
			_baseStream.Position = value;
		}
	}

	public ThrottledStream(Stream baseStream, long bandwidthLimit)
	{
		if (bandwidthLimit < 0)
		{
			throw new ArgumentOutOfRangeException("bandwidthLimit", bandwidthLimit, "The maximum number of bytes per second can't be negative.");
		}
		_baseStream = baseStream ?? throw new ArgumentNullException("baseStream");
		BandwidthLimit = bandwidthLimit;
	}

	public override void Flush()
	{
		_baseStream.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _baseStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		_baseStream.SetLength(value);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Throttle(count).Wait();
		return _baseStream.Read(buffer, offset, count);
	}

	public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		await Throttle(count).ConfigureAwait(continueOnCapturedContext: false);
		return await _baseStream.ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Throttle(count).Wait();
		_baseStream.Write(buffer, offset, count);
	}

	public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		await Throttle(count).ConfigureAwait(continueOnCapturedContext: false);
		await _baseStream.WriteAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	public override void Close()
	{
		_baseStream.Close();
		base.Close();
	}

	private async Task Throttle(int transmissionVolume)
	{
		if (BandwidthLimit > 0 && transmissionVolume > 0)
		{
			_bandwidth.CalculateSpeed(transmissionVolume);
			await Sleep(_bandwidth.PopSpeedRetrieveTime()).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private static async Task Sleep(int time)
	{
		if (time > 0)
		{
			await Task.Delay(time).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public override string ToString()
	{
		return _baseStream.ToString();
	}
}
