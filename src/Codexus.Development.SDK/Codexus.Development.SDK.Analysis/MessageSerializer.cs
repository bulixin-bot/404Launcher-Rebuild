using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Packet;
using Codexus.Development.SDK.Utils;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Codexus.Development.SDK.Analysis;

public class MessageSerializer : MessageToByteEncoder<IPacket>
{
	protected override void Encode(IChannelHandlerContext context, IPacket message, IByteBuffer output)
	{
		GameConnection gameConnection = context.Channel.GetAttribute(ChannelAttribute.Connection).Get();
		int packetId = PacketManager.Instance.GetPacketId(gameConnection.ProtocolVersion, message);
		if (packetId != -1)
		{
			output.WriteVarInt(packetId);
			message.WriteToBuffer(output);
		}
	}
}
