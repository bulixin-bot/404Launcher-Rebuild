using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Manager;

namespace Codexus.Development.SDK.Event;

public class EventConnectionClosed : EventArgsBase
{
	public EventConnectionClosed(GameConnection connection)
		: base(connection)
	{
	}
}
