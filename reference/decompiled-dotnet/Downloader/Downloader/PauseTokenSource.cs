using System.Threading;
using System.Threading.Tasks;

namespace Downloader;

public class PauseTokenSource
{
	private volatile TaskCompletionSource<bool> _tcsPaused;

	public PauseToken Token => new PauseToken(this);

	public bool IsPaused => _tcsPaused != null;

	public void Pause()
	{
		Interlocked.CompareExchange(ref _tcsPaused, new TaskCompletionSource<bool>(), null);
	}

	public void Resume()
	{
		TaskCompletionSource<bool> tcsPaused;
		do
		{
			tcsPaused = _tcsPaused;
			if (tcsPaused == null)
			{
				return;
			}
		}
		while (Interlocked.CompareExchange(ref _tcsPaused, null, tcsPaused) != tcsPaused);
		tcsPaused.SetResult(result: true);
	}

	internal Task WaitWhilePausedAsync()
	{
		return _tcsPaused?.Task ?? Task.FromResult(result: true);
	}
}
