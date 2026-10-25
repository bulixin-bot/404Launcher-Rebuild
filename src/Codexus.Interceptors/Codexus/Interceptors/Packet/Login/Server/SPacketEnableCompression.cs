using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Interceptors.Packet.Login.Server;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ClientBound, 3, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V108X,
	EnumProtocolVersion.V1200,
	EnumProtocolVersion.V1122,
	EnumProtocolVersion.V1180,
	EnumProtocolVersion.V1210,
	EnumProtocolVersion.V1206
}, false)]
public class SPacketEnableCompression : IPacket
{
	private int CompressionThreshold { get; set; }

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		CompressionThreshold = buffer.ReadVarIntFromBuffer();
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteVarInt(CompressionThreshold);
	}

	public bool HandlePacket(GameConnection connection)
	{
		GameConnection.EnableCompression(connection.ServerChannel, CompressionThreshold);
		return true;
	}
}
