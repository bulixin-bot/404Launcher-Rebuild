using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using Codexus.Interceptors.Packet.Configuration.Server;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ClientBound, 3, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class FinishConfiguration772 : Protocol772Adapter<SFinishConfiguration>
{
	public FinishConfiguration772()
		: base(new SFinishConfiguration())
	{
	}
}
