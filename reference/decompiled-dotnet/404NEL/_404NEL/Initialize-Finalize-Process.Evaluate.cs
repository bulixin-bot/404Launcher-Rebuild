using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace _404NEL;

internal sealed class Initialize_0020Finalize_0020Process_002EEvaluate : IDisposable
{
	private sealed record UpstreamRoute(string Address, int Port, string HandshakeHost, int HandshakePort);

	private sealed record HandshakePacket(int ProtocolVersion, string ServerAddress, int ServerPort, int NextState);

	private static readonly object Transform_0020Materialize_0020Validate_002EObserve = new object();

	private static readonly SemaphoreSlim Normalize_0020Normalize_0020Process_002ETransform = new SemaphoreSlim(256, 256);

	private readonly TcpListener Transform_0020Dispatch_0020Evaluate_002EAcquire;

	private readonly IPAddress Transform_0020Schedule_0020Resolve_002EWorker;

	private UpstreamRoute Normalize_0020Transform_0020Schedule_002ESchedule;

	private readonly bool Dispatch_0020Synchronize_0020Dispatch_002EEvaluate;

	private readonly Func<string, int, CancellationToken, Task<TcpClient>>? Normalize_0020Acquire_0020Dispatch_002EWorker;

	private readonly string Transform_0020Worker_0020Worker_002EValidate;

	private readonly Func<Task<string?>>? Acquire_0020Observe_0020Acquire_002EDispatch;

	private readonly Action<string, string>? Dispatch_0020Normalize_0020Normalize_002EProcess;

	private readonly CancellationTokenSource Validate_0020Worker_0020Schedule_002EWorker = new CancellationTokenSource();

	private readonly ConcurrentDictionary<TcpClient, byte> Normalize_0020Resolve_0020Observe_002ENormalize = new ConcurrentDictionary<TcpClient, byte>();

	private int Schedule_0020Resolve_0020Dispatch_002EMaterialize;

	private int Acquire_0020Normalize_0020Evaluate_002EDispatch;

	public int Port { get; }

	public string ListenAddress => Transform_0020Schedule_0020Resolve_002EWorker.ToString();

	internal CancellationToken ShutdownToken => Validate_0020Worker_0020Schedule_002EWorker.Token;

	public Initialize_0020Finalize_0020Process_002EEvaluate(int preferredPort, string listenAddress, string targetAddress, int targetPort, string handshakeHost, int handshakePort, string motd, Func<Task<string?>>? loginPreflightAsync = null, Action<string, string>? log = null, bool forwardToUpstream = false, Func<string, int, CancellationToken, Task<TcpClient>>? upstreamConnector = null)
	{
		Transform_0020Schedule_0020Resolve_002EWorker = IPAddress.Parse(listenAddress);
		Normalize_0020Transform_0020Schedule_002ESchedule = new UpstreamRoute(targetAddress, targetPort, string.IsNullOrWhiteSpace(handshakeHost) ? targetAddress : handshakeHost.Trim(), (handshakePort > 0 && handshakePort <= 65535) ? handshakePort : targetPort);
		Dispatch_0020Synchronize_0020Dispatch_002EEvaluate = forwardToUpstream;
		Normalize_0020Acquire_0020Dispatch_002EWorker = upstreamConnector;
		Transform_0020Worker_0020Worker_002EValidate = motd;
		Acquire_0020Observe_0020Acquire_002EDispatch = loginPreflightAsync;
		Dispatch_0020Normalize_0020Normalize_002EProcess = log;
		Transform_0020Dispatch_0020Evaluate_002EAcquire = Finalize_0020Initialize_0020Dispatch_002EValidate(Transform_0020Schedule_0020Resolve_002EWorker, preferredPort, out var Acquire_0020Resolve_0020Normalize_002EAcquire);
		Port = Acquire_0020Resolve_0020Normalize_002EAcquire;
	}

	public void Acquire_0020Resolve_0020Evaluate_002ENormalize()
	{
		if (Interlocked.Exchange(ref Schedule_0020Resolve_0020Dispatch_002EMaterialize, 1) == 0)
		{
			UpstreamRoute upstreamRoute = Volatile.Read(in Normalize_0020Transform_0020Schedule_002ESchedule);
			string text = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Resolve_0020Schedule_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 4);
			defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Transform_0020Synchronize_0020Schedule_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
			defaultInterpolatedStringHandler.AppendFormatted(ListenAddress);
			defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Transform_0020Acquire_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
			defaultInterpolatedStringHandler.AppendFormatted(Port);
			defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Resolve_0020Dispatch_0020Normalize_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
			defaultInterpolatedStringHandler.AppendFormatted(upstreamRoute.Address);
			defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Dispatch_0020Normalize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
			defaultInterpolatedStringHandler.AppendFormatted(upstreamRoute.Port);
			Worker_0020Schedule_0020Process_002EProcess(text, defaultInterpolatedStringHandler.ToStringAndClear());
			AcceptLoopAsync();
		}
	}

	internal bool Worker_0020Process_0020Initialize_002EAcquire(string Synchronize_0020Normalize_0020Materialize_002EInitialize, int Observe_0020Observe_0020Schedule_002EWorker)
	{
		if (!Dispatch_0020Synchronize_0020Dispatch_002EEvaluate || Volatile.Read(in Acquire_0020Normalize_0020Evaluate_002EDispatch) != 0)
		{
			return false;
		}
		Volatile.Write(ref Normalize_0020Transform_0020Schedule_002ESchedule, new UpstreamRoute(Synchronize_0020Normalize_0020Materialize_002EInitialize, Observe_0020Observe_0020Schedule_002EWorker, Synchronize_0020Normalize_0020Materialize_002EInitialize, Observe_0020Observe_0020Schedule_002EWorker));
		return true;
	}

	internal async Task<TcpClient> ConnectUpstreamAsync(string address, int port, CancellationToken cancellation)
	{
		if (Normalize_0020Acquire_0020Dispatch_002EWorker != null)
		{
			return await Normalize_0020Acquire_0020Dispatch_002EWorker(address, port, cancellation);
		}
		TcpClient client = new TcpClient();
		try
		{
			await client.ConnectAsync(address, port, cancellation);
			return client;
		}
		catch
		{
			client.Dispose();
			throw;
		}
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref Acquire_0020Normalize_0020Evaluate_002EDispatch, 1) != 0)
		{
			return;
		}
		Validate_0020Worker_0020Schedule_002EWorker.Cancel();
		Transform_0020Dispatch_0020Evaluate_002EAcquire.Stop();
		string text = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Observe_0020Transform_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 3);
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Dispatch_0020Synchronize_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendFormatted(ListenAddress);
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Evaluate_0020Worker_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendFormatted(Port);
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Synchronize_0020Finalize_0020Resolve_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendFormatted(Normalize_0020Resolve_0020Observe_002ENormalize.Count);
		Worker_0020Schedule_0020Process_002EProcess(text, defaultInterpolatedStringHandler.ToStringAndClear());
		foreach (TcpClient key in Normalize_0020Resolve_0020Observe_002ENormalize.Keys)
		{
			key.Dispose();
		}
	}

	private async Task AcceptLoopAsync()
	{
		while (!Validate_0020Worker_0020Schedule_002EWorker.IsCancellationRequested)
		{
			try
			{
				TcpClient client = await Transform_0020Dispatch_0020Evaluate_002EAcquire.AcceptTcpClientAsync(Validate_0020Worker_0020Schedule_002EWorker.Token);
				Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"收到连接：远端={client.Client.RemoteEndPoint}，入口={ListenAddress}:{Port}");
				if (!(await Normalize_0020Normalize_0020Process_002ETransform.WaitAsync(TimeSpan.Zero, Validate_0020Worker_0020Schedule_002EWorker.Token)))
				{
					Worker_0020Schedule_0020Process_002EProcess("WARN", "拒绝连接：达到全局连接上限，远端=" + client.Client.RemoteEndPoint);
					client.Dispose();
				}
				else
				{
					Normalize_0020Resolve_0020Observe_002ENormalize.TryAdd(client, 0);
					HandleTrackedClientAsync(client);
				}
			}
			catch (OperationCanceledException)
			{
				break;
			}
			catch (ObjectDisposedException)
			{
				break;
			}
		}
	}

	private async Task HandleTrackedClientAsync(TcpClient client)
	{
		try
		{
			await HandleClientAsync(client);
		}
		finally
		{
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"连接关闭：远端={client.Client.RemoteEndPoint}");
			Normalize_0020Resolve_0020Observe_002ENormalize.TryRemove(client, out var _);
			client.Dispose();
			Normalize_0020Normalize_0020Process_002ETransform.Release();
		}
	}

	private async Task HandleClientAsync(TcpClient client)
	{
		_ = 7;
		try
		{
			NetworkStream stream = client.GetStream();
			(byte[], byte[])? tuple = await ReadFrameAsync(stream, Validate_0020Worker_0020Schedule_002EWorker.Token);
			if (!tuple.HasValue)
			{
				Worker_0020Schedule_0020Process_002EProcess("DEBUG", "连接在握手前关闭");
				return;
			}
			HandshakePacket handshake = Materialize_0020Schedule_0020Initialize_002EMaterialize(tuple.Value.Item2);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"收到握手：协议={handshake.ProtocolVersion}，地址={handshake.ServerAddress}:{handshake.ServerPort}，状态={handshake.NextState}");
			if (handshake.NextState == 1 && !Dispatch_0020Synchronize_0020Dispatch_002EEvaluate)
			{
				Worker_0020Schedule_0020Process_002EProcess("DEBUG", "处理服务器状态查询");
				await HandleStatusAsync(stream, handshake.ProtocolVersion, Validate_0020Worker_0020Schedule_002EWorker.Token);
				return;
			}
			string text = ((Acquire_0020Observe_0020Acquire_002EDispatch != null) ? (await Acquire_0020Observe_0020Acquire_002EDispatch()) : null);
			string text2 = text;
			if (!string.IsNullOrWhiteSpace(text2))
			{
				Worker_0020Schedule_0020Process_002EProcess("WARN", "登录预检失败，已向客户端发送断开原因：" + text2);
				await Schedule_0020Materialize_0020Process_002EResolve(stream, text2);
				return;
			}
			UpstreamRoute route = Volatile.Read(in Normalize_0020Transform_0020Schedule_002ESchedule);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"正在连接内部代理：{route.Address}:{route.Port}");
			using TcpClient upstream = await ConnectUpstreamAsync(route.Address, route.Port, Validate_0020Worker_0020Schedule_002EWorker.Token);
			NetworkStream upstreamStream = upstream.GetStream();
			string text3 = route.HandshakeHost;
			int num = handshake.ServerAddress.IndexOf('\0');
			if (Dispatch_0020Synchronize_0020Dispatch_002EEvaluate && num >= 0)
			{
				string text4 = text3;
				text = handshake.ServerAddress;
				int num2 = num;
				text3 = text4 + text.Substring(num2, text.Length - num2);
			}
			byte[] array = Evaluate_0020Normalize_0020Evaluate_002EProcess(handshake, text3, route.HandshakePort);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"已改写登录握手地址：{handshake.ServerAddress}:{handshake.ServerPort} -> {route.HandshakeHost}:{route.HandshakePort}");
			await upstreamStream.WriteAsync(array, Validate_0020Worker_0020Schedule_002EWorker.Token);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", "内部代理连接成功，开始双向转发");
			using CancellationTokenSource forwarding = CancellationTokenSource.CreateLinkedTokenSource(Validate_0020Worker_0020Schedule_002EWorker.Token);
			Task clientToUpstream = ForwardWithTraceAsync(stream, upstreamStream, "客户端->内部代理", forwarding.Token);
			Task upstreamToClient = ForwardWithTraceAsync(upstreamStream, stream, "内部代理->客户端", forwarding.Token);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", (await Task.WhenAny(clientToUpstream, upstreamToClient) == clientToUpstream) ? "客户端到代理方向结束，停止连接" : "上游代理到客户端方向结束，停止连接");
			forwarding.Cancel();
			await IgnoreConnectionErrorsAsync(clientToUpstream, upstreamToClient);
		}
		catch (OperationCanceledException)
		{
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", "连接因代理关闭而取消");
		}
		catch (EndOfStreamException)
		{
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", "对端在数据包读取完成前关闭连接");
		}
		catch (IOException ex3)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", "连接发生 IO 异常：" + ex3.Message);
		}
		catch (SocketException ex4)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", $"连接发生 Socket 异常：{ex4.SocketErrorCode}，{ex4.Message}");
		}
		catch (InvalidDataException ex5)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", "收到无效协议数据：" + ex5.Message);
		}
		catch (ArgumentOutOfRangeException ex6)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", "握手数据超出范围：" + ex6.Message);
		}
	}

	private async Task ForwardWithTraceAsync(Stream source, Stream destination, string direction, CancellationToken cancellation)
	{
		byte[] buffer = new byte[32768];
		long totalBytes = 0L;
		int chunks = 0;
		try
		{
			while (true)
			{
				int read = await source.ReadAsync(buffer, cancellation);
				if (read == 0)
				{
					break;
				}
				await destination.WriteAsync(buffer.AsMemory(0, read), cancellation);
				await destination.FlushAsync(cancellation);
				totalBytes += read;
				chunks++;
			}
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"{direction} 转发结束：对端关闭，累计={totalBytes} 字节，块数={chunks}");
		}
		catch (OperationCanceledException)
		{
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", $"{direction} 转发取消：累计={totalBytes} 字节，块数={chunks}");
			throw;
		}
		catch (IOException ex2)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", $"{direction} 转发 IO 异常：累计={totalBytes} 字节，块数={chunks}，{ex2.Message}");
			throw;
		}
		catch (SocketException ex3)
		{
			Worker_0020Schedule_0020Process_002EProcess("WARN", $"{direction} 转发 Socket 异常：累计={totalBytes} 字节，块数={chunks}，{ex3.SocketErrorCode}，{ex3.Message}");
			throw;
		}
	}

	private async Task HandleStatusAsync(NetworkStream stream, int protocolVersion, CancellationToken cancellation)
	{
		if (!(await ReadFrameAsync(stream, cancellation)).HasValue)
		{
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", "状态查询连接在请求前关闭");
			return;
		}
		string s = JsonSerializer.Serialize(new
		{
			version = new
			{
				name = "404 Proxy",
				protocol = protocolVersion
			},
			players = new
			{
				max = 500,
				online = 0,
				sample = Array.Empty<object>()
			},
			description = new
			{
				text = Transform_0020Worker_0020Worker_002EValidate
			}
		});
		await WritePacketAsync(stream, 0, Encoding.UTF8.GetBytes(s));
		Worker_0020Schedule_0020Process_002EProcess("DEBUG", "已发送服务器状态响应");
		(byte[], byte[])? tuple = await ReadFrameAsync(stream, cancellation);
		if (tuple.HasValue)
		{
			await stream.WriteAsync(tuple.Value.Item1, cancellation);
			Worker_0020Schedule_0020Process_002EProcess("DEBUG", "已响应状态 Ping");
		}
	}

	private void Worker_0020Schedule_0020Process_002EProcess(string Normalize_0020Transform_0020Finalize_002EValidate, string Evaluate_0020Validate_0020Process_002EValidate)
	{
		Dispatch_0020Normalize_0020Normalize_002EProcess?.Invoke(Normalize_0020Transform_0020Finalize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Observe_0020Normalize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) + Evaluate_0020Validate_0020Process_002EValidate);
	}

	private static async Task IgnoreConnectionErrorsAsync(params Task[] tasks)
	{
		try
		{
			await Task.WhenAll(tasks);
		}
		catch (OperationCanceledException)
		{
		}
		catch (IOException)
		{
		}
		catch (SocketException)
		{
		}
	}

	private static Task Schedule_0020Materialize_0020Process_002EResolve(NetworkStream Resolve_0020Normalize_0020Synchronize_002EValidate, string Materialize_0020Worker_0020Finalize_002EValidate)
	{
		string s = JsonSerializer.Serialize(new
		{
			text = Materialize_0020Worker_0020Finalize_002EValidate
		});
		return WritePacketAsync(Resolve_0020Normalize_0020Synchronize_002EValidate, 0, Encoding.UTF8.GetBytes(s));
	}

	private static HandshakePacket Materialize_0020Schedule_0020Initialize_002EMaterialize(byte[] Resolve_0020Initialize_0020Resolve_002EDispatch)
	{
		int Process_0020Validate_0020Normalize_002EAcquire = 0;
		Synchronize_0020Initialize_0020Dispatch_002EAcquire(Resolve_0020Initialize_0020Resolve_002EDispatch, ref Process_0020Validate_0020Normalize_002EAcquire);
		int protocolVersion = Synchronize_0020Initialize_0020Dispatch_002EAcquire(Resolve_0020Initialize_0020Resolve_002EDispatch, ref Process_0020Validate_0020Normalize_002EAcquire);
		string serverAddress = Process_0020Schedule_0020Evaluate_002EEvaluate(Resolve_0020Initialize_0020Resolve_002EDispatch, ref Process_0020Validate_0020Normalize_002EAcquire);
		int serverPort = Resolve_0020Dispatch_0020Observe_002EResolve(Resolve_0020Initialize_0020Resolve_002EDispatch, ref Process_0020Validate_0020Normalize_002EAcquire);
		return new HandshakePacket(protocolVersion, serverAddress, serverPort, Synchronize_0020Initialize_0020Dispatch_002EAcquire(Resolve_0020Initialize_0020Resolve_002EDispatch, ref Process_0020Validate_0020Normalize_002EAcquire));
	}

	private static byte[] Evaluate_0020Normalize_0020Evaluate_002EProcess(HandshakePacket Validate_0020Evaluate_0020Acquire_002ESchedule, string Acquire_0020Schedule_0020Acquire_002ESynchronize, int Synchronize_0020Evaluate_0020Schedule_002ENormalize)
	{
		using MemoryStream memoryStream = new MemoryStream();
		Acquire_0020Acquire_0020Materialize_002EInitialize(memoryStream, 0);
		Acquire_0020Acquire_0020Materialize_002EInitialize(memoryStream, Validate_0020Evaluate_0020Acquire_002ESchedule.ProtocolVersion);
		Materialize_0020Evaluate_0020Worker_002EProcess(memoryStream, Acquire_0020Schedule_0020Acquire_002ESynchronize);
		Transform_0020Synchronize_0020Dispatch_002EResolve(memoryStream, Synchronize_0020Evaluate_0020Schedule_002ENormalize);
		Acquire_0020Acquire_0020Materialize_002EInitialize(memoryStream, Validate_0020Evaluate_0020Acquire_002ESchedule.NextState);
		byte[] array = memoryStream.ToArray();
		using MemoryStream memoryStream2 = new MemoryStream();
		Acquire_0020Acquire_0020Materialize_002EInitialize(memoryStream2, array.Length);
		memoryStream2.Write(array);
		return memoryStream2.ToArray();
	}

	private static async Task WritePacketAsync(NetworkStream stream, int packetId, byte[] payload)
	{
		using MemoryStream packet = new MemoryStream();
		Acquire_0020Acquire_0020Materialize_002EInitialize(packet, packetId);
		Acquire_0020Acquire_0020Materialize_002EInitialize(packet, payload.Length);
		packet.Write(payload);
		byte[] array = packet.ToArray();
		using MemoryStream frame = new MemoryStream();
		Acquire_0020Acquire_0020Materialize_002EInitialize(frame, array.Length);
		frame.Write(array);
		await stream.WriteAsync(frame.ToArray());
	}

	private static async Task<(byte[] Raw, byte[] Payload)?> ReadFrameAsync(NetworkStream stream, CancellationToken cancellation)
	{
		List<byte> lengthBytes = new List<byte>(5);
		int num = await ReadVarIntAsync(stream, lengthBytes, cancellation);
		if (num < 0 || num > 2097152)
		{
			return null;
		}
		byte[] payload = new byte[num];
		await stream.ReadExactlyAsync(payload, cancellation);
		byte[] array = new byte[lengthBytes.Count + payload.Length];
		lengthBytes.CopyTo(array, 0);
		payload.CopyTo(array, lengthBytes.Count);
		return (array, payload);
	}

	private static async Task<int> ReadVarIntAsync(NetworkStream stream, List<byte> raw, CancellationToken cancellation)
	{
		int value = 0;
		for (int position = 0; position < 35; position += 7)
		{
			byte[] buffer = new byte[1];
			if (await stream.ReadAsync(buffer, cancellation) == 0)
			{
				return -1;
			}
			byte b = buffer[0];
			raw.Add(b);
			value |= (b & 0x7F) << position;
			if ((b & 0x80) == 0)
			{
				return value;
			}
		}
		throw new InvalidDataException("VarInt is too large");
	}

	private static int Synchronize_0020Initialize_0020Dispatch_002EAcquire(byte[] Evaluate_0020Resolve_0020Process_002ETransform, ref int Process_0020Validate_0020Normalize_002EAcquire)
	{
		int num = 0;
		for (int i = 0; i < 35; i += 7)
		{
			byte b = Evaluate_0020Resolve_0020Process_002ETransform[Process_0020Validate_0020Normalize_002EAcquire++];
			num |= (b & 0x7F) << i;
			if ((b & 0x80) == 0)
			{
				return num;
			}
		}
		throw new InvalidDataException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Evaluate_0020Normalize_002ENormalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}

	private static string Process_0020Schedule_0020Evaluate_002EEvaluate(byte[] Synchronize_0020Process_0020Initialize_002ESynchronize, ref int Materialize_0020Validate_0020Finalize_002EWorker)
	{
		int num = Synchronize_0020Initialize_0020Dispatch_002EAcquire(Synchronize_0020Process_0020Initialize_002ESynchronize, ref Materialize_0020Validate_0020Finalize_002EWorker);
		if (num < 0 || Materialize_0020Validate_0020Finalize_002EWorker + num > Synchronize_0020Process_0020Initialize_002ESynchronize.Length)
		{
			throw new InvalidDataException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Observe_0020Transform_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		}
		string result = Encoding.UTF8.GetString(Synchronize_0020Process_0020Initialize_002ESynchronize, Materialize_0020Validate_0020Finalize_002EWorker, num);
		Materialize_0020Validate_0020Finalize_002EWorker += num;
		return result;
	}

	private static int Resolve_0020Dispatch_0020Observe_002EResolve(byte[] Process_0020Evaluate_0020Schedule_002EWorker, ref int Initialize_0020Validate_0020Dispatch_002ETransform)
	{
		if (Initialize_0020Validate_0020Dispatch_002ETransform + 2 > Process_0020Evaluate_0020Schedule_002EWorker.Length)
		{
			throw new InvalidDataException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Transform_0020Schedule_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		}
		return (Process_0020Evaluate_0020Schedule_002EWorker[Initialize_0020Validate_0020Dispatch_002ETransform++] << 8) | Process_0020Evaluate_0020Schedule_002EWorker[Initialize_0020Validate_0020Dispatch_002ETransform++];
	}

	private static void Acquire_0020Acquire_0020Materialize_002EInitialize(Stream Initialize_0020Acquire_0020Finalize_002EInitialize, int Initialize_0020Acquire_0020Validate_002EEvaluate)
	{
		do
		{
			byte b = (byte)(Initialize_0020Acquire_0020Validate_002EEvaluate & 0x7F);
			Initialize_0020Acquire_0020Validate_002EEvaluate >>>= 7;
			if (Initialize_0020Acquire_0020Validate_002EEvaluate != 0)
			{
				b |= 0x80;
			}
			Initialize_0020Acquire_0020Finalize_002EInitialize.WriteByte(b);
		}
		while (Initialize_0020Acquire_0020Validate_002EEvaluate != 0);
	}

	private static void Materialize_0020Evaluate_0020Worker_002EProcess(Stream Synchronize_0020Observe_0020Normalize_002EMaterialize, string Acquire_0020Transform_0020Observe_002ETransform)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(Acquire_0020Transform_0020Observe_002ETransform);
		Acquire_0020Acquire_0020Materialize_002EInitialize(Synchronize_0020Observe_0020Normalize_002EMaterialize, bytes.Length);
		Synchronize_0020Observe_0020Normalize_002EMaterialize.Write(bytes);
	}

	private static void Transform_0020Synchronize_0020Dispatch_002EResolve(Stream Synchronize_0020Dispatch_0020Transform_002EWorker, int Evaluate_0020Synchronize_0020Finalize_002EWorker)
	{
		if ((Evaluate_0020Synchronize_0020Finalize_002EWorker < 0 || Evaluate_0020Synchronize_0020Finalize_002EWorker > 65535) ? true : false)
		{
			throw new ArgumentOutOfRangeException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Schedule_0020Synchronize_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		}
		Synchronize_0020Dispatch_0020Transform_002EWorker.WriteByte((byte)((Evaluate_0020Synchronize_0020Finalize_002EWorker >> 8) & 0xFF));
		Synchronize_0020Dispatch_0020Transform_002EWorker.WriteByte((byte)(Evaluate_0020Synchronize_0020Finalize_002EWorker & 0xFF));
	}

	private static TcpListener Finalize_0020Initialize_0020Dispatch_002EValidate(IPAddress Process_0020Normalize_0020Initialize_002ENormalize, int Evaluate_0020Materialize_0020Observe_002EMaterialize, out int Acquire_0020Resolve_0020Normalize_002EAcquire)
	{
		lock (Transform_0020Materialize_0020Validate_002EObserve)
		{
			for (int i = Evaluate_0020Materialize_0020Observe_002EMaterialize; i <= 65535; i++)
			{
				try
				{
					TcpListener tcpListener = new TcpListener(Process_0020Normalize_0020Initialize_002ENormalize, i);
					tcpListener.Start(128);
					Acquire_0020Resolve_0020Normalize_002EAcquire = i;
					return tcpListener;
				}
				catch (SocketException)
				{
				}
			}
		}
		throw new InvalidOperationException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Materialize_0020Normalize_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}
}
