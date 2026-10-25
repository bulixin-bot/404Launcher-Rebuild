using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using Codexus.Interceptors.Packet.Login.Client;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ServerBound, 3, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class LoginAcknowledged772 : Protocol772Adapter<CPacketLoginAcknowledged>
{
	public LoginAcknowledged772()
		: base(new CPacketLoginAcknowledged())
	{
	}
}
