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

internal sealed class Worker_0020Worker_0020Acquire_002EValidate : IDisposable
{
	private sealed record HandshakePacket(int ProtocolVersion, string ServerAddress, int ServerPort, int NextState);

	private static readonly object Materialize_0020Observe_0020Evaluate_002EDispatch = new object();

	private static readonly SemaphoreSlim Resolve_0020Schedule_0020Schedule_002EAcquire = new SemaphoreSlim(256, 256);

	private readonly TcpListener Normalize_0020Evaluate_0020Dispatch_002EDispatch;

	private readonly IPAddress Process_0020Synchronize_0020Resolve_002EResolve;

	private readonly string Evaluate_0020Evaluate_0020Dispatch_002EAcquire;

	private readonly int Synchronize_0020Synchronize_0020Resolve_002EFinalize;

	private readonly string Resolve_0020Finalize_0020Initialize_002EProcess;

	private readonly int Finalize_0020Evaluate_0020Validate_002EValidate;

	private readonly string Worker_0020Worker_0020Materialize_002EFinalize;

	private readonly Func<Task<string?>>? Synchronize_0020Resolve_0020Observe_002EInitialize;

	private readonly Action<string, string>? Materialize_0020Process_0020Process_002ENormalize;

	private readonly CancellationTokenSource Dispatch_0020Initialize_0020Finalize_002EValidate = new CancellationTokenSource();

	private readonly ConcurrentDictionary<TcpClient, byte> Finalize_0020Synchronize_0020Finalize_002EObserve = new ConcurrentDictionary<TcpClient, byte>();

	private int Transform_0020Finalize_0020Process_002EProcess;

	private int Process_0020Acquire_0020Schedule_002EObserve;

	public int Port { get; }

	public string ListenAddress => Process_0020Synchronize_0020Resolve_002EResolve.ToString();

	public Worker_0020Worker_0020Acquire_002EValidate(int preferredPort, string listenAddress, string targetAddress, int targetPort, string handshakeHost, int handshakePort, string motd, Func<Task<string?>>? loginPreflightAsync = null, Action<string, string>? log = null)
	{
		Process_0020Synchronize_0020Resolve_002EResolve = IPAddress.Parse(listenAddress);
		Evaluate_0020Evaluate_0020Dispatch_002EAcquire = targetAddress;
		Synchronize_0020Synchronize_0020Resolve_002EFinalize = targetPort;
		Resolve_0020Finalize_0020Initialize_002EProcess = (string.IsNullOrWhiteSpace(handshakeHost) ? targetAddress : handshakeHost.Trim());
		Finalize_0020Evaluate_0020Validate_002EValidate = ((handshakePort > 0 && handshakePort <= 65535) ? handshakePort : targetPort);
		Worker_0020Worker_0020Materialize_002EFinalize = motd;
		Synchronize_0020Resolve_0020Observe_002EInitialize = loginPreflightAsync;
		Materialize_0020Process_0020Process_002ENormalize = log;
		Normalize_0020Evaluate_0020Dispatch_002EDispatch = Evaluate_0020Observe_0020Observe_002EProcess(Process_0020Synchronize_0020Resolve_002EResolve, preferredPort, out var Schedule_0020Resolve_0020Acquire_002EDispatch);
		Port = Schedule_0020Resolve_0020Acquire_002EDispatch;
	}

	public void Resolve_0020Resolve_0020Validate_002EValidate()
	{
		if (Interlocked.Exchange(ref Transform_0020Finalize_0020Process_002EProcess, 1) == 0)
		{
			string text = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AE.Synchronize_0020Synchronize_0020Synchronize_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 4);
			defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AF.Resolve_0020Transform_0020Synchronize_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
			defaultInterpolatedStringHandler.AppendFormatted(ListenAddress);
			defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B0.Transform_0020Process_0020Materialize_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
			defaultInterpolatedStringHandler.AppendFormatted(Port);
			defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B1.Evaluate_0020Dispatch_0020Validate_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
			defaultInterpolatedStringHandler.AppendFormatted(Evaluate_0020Evaluate_0020Dispatch_002EAcquire);
			defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B2.Materialize_0020Finalize_0020Materialize_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
			defaultInterpolatedStringHandler.AppendFormatted(Synchronize_0020Synchronize_0020Resolve_002EFinalize);
			Normalize_0020Evaluate_0020Worker_002EFinalize(text, defaultInterpolatedStringHandler.ToStringAndClear());
			AcceptLoopAsync();
		}
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref Process_0020Acquire_0020Schedule_002EObserve, 1) != 0)
		{
			return;
		}
		Dispatch_0020Initialize_0020Finalize_002EValidate.Cancel();
		Normalize_0020Evaluate_0020Dispatch_002EDispatch.Stop();
		string text = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B3.Validate_0020Observe_0020Process_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 3);
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B4.Observe_0020Normalize_0020Observe_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendFormatted(ListenAddress);
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B5.Initialize_0020Materialize_0020Validate_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendFormatted(Port);
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B6.Resolve_0020Validate_0020Transform_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendFormatted(Finalize_0020Synchronize_0020Finalize_002EObserve.Count);
		Normalize_0020Evaluate_0020Worker_002EFinalize(text, defaultInterpolatedStringHandler.ToStringAndClear());
		foreach (TcpClient key in Finalize_0020Synchronize_0020Finalize_002EObserve.Keys)
		{
			key.Dispose();
		}
	}

	private async Task AcceptLoopAsync()
	{
		while (!Dispatch_0020Initialize_0020Finalize_002EValidate.IsCancellationRequested)
		{
			try
			{
				TcpClient client = await Normalize_0020Evaluate_0020Dispatch_002EDispatch.AcceptTcpClientAsync(Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
				Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"收到连接：远端={client.Client.RemoteEndPoint}，入口={ListenAddress}:{Port}");
				if (!(await Resolve_0020Schedule_0020Schedule_002EAcquire.WaitAsync(TimeSpan.Zero, Dispatch_0020Initialize_0020Finalize_002EValidate.Token)))
				{
					Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", "拒绝连接：达到全局连接上限，远端=" + client.Client.RemoteEndPoint);
					client.Dispose();
				}
				else
				{
					Finalize_0020Synchronize_0020Finalize_002EObserve.TryAdd(client, 0);
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
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"连接关闭：远端={client.Client.RemoteEndPoint}");
			Finalize_0020Synchronize_0020Finalize_002EObserve.TryRemove(client, out var _);
			client.Dispose();
			Resolve_0020Schedule_0020Schedule_002EAcquire.Release();
		}
	}

	private async Task HandleClientAsync(TcpClient client)
	{
		_ = 7;
		try
		{
			NetworkStream stream = client.GetStream();
			(byte[], byte[])? tuple = await ReadFrameAsync(stream, Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
			if (!tuple.HasValue)
			{
				Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "连接在握手前关闭");
				return;
			}
			HandshakePacket handshake = Initialize_0020Resolve_0020Validate_002EResolve(tuple.Value.Item2);
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"收到握手：协议={handshake.ProtocolVersion}，地址={handshake.ServerAddress}:{handshake.ServerPort}，状态={handshake.NextState}");
			if (handshake.NextState == 1)
			{
				Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "处理服务器状态查询");
				await HandleStatusAsync(stream, handshake.ProtocolVersion, Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
				return;
			}
			string text = ((Synchronize_0020Resolve_0020Observe_002EInitialize != null) ? (await Synchronize_0020Resolve_0020Observe_002EInitialize()) : null);
			string text2 = text;
			if (!string.IsNullOrWhiteSpace(text2))
			{
				Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", "登录预检失败，已向客户端发送断开原因：" + text2);
				await Validate_0020Validate_0020Resolve_002EDispatch(stream, text2);
				return;
			}
			using TcpClient upstream = new TcpClient();
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"正在连接内部代理：{Evaluate_0020Evaluate_0020Dispatch_002EAcquire}:{Synchronize_0020Synchronize_0020Resolve_002EFinalize}");
			await upstream.ConnectAsync(Evaluate_0020Evaluate_0020Dispatch_002EAcquire, Synchronize_0020Synchronize_0020Resolve_002EFinalize, Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
			NetworkStream upstreamStream = upstream.GetStream();
			byte[] array = Acquire_0020Schedule_0020Acquire_002EDispatch(handshake, Resolve_0020Finalize_0020Initialize_002EProcess, Finalize_0020Evaluate_0020Validate_002EValidate);
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"已改写登录握手地址：{handshake.ServerAddress}:{handshake.ServerPort} -> {Resolve_0020Finalize_0020Initialize_002EProcess}:{Finalize_0020Evaluate_0020Validate_002EValidate}");
			await upstreamStream.WriteAsync(array, Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "内部代理连接成功，开始双向转发");
			using CancellationTokenSource forwarding = CancellationTokenSource.CreateLinkedTokenSource(Dispatch_0020Initialize_0020Finalize_002EValidate.Token);
			Task clientToUpstream = ForwardWithTraceAsync(stream, upstreamStream, "客户端->内部代理", forwarding.Token);
			Task upstreamToClient = ForwardWithTraceAsync(upstreamStream, stream, "内部代理->客户端", forwarding.Token);
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", (await Task.WhenAny(clientToUpstream, upstreamToClient) == clientToUpstream) ? "客户端到代理方向结束，停止连接" : "上游代理到客户端方向结束，停止连接");
			forwarding.Cancel();
			await IgnoreConnectionErrorsAsync(clientToUpstream, upstreamToClient);
		}
		catch (OperationCanceledException)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "连接因代理关闭而取消");
		}
		catch (EndOfStreamException)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "对端在数据包读取完成前关闭连接");
		}
		catch (IOException ex3)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", "连接发生 IO 异常：" + ex3.Message);
		}
		catch (SocketException ex4)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", $"连接发生 Socket 异常：{ex4.SocketErrorCode}，{ex4.Message}");
		}
		catch (InvalidDataException ex5)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", "收到无效协议数据：" + ex5.Message);
		}
		catch (ArgumentOutOfRangeException ex6)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", "握手数据超出范围：" + ex6.Message);
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
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"{direction} 转发结束：对端关闭，累计={totalBytes} 字节，块数={chunks}");
		}
		catch (OperationCanceledException)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", $"{direction} 转发取消：累计={totalBytes} 字节，块数={chunks}");
			throw;
		}
		catch (IOException ex2)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", $"{direction} 转发 IO 异常：累计={totalBytes} 字节，块数={chunks}，{ex2.Message}");
			throw;
		}
		catch (SocketException ex3)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("WARN", $"{direction} 转发 Socket 异常：累计={totalBytes} 字节，块数={chunks}，{ex3.SocketErrorCode}，{ex3.Message}");
			throw;
		}
	}

	private async Task HandleStatusAsync(NetworkStream stream, int protocolVersion, CancellationToken cancellation)
	{
		if (!(await ReadFrameAsync(stream, cancellation)).HasValue)
		{
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "状态查询连接在请求前关闭");
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
				text = Worker_0020Worker_0020Materialize_002EFinalize
			}
		});
		await WritePacketAsync(stream, 0, Encoding.UTF8.GetBytes(s));
		Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "已发送服务器状态响应");
		(byte[], byte[])? tuple = await ReadFrameAsync(stream, cancellation);
		if (tuple.HasValue)
		{
			await stream.WriteAsync(tuple.Value.Item1, cancellation);
			Normalize_0020Evaluate_0020Worker_002EFinalize("DEBUG", "已响应状态 Ping");
		}
	}

	private void Normalize_0020Evaluate_0020Worker_002EFinalize(string Acquire_0020Transform_0020Synchronize_002ESynchronize, string Process_0020Dispatch_0020Schedule_002EObserve)
	{
		Materialize_0020Process_0020Process_002ENormalize?.Invoke(Acquire_0020Transform_0020Synchronize_002ESynchronize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B7.Worker_0020Acquire_0020Evaluate_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) + Process_0020Dispatch_0020Schedule_002EObserve);
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

	private static Task Validate_0020Validate_0020Resolve_002EDispatch(NetworkStream Evaluate_0020Normalize_0020Dispatch_002EResolve, string Process_0020Observe_0020Acquire_002EEvaluate)
	{
		string s = JsonSerializer.Serialize(new
		{
			text = Process_0020Observe_0020Acquire_002EEvaluate
		});
		return WritePacketAsync(Evaluate_0020Normalize_0020Dispatch_002EResolve, 0, Encoding.UTF8.GetBytes(s));
	}

	private static HandshakePacket Initialize_0020Resolve_0020Validate_002EResolve(byte[] Materialize_0020Worker_0020Observe_002EAcquire)
	{
		int Observe_0020Worker_0020Evaluate_002EInitialize = 0;
		Synchronize_0020Materialize_0020Process_002EInitialize(Materialize_0020Worker_0020Observe_002EAcquire, ref Observe_0020Worker_0020Evaluate_002EInitialize);
		int protocolVersion = Synchronize_0020Materialize_0020Process_002EInitialize(Materialize_0020Worker_0020Observe_002EAcquire, ref Observe_0020Worker_0020Evaluate_002EInitialize);
		string serverAddress = Process_0020Normalize_0020Worker_002EResolve(Materialize_0020Worker_0020Observe_002EAcquire, ref Observe_0020Worker_0020Evaluate_002EInitialize);
		int serverPort = Evaluate_0020Resolve_0020Acquire_002EEvaluate(Materialize_0020Worker_0020Observe_002EAcquire, ref Observe_0020Worker_0020Evaluate_002EInitialize);
		return new HandshakePacket(protocolVersion, serverAddress, serverPort, Synchronize_0020Materialize_0020Process_002EInitialize(Materialize_0020Worker_0020Observe_002EAcquire, ref Observe_0020Worker_0020Evaluate_002EInitialize));
	}

	private static byte[] Acquire_0020Schedule_0020Acquire_002EDispatch(HandshakePacket Schedule_0020Initialize_0020Synchronize_002ETransform, string Validate_0020Validate_0020Observe_002EFinalize, int Schedule_0020Schedule_0020Finalize_002EMaterialize)
	{
		using MemoryStream memoryStream = new MemoryStream();
		Materialize_0020Initialize_0020Normalize_002EInitialize(memoryStream, 0);
		Materialize_0020Initialize_0020Normalize_002EInitialize(memoryStream, Schedule_0020Initialize_0020Synchronize_002ETransform.ProtocolVersion);
		Dispatch_0020Synchronize_0020Worker_002EDispatch(memoryStream, Validate_0020Validate_0020Observe_002EFinalize);
		Finalize_0020Initialize_0020Observe_002EDispatch(memoryStream, Schedule_0020Schedule_0020Finalize_002EMaterialize);
		Materialize_0020Initialize_0020Normalize_002EInitialize(memoryStream, Schedule_0020Initialize_0020Synchronize_002ETransform.NextState);
		byte[] array = memoryStream.ToArray();
		using MemoryStream memoryStream2 = new MemoryStream();
		Materialize_0020Initialize_0020Normalize_002EInitialize(memoryStream2, array.Length);
		memoryStream2.Write(array);
		return memoryStream2.ToArray();
	}

	private static async Task WritePacketAsync(NetworkStream stream, int packetId, byte[] payload)
	{
		using MemoryStream packet = new MemoryStream();
		Materialize_0020Initialize_0020Normalize_002EInitialize(packet, packetId);
		Materialize_0020Initialize_0020Normalize_002EInitialize(packet, payload.Length);
		packet.Write(payload);
		byte[] array = packet.ToArray();
		using MemoryStream frame = new MemoryStream();
		Materialize_0020Initialize_0020Normalize_002EInitialize(frame, array.Length);
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

	private static int Synchronize_0020Materialize_0020Process_002EInitialize(byte[] Materialize_0020Dispatch_0020Observe_002EWorker, ref int Observe_0020Worker_0020Evaluate_002EInitialize)
	{
		int num = 0;
		for (int i = 0; i < 35; i += 7)
		{
			byte b = Materialize_0020Dispatch_0020Observe_002EWorker[Observe_0020Worker_0020Evaluate_002EInitialize++];
			num |= (b & 0x7F) << i;
			if ((b & 0x80) == 0)
			{
				return num;
			}
		}
		throw new InvalidDataException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B8.Initialize_0020Dispatch_0020Validate_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
	}

	private static string Process_0020Normalize_0020Worker_002EResolve(byte[] Dispatch_0020Materialize_0020Schedule_002EAcquire, ref int Process_0020Finalize_0020Process_002ETransform)
	{
		int num = Synchronize_0020Materialize_0020Process_002EInitialize(Dispatch_0020Materialize_0020Schedule_002EAcquire, ref Process_0020Finalize_0020Process_002ETransform);
		if (num < 0 || Process_0020Finalize_0020Process_002ETransform + num > Dispatch_0020Materialize_0020Schedule_002EAcquire.Length)
		{
			throw new InvalidDataException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000B9.Worker_0020Dispatch_0020Resolve_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		}
		string result = Encoding.UTF8.GetString(Dispatch_0020Materialize_0020Schedule_002EAcquire, Process_0020Finalize_0020Process_002ETransform, num);
		Process_0020Finalize_0020Process_002ETransform += num;
		return result;
	}

	private static int Evaluate_0020Resolve_0020Acquire_002EEvaluate(byte[] Schedule_0020Initialize_0020Evaluate_002ESchedule, ref int Dispatch_0020Process_0020Transform_002EResolve)
	{
		if (Dispatch_0020Process_0020Transform_002EResolve + 2 > Schedule_0020Initialize_0020Evaluate_002ESchedule.Length)
		{
			throw new InvalidDataException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000BA.Initialize_0020Normalize_0020Process_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		}
		return (Schedule_0020Initialize_0020Evaluate_002ESchedule[Dispatch_0020Process_0020Transform_002EResolve++] << 8) | Schedule_0020Initialize_0020Evaluate_002ESchedule[Dispatch_0020Process_0020Transform_002EResolve++];
	}

	private static void Materialize_0020Initialize_0020Normalize_002EInitialize(Stream Resolve_0020Finalize_0020Process_002EInitialize, int Finalize_0020Schedule_0020Schedule_002EEvaluate)
	{
		do
		{
			byte b = (byte)(Finalize_0020Schedule_0020Schedule_002EEvaluate & 0x7F);
			Finalize_0020Schedule_0020Schedule_002EEvaluate >>>= 7;
			if (Finalize_0020Schedule_0020Schedule_002EEvaluate != 0)
			{
				b |= 0x80;
			}
			Resolve_0020Finalize_0020Process_002EInitialize.WriteByte(b);
		}
		while (Finalize_0020Schedule_0020Schedule_002EEvaluate != 0);
	}

	private static void Dispatch_0020Synchronize_0020Worker_002EDispatch(Stream Finalize_0020Resolve_0020Validate_002EFinalize, string Finalize_0020Process_0020Finalize_002EWorker)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(Finalize_0020Process_0020Finalize_002EWorker);
		Materialize_0020Initialize_0020Normalize_002EInitialize(Finalize_0020Resolve_0020Validate_002EFinalize, bytes.Length);
		Finalize_0020Resolve_0020Validate_002EFinalize.Write(bytes);
	}

	private static void Finalize_0020Initialize_0020Observe_002EDispatch(Stream Materialize_0020Validate_0020Process_002EWorker, int Evaluate_0020Resolve_0020Resolve_002ESynchronize)
	{
		if ((Evaluate_0020Resolve_0020Resolve_002ESynchronize < 0 || Evaluate_0020Resolve_0020Resolve_002ESynchronize > 65535) ? true : false)
		{
			throw new ArgumentOutOfRangeException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000BB.Process_0020Materialize_0020Initialize_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		}
		Materialize_0020Validate_0020Process_002EWorker.WriteByte((byte)((Evaluate_0020Resolve_0020Resolve_002ESynchronize >> 8) & 0xFF));
		Materialize_0020Validate_0020Process_002EWorker.WriteByte((byte)(Evaluate_0020Resolve_0020Resolve_002ESynchronize & 0xFF));
	}

	private static TcpListener Evaluate_0020Observe_0020Observe_002EProcess(IPAddress Validate_0020Transform_0020Materialize_002EWorker, int Resolve_0020Initialize_0020Validate_002EObserve, out int Schedule_0020Resolve_0020Acquire_002EDispatch)
	{
		lock (Materialize_0020Observe_0020Evaluate_002EDispatch)
		{
			for (int i = Resolve_0020Initialize_0020Validate_002EObserve; i <= 65535; i++)
			{
				try
				{
					TcpListener tcpListener = new TcpListener(Validate_0020Transform_0020Materialize_002EWorker, i);
					tcpListener.Start(128);
					Schedule_0020Resolve_0020Acquire_002EDispatch = i;
					return tcpListener;
				}
				catch (SocketException)
				{
				}
			}
		}
		throw new InvalidOperationException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000BC.Initialize_0020Initialize_0020Resolve_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
	}
}
