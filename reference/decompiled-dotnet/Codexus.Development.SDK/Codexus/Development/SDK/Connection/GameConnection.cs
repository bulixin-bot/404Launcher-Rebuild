using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Codexus.Development.SDK.Analysis;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Event;
using Codexus.Development.SDK.Extensions;
using Codexus.Development.SDK.Handlers;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Packet;
using Codexus.Development.SDK.Utils;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenTl.Netty.Socks.Handlers;
using Serilog;

namespace Codexus.Development.SDK.Connection;

public class GameConnection : IConnection
{
	public readonly IChannel ClientChannel;

	private bool _initialized;

	private MultithreadEventLoopGroup? _workerGroup;

	public IChannel? ServerChannel;

	private readonly EntitySocks5 _socks5;

	public string NickName { get; set; }

	public EnumProtocolVersion ProtocolVersion { get; set; }

	public EnumConnectionState State { get; set; }

	public Action<string>? OnJoinServer { get; set; }

	public MultithreadEventLoopGroup TaskGroup { get; }

	public GameSession Session { get; set; }

	public string GameId { get; }

	public string ModInfo { get; }

	public int ForwardPort { get; }

	public string ForwardAddress { get; }

	public byte[] Uuid { get; set; }

	public GameConnection(EntitySocks5 socks5, string modInfo, string gameId, string forwardAddress, int forwardPort, string nickName, string userId, string userToken, IChannel channel, Action<string>? onJoinServer)
	{
		_socks5 = socks5;
		ClientChannel = channel;
		NickName = nickName;
		ProtocolVersion = EnumProtocolVersion.None;
		OnJoinServer = onJoinServer;
		TaskGroup = new MultithreadEventLoopGroup();
		Session = new GameSession(nickName, userId, userToken);
		GameId = gameId;
		ModInfo = modInfo;
		ForwardPort = forwardPort;
		ForwardAddress = forwardAddress;
		Uuid = new byte[16];
	}

	public void Prepare()
	{
		_initialized = false;
		if (_workerGroup != null)
		{
			Shutdown();
		}
		_workerGroup = new MultithreadEventLoopGroup();
		Bootstrap bootstrap = new Bootstrap().Group(_workerGroup).Channel<TcpSocketChannel>().Option(ChannelOption.TcpNodelay, value: true)
			.Option(ChannelOption.SoKeepalive, value: true)
			.Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
			.Option(ChannelOption.SoSndbuf, 1048576)
			.Option(ChannelOption.SoRcvbuf, 1048576)
			.Option(ChannelOption.WriteBufferHighWaterMark, 1048576)
			.Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(30.0))
			.Handler(new ActionChannelInitializer<IChannel>((IChannel channel) =>
			{
				if (_socks5.Enabled)
				{
					if (!IPAddress.TryParse(_socks5.Address, out IPAddress address))
					{
						address = Dns.GetHostAddressesAsync(_socks5.Address).GetAwaiter().GetResult()
							.First();
					}
					channel.Pipeline.AddLast("socks5", new Socks5ProxyHandler(new IPEndPoint(address, _socks5.Port), _socks5.Username, _socks5.Password));
				}
				channel.Pipeline.AddLast("splitter", new MessageDeserializer21Bit());
				channel.Pipeline.AddLast("handler", new ClientHandler(this)).AddLast("pre-encoder", new MessageSerializer21Bit()).AddLast("encoder", new MessageSerializer());
			}));
		Task.Run(async () =>
		{
			EventParseAddress finalAddress = EventManager.Instance.TriggerEvent("channel_connection", new EventParseAddress(this, ForwardAddress, ForwardPort));
			IChannel serverChannel = await (IPAddress.TryParse(finalAddress.Address, out IPAddress address) ? bootstrap.ConnectAsync(address, finalAddress.Port) : bootstrap.ConnectAsync(finalAddress.Address, finalAddress.Port)).ContinueWith((Task<IChannel> channel) =>
			{
				if (!channel.IsFaulted)
				{
					return channel.Result;
				}
				Log.Error(channel.Exception, "Failed to connect to remote server {Address}:{Port}", new object[2] { finalAddress.Address, finalAddress.Port });
				return (IChannel)null;
			});
			ServerChannel = serverChannel;
			_initialized = true;
		});
		while (!_initialized)
		{
			Thread.Sleep(100);
		}
		if (ServerChannel == null)
		{
			Shutdown();
		}
	}

	public void OnServerReceived(IByteBuffer buffer)
	{
		HandlePacketReceived(buffer, EnumPacketDirection.ClientBound, (object data) =>
		{
			ClientChannel.WriteAndFlushAsync(data);
		});
	}

	public void OnClientReceived(IByteBuffer buffer)
	{
		HandlePacketReceived(buffer, EnumPacketDirection.ServerBound, (object data) =>
		{
			ServerChannel?.WriteAndFlushAsync(data);
		});
	}

	public void Shutdown()
	{
		EventManager.Instance.TriggerEvent("channel_connection", new EventConnectionClosed(this));
		Log.Debug("Shutting down connection...", Array.Empty<object>());
		TaskGroup.ShutdownGracefullyAsync();
		ClientChannel.CloseAsync();
		ServerChannel?.CloseAsync();
		_workerGroup?.ShutdownGracefullyAsync();
	}

	private void HandlePacketReceived(IByteBuffer buffer, EnumPacketDirection direction, Action<object> onRedirect)
	{
		buffer.MarkReaderIndex();
		int num = buffer.ReadVarIntFromBuffer();
		IPacket packet = PacketManager.Instance.BuildPacket(State, direction, ProtocolVersion, num);
		if (packet == null)
		{
			buffer.ResetReaderIndex();
			onRedirect(buffer);
			return;
		}
		RegisterPacket metadata = PacketManager.Instance.GetMetadata(packet);
		if (metadata != null && metadata.Skip)
		{
			buffer.ResetReaderIndex();
			onRedirect(buffer);
			return;
		}
		packet.ClientProtocolVersion = ProtocolVersion;
		try
		{
			packet.ReadFromBuffer(buffer);
		}
		catch (Exception exception)
		{
			Log.Error(exception, "Cannot read packet from buffer, direction: {Direction}, Id: {Id}, Packet: {Packet}, ProtocolVersion: {ProtocolVersion}", direction, num, packet, ProtocolVersion);
			throw;
		}
		if (packet.HandlePacket(this))
		{
			buffer.ResetReaderIndex();
			return;
		}
		buffer.ResetReaderIndex();
		onRedirect(packet);
	}

	public static void EnableCompression(IChannel channel, int threshold)
	{
		if (threshold < 0)
		{
			if (channel.Pipeline.Get("decompress") is NettyCompressionDecoder)
			{
				channel.Pipeline.Remove("decompress");
			}
			if (channel.Pipeline.Get("compress") is NettyCompressionEncoder)
			{
				channel.Pipeline.Remove("compress");
			}
		}
		else
		{
			if (channel.Pipeline.Get("decompress") is NettyCompressionDecoder nettyCompressionDecoder)
			{
				nettyCompressionDecoder.Threshold = threshold;
			}
			else
			{
				channel.Pipeline.AddAfter("splitter", "decompress", new NettyCompressionDecoder(threshold));
			}
			if (channel.Pipeline.Get("compress") is NettyCompressionEncoder nettyCompressionEncoder)
			{
				nettyCompressionEncoder.Threshold = threshold;
			}
			else
			{
				channel.Pipeline.AddBefore("encoder", "compress", new NettyCompressionEncoder(threshold));
			}
		}
	}

	public static void EnableEncryption(IChannel channel, byte[] secretKey)
	{
		channel.Pipeline.AddBefore("splitter", "decrypt", new NettyEncryptionDecoder(secretKey)).AddBefore("pre-encoder", "encrypt", new NettyEncryptionEncoder(secretKey));
	}
}
