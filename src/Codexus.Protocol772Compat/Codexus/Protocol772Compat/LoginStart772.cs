using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ServerBound, 0, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class LoginStart772 : IPacket
{
	private string profile = "";

	private readonly byte[] uuid = new byte[16];

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		profile = buffer.ReadStringFromBuffer(16);
		buffer.ReadBytes(uuid);
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(profile, 16);
		buffer.WriteBytes(uuid);
	}

	public bool HandlePacket(GameConnection connection)
	{
		profile = connection.NickName;
		return false;
	}
}
