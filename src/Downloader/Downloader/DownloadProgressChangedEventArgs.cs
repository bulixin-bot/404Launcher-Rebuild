using System;

namespace Downloader;

public class DownloadProgressChangedEventArgs : EventArgs
{
	public string ProgressId { get; }

	public double ProgressPercentage
	{
		get
		{
			if (TotalBytesToReceive != 0L)
			{
				return (double)ReceivedBytesSize * 100.0 / (double)TotalBytesToReceive;
			}
			return 0.0;
		}
	}

	public long ReceivedBytesSize { get; internal set; }

	public long TotalBytesToReceive { get; internal set; }

	public double BytesPerSecondSpeed { get; internal set; }

	public double AverageBytesPerSecondSpeed { get; internal set; }

	public long ProgressedByteSize { get; internal set; }

	public byte[] ReceivedBytes { get; internal set; }

	public int ActiveChunks { get; internal set; }

	public DownloadProgressChangedEventArgs(string id)
	{
		ProgressId = id ?? "Main";
	}
}
