using System;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Utils;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Codexus.Interceptors.Handlers;

public class ServerHandler(Interceptor interceptor, EntitySocks5 socks5, string modInfo, string gameId, string forwardAddress, int forwardPort, string nickName, string userId, string userToken, Action<string>? onJoinServer) : ChannelHandlerAdapter
{
	public override void ChannelActive(IChannelHandlerContext context)
	{
		interceptor.ActiveChannels.TryAdd(context.Channel.Id, context.Channel);
		IChannel channel = context.Channel;
		GameConnection gameConnection = new GameConnection(socks5, modInfo, gameId, forwardAddress, forwardPort, nickName, userId, userToken, channel, onJoinServer);
		channel.GetAttribute(ChannelAttribute.Connection).Set(gameConnection);
		gameConnection.Prepare();
	}

	public override void ChannelRead(IChannelHandlerContext context, object message)
	{
		context.Channel.GetAttribute(ChannelAttribute.Connection).Get().OnClientReceived((IByteBuffer)message);
	}

	public override void ChannelInactive(IChannelHandlerContext context)
	{
		interceptor.ActiveChannels.TryRemove(context.Channel.Id, out var _);
		context.Channel.GetAttribute(ChannelAttribute.Connection).Get().Shutdown();
	}
}
