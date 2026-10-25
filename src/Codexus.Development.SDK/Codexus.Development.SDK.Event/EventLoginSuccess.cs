using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Manager;

namespace Codexus.Development.SDK.Event;

public class EventLoginSuccess : EventArgsBase
{
	public EventLoginSuccess(GameConnection connection)
		: base(connection)
	{
	}
}
