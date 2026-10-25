using System;

namespace Downloader;

public class DownloadStartedEventArgs : EventArgs
{
	public long TotalBytesToReceive { get; }

	public string FileName { get; }

	public DownloadStartedEventArgs(string fileName, long totalBytes)
	{
		FileName = fileName;
		TotalBytesToReceive = totalBytes;
	}
}
