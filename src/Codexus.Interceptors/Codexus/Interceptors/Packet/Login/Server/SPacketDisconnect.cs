using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Interceptors.Packet.Login.Server;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ClientBound, 0, false)]
public class SPacketDisconnect : IPacket
{
	public string Reason { get; set; } = "";

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		Reason = buffer.ReadStringFromBuffer(32767);
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteStringToBuffer(Reason);
	}

	public bool HandlePacket(GameConnection connection)
	{
		Log.Debug("Disconnect reason: {Reason}", new object[1] { Reason });
		return false;
	}
}
