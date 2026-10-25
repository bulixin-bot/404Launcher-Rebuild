using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Protocol772Compat;

public abstract class Protocol772Adapter<TPacket> : IPacket where TPacket : IPacket
{
	private readonly TPacket packet;

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	protected Protocol772Adapter(TPacket packet)
	{
		this.packet = packet;
	}

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		TPacket val = packet;
		val.ClientProtocolVersion = EnumProtocolVersion.V1210;
		packet.ReadFromBuffer(buffer);
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		TPacket val = packet;
		val.ClientProtocolVersion = EnumProtocolVersion.V1210;
		packet.WriteToBuffer(buffer);
	}

	public bool HandlePacket(GameConnection connection)
	{
		TPacket val = packet;
		val.ClientProtocolVersion = EnumProtocolVersion.V1210;
		return packet.HandlePacket(connection);
	}
}
