using System;
using System.Threading;

namespace Downloader;

public class Bandwidth
{
	private const double OneSecond = 1000.0;

	private long _count;

	private int _lastSecondCheckpoint;

	private long _lastTransferredBytesCount;

	private int _speedRetrieveTime;

	public double Speed { get; private set; }

	public double AverageSpeed { get; private set; }

	public long BandwidthLimit { get; set; }

	public Bandwidth()
	{
		BandwidthLimit = long.MaxValue;
		Reset();
	}

	public void CalculateSpeed(long receivedBytesCount)
	{
		int num = Environment.TickCount - _lastSecondCheckpoint + 1;
		receivedBytesCount = Interlocked.Add(ref _lastTransferredBytesCount, receivedBytesCount);
		double num2 = (double)receivedBytesCount * 1000.0 / (double)num;
		if ((double)num > 1000.0)
		{
			Speed = num2;
			AverageSpeed = (AverageSpeed * (double)_count + Speed) / (double)(_count + 1);
			_count++;
			SecondCheckpoint();
		}
		if (num2 >= (double)BandwidthLimit)
		{
			int value = (int)((double)receivedBytesCount * 1000.0 / (double)BandwidthLimit - (double)num);
			Interlocked.Add(ref _speedRetrieveTime, value);
		}
	}

	public int PopSpeedRetrieveTime()
	{
		return Interlocked.Exchange(ref _speedRetrieveTime, 0);
	}

	public void Reset()
	{
		SecondCheckpoint();
		_count = 0L;
		Speed = 0.0;
		AverageSpeed = 0.0;
	}

	private void SecondCheckpoint()
	{
		Interlocked.Exchange(ref _lastSecondCheckpoint, Environment.TickCount);
		Interlocked.Exchange(ref _lastTransferredBytesCount, 0L);
	}
}
