using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using Codexus.Interceptors.Packet.Login.Server;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ClientBound, 1, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class EncryptionRequest772 : Protocol772Adapter<SPacketEncryptionRequest>
{
	public EncryptionRequest772()
		: base(new SPacketEncryptionRequest())
	{
	}
}
