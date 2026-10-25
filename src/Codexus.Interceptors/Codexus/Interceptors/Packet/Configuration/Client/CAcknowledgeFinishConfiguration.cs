using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Interceptors.Packet.Configuration.Client;

[RegisterPacket(EnumConnectionState.Configuration, EnumPacketDirection.ServerBound, 3, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1206,
	EnumProtocolVersion.V1210
}, false)]
public class CAcknowledgeFinishConfiguration : IPacket
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
		connection.State = EnumConnectionState.Play;
		Log.Debug("Finished configuration.", Array.Empty<object>());
		return false;
	}
}
