using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Interceptors.Packet.Configuration.Server;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ClientBound, 3, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1206,
	EnumProtocolVersion.V1210
}, false)]
public class SFinishConfiguration : IPacket
{
	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
	}

	public bool HandlePacket(GameConnection connection)
	{
		return false;
	}
}
