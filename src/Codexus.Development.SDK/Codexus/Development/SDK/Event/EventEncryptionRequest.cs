using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Manager;

namespace Codexus.Development.SDK.Event;

public class EventEncryptionRequest : EventArgsBase
{
	public string ServerId { get; }

	public EventEncryptionRequest(GameConnection connection, string serverId)
		: base(connection)
	{
		ServerId = serverId;
	}
}
