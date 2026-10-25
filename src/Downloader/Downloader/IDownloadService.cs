using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Downloader;

public interface IDownloadService
{
	bool IsBusy { get; }

	bool IsCancelled { get; }

	DownloadPackage Package { get; }

	DownloadStatus Status { get; }

	event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;

	event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

	event EventHandler<DownloadProgressChangedEventArgs> ChunkDownloadProgressChanged;

	event EventHandler<DownloadStartedEventArgs> DownloadStarted;

	Task<Stream> DownloadFileTaskAsync(DownloadPackage package, CancellationToken cancellationToken = default(CancellationToken));

	Task<Stream> DownloadFileTaskAsync(DownloadPackage package, string address, CancellationToken cancellationToken = default(CancellationToken));

	Task<Stream> DownloadFileTaskAsync(DownloadPackage package, string[] urls, CancellationToken cancellationToken = default(CancellationToken));

	Task<Stream> DownloadFileTaskAsync(string address, CancellationToken cancellationToken = default(CancellationToken));

	Task<Stream> DownloadFileTaskAsync(string[] urls, CancellationToken cancellationToken = default(CancellationToken));

	Task DownloadFileTaskAsync(string address, string fileName, CancellationToken cancellationToken = default(CancellationToken));

	Task DownloadFileTaskAsync(string[] urls, string fileName, CancellationToken cancellationToken = default(CancellationToken));

	Task DownloadFileTaskAsync(string address, DirectoryInfo folder, CancellationToken cancellationToken = default(CancellationToken));

	Task DownloadFileTaskAsync(string[] urls, DirectoryInfo folder, CancellationToken cancellationToken = default(CancellationToken));

	void CancelAsync();

	Task CancelTaskAsync();

	void Pause();

	void Resume();

	Task Clear();

	void AddLogger(ILogger logger);
}
