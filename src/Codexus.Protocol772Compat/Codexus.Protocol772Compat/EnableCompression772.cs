using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using Codexus.Interceptors.Packet.Login.Server;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ClientBound, 3, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class EnableCompression772 : Protocol772Adapter<SPacketEnableCompression>
{
	public EnableCompression772()
		: base(new SPacketEnableCompression())
	{
	}
}
