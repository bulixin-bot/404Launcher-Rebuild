using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Interceptors.Packet.Configuration.Server;

[RegisterPacket(EnumConnectionState.Play, EnumPacketDirection.ClientBound, 105, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1206,
	EnumProtocolVersion.V1210
}, false)]
public class SStartConfiguration : IPacket
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
		connection.State = EnumConnectionState.Configuration;
		Log.Debug("Starting configuration.", Array.Empty<object>());
		return false;
	}
}
