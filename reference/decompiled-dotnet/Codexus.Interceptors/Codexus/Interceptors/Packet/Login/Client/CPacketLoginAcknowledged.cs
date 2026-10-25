using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Event;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Packet;
using Codexus.Development.SDK.Utils;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Interceptors.Packet.Login.Client;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ServerBound, 3, new EnumProtocolVersion[]
{
	EnumProtocolVersion.V1210,
	EnumProtocolVersion.V1206
}, false)]
public class CPacketLoginAcknowledged : IPacket
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
		if (EventManager.Instance.TriggerEvent(MessageChannels.AllVersions, new EventLoginSuccess(connection)).IsCancelled)
		{
			return true;
		}
		connection.State = EnumConnectionState.Configuration;
		Log.Debug("Configuration started.", Array.Empty<object>());
		return false;
	}
}
