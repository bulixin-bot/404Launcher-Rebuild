using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Codexus.Development.SDK.Analysis;
using Codexus.Development.SDK.Connection;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Event;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Utils;
using Codexus.Interceptors.Handlers;
using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Serilog;

namespace Codexus.Interceptors;

public class Interceptor
{
	public readonly ConcurrentDictionary<IChannelId, IChannel> ActiveChannels;

	private IEventLoopGroup acceptorGroup;

	private IEventLoopGroup workerGroup;

	public Guid Identifier { get; }

	public string LocalAddress { get; set; }

	public int LocalPort { get; set; }

	public string NickName { get; set; }

	public string ForwardAddress { get; private set; }

	public int ForwardPort { get; private set; }

	public string ServerName { get; set; }

	public string ServerVersion { get; set; }

	private IChannel? Channel { get; set; }

	private UdpBroadcaster? UdpBroadcaster { get; set; }

	public Interceptor(MultithreadEventLoopGroup acceptorGroup, MultithreadEventLoopGroup workerGroup, string serverName, string serverVersion, string forwardAddress, int forwardPort, string localAddress, int localPort, string nickName)
	{
		this.acceptorGroup = acceptorGroup;
		this.workerGroup = workerGroup;
		ActiveChannels = new ConcurrentDictionary<IChannelId, IChannel>();
		Identifier = Guid.NewGuid();
		LocalAddress = localAddress;
		LocalPort = localPort;
		NickName = nickName;
		ForwardAddress = forwardAddress;
		ForwardPort = forwardPort;
		ServerName = serverName;
		ServerVersion = serverVersion;
	}

	public static Interceptor CreateInterceptor(EntitySocks5 socks5, string modInfo, string gameId, string serverName, string serverVersion, string forwardAddress, int forwardPort, string nickName, string userId, string userToken, Action<string>? onJoinServer = null, string localAddress = "127.0.0.1", int localPort = 6445)
	{
		EventCreateInterceptor eventCreateInterceptor = EventManager.Instance.TriggerEvent("channel_interceptor", new EventCreateInterceptor(localPort));
		if (eventCreateInterceptor.IsCancelled)
		{
			throw new InvalidOperationException("Create Interceptor cancelled");
		}
		int availablePort = NetworkUtil.GetAvailablePort(eventCreateInterceptor.Port, 35565, reuseTimeWait: true);
		MultithreadEventLoopGroup parentGroup = new MultithreadEventLoopGroup();
		MultithreadEventLoopGroup childGroup = new MultithreadEventLoopGroup();
		Interceptor interceptor = new Interceptor(parentGroup, childGroup, serverName, serverVersion, forwardAddress, forwardPort, localAddress, availablePort, nickName);
		ServerBootstrap serverBootstrap = new ServerBootstrap().Group(parentGroup, childGroup).Channel<TcpServerSocketChannel>().Option(ChannelOption.SoReuseaddr, value: true)
			.Option(ChannelOption.SoReuseport, value: true)
			.Option(ChannelOption.TcpNodelay, value: true)
			.Option(ChannelOption.SoKeepalive, value: true)
			.Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
			.Option(ChannelOption.SoSndbuf, 1048576)
			.Option(ChannelOption.SoRcvbuf, 1048576)
			.Option(ChannelOption.WriteBufferHighWaterMark, 1048576)
			.Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(10.0))
			.ChildHandler(new ActionChannelInitializer<IChannel>((IChannel channel) =>
			{
				channel.Pipeline.AddLast("splitter", (IChannelHandler)new MessageDeserializer21Bit()).AddLast("handler", new ServerHandler(interceptor, socks5, modInfo, gameId, forwardAddress, forwardPort, nickName, userId, userToken, onJoinServer)).AddLast("pre-encoder", (IChannelHandler)new MessageSerializer21Bit())
					.AddLast("encoder", (IChannelHandler)new MessageSerializer());
			}))
			.LocalAddress(IPAddress.Any, availablePort);
		Log.Information("请通过{Address}游玩", new object[1] { $"{localAddress}:{availablePort}" });
		Log.Information("您的名字:{Name}", new object[1] { nickName });
		interceptor.UdpBroadcaster = new UdpBroadcaster("224.0.2.60", 4445, availablePort, forwardAddress, nickName, serverVersion.Contains("1.8.") || serverVersion.Contains("1.7."));
		serverBootstrap.BindAsync().ContinueWith((Task<IChannel> task) =>
		{
			if (task.IsCompletedSuccessfully)
			{
				interceptor.Channel = task.Result;
			}
		}).ContinueWith((Task _) => interceptor.UdpBroadcaster.StartBroadcastingAsync());
		return interceptor;
	}

	public async Task ChangeForwardAddressAsync(string newAddress, int newPort)
	{
		ForwardAddress = newAddress;
		ForwardPort = newPort;
		UdpBroadcaster?.Stop();
		UdpBroadcaster = new UdpBroadcaster("224.0.2.60", 4445, LocalPort, ForwardAddress, NickName, ServerVersion.Contains("1.8.") || ServerVersion.Contains("1.7."));
		await UdpBroadcaster.StartBroadcastingAsync();
	}

	public void ShutdownAsync()
	{
		try
		{
			UdpBroadcaster?.Stop();
			foreach (var (_, channel2) in ActiveChannels)
			{
				GameConnection gameConnection = channel2.GetAttribute(ChannelAttribute.Connection).Get();
				channel2.GetAttribute(ChannelAttribute.Connection).Remove();
				gameConnection.Shutdown();
			}
			Channel?.CloseAsync();
			((AbstractEventExecutorGroup)acceptorGroup).ShutdownGracefullyAsync();
			((AbstractEventExecutorGroup)workerGroup).ShutdownGracefullyAsync();
		}
		catch
		{
		}
	}

	public static void EnsureLoaded()
	{
		if (typeof(Interceptor).Assembly.GetName().Name == null)
		{
			throw new InvalidOperationException("Should never call CheckIsLoaded()");
		}
	}
}
