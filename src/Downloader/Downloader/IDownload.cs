using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader;

public interface IDownload : IDisposable, IAsyncDisposable
{
	string Url { get; }

	string Folder { get; }

	string Filename { get; }

	long DownloadedFileSize { get; }

	long TotalFileSize { get; }

	DownloadPackage Package { get; }

	DownloadStatus Status { get; }

	event EventHandler<DownloadProgressChangedEventArgs> ChunkDownloadProgressChanged;

	event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;

	event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

	event EventHandler<DownloadStartedEventArgs> DownloadStarted;

	Task<Stream> StartAsync(CancellationToken cancellationToken = default(CancellationToken));

	void Stop();

	void Pause();

	void Resume();
}
