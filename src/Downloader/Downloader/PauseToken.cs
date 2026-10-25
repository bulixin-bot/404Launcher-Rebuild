using System.Threading.Tasks;

namespace Downloader;

public record PauseToken
{
	private readonly PauseTokenSource _tokenSource;

	public bool IsPaused => _tokenSource?.IsPaused ?? false;

	internal PauseToken(PauseTokenSource source)
	{
		_tokenSource = source;
	}

	public Task WaitWhilePausedAsync()
	{
		if (!IsPaused)
		{
			return Task.FromResult(result: true);
		}
		return _tokenSource.WaitWhilePausedAsync();
	}
}
