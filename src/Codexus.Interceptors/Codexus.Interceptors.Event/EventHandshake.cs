using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Manager;
using Codexus.Interceptors.Packet.Handshake.Client;

namespace Codexus.Interceptors.Event;

public class EventHandshake : EventArgsBase
{
	public CHandshake Handshake { get; }

	public EventHandshake(GameConnection connection, CHandshake handshake)
		: base(connection)
	{
		Handshake = handshake;
	}
}
