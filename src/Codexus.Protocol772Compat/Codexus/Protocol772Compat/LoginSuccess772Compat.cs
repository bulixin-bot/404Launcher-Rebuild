using System;
using System.Collections.Generic;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Packet;
using DotNetty.Buffers;
using Serilog;

namespace Codexus.Protocol772Compat;

[RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ClientBound, 2, new EnumProtocolVersion[]
{
	(EnumProtocolVersion)772,
	(EnumProtocolVersion)773
}, false)]
public sealed class LoginSuccess772Compat : IPacket
{
	private readonly byte[] uuid = new byte[16];

	private string username = "";

	private List<Property>? properties;

	private bool hasStrictErrorHandling;

	private bool strictErrorHandling;

	public EnumProtocolVersion ClientProtocolVersion { get; set; }

	public void ReadFromBuffer(IByteBuffer buffer)
	{
		buffer.ReadBytes(uuid);
		username = buffer.ReadStringFromBuffer(16);
		properties = buffer.ReadProperties();
		hasStrictErrorHandling = buffer.ReadableBytes > 0;
		if (hasStrictErrorHandling)
		{
			strictErrorHandling = buffer.ReadBoolean();
		}
	}

	public void WriteToBuffer(IByteBuffer buffer)
	{
		buffer.WriteBytes(uuid);
		buffer.WriteStringToBuffer(username, 16);
		buffer.WriteProperties(properties);
		if (hasStrictErrorHandling)
		{
			buffer.WriteBoolean(strictErrorHandling);
		}
	}

	public bool HandlePacket(GameConnection connection)
	{
		Log.Information("{Username}({Uuid}) joined through native protocol {Protocol}", username, new Guid(uuid, bigEndian: true), (int)ClientProtocolVersion);
		connection.Uuid = uuid;
		return false;
	}
}
