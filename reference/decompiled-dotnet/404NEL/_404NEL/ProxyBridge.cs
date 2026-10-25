using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Mods;
using Codexus.Cipher.Protocol;
using Codexus.Development.SDK.Entities;
using Codexus.Development.SDK.Enums;
using Codexus.Development.SDK.Manager;
using Codexus.Development.SDK.Packet;
using Codexus.Game.Launcher.Utils;
using Codexus.Interceptors;
using Codexus.Interceptors.Packet.Handshake.Client;
using Codexus.Interceptors.Packet.Login.Client;
using Codexus.OpenSDK;
using Codexus.OpenSDK.Entities;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Codexus.OpenSDK.Yggdrasil;
using Codexus.Protocol772Compat;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace _404NEL;

public static class ProxyBridge
{
	private sealed class BridgeLogSink : ILogEventSink
	{
		public void Emit(LogEvent logEvent)
		{
			string text = logEvent.RenderMessage();
			if (!text.StartsWith("Broadcasting operation cancelled", StringComparison.OrdinalIgnoreCase) || (!(logEvent.Exception is OperationCanceledException) && !text.Contains("A task was canceled", StringComparison.OrdinalIgnoreCase)))
			{
				string text2 = logEvent.Level switch
				{
					LogEventLevel.Verbose => "DEBUG", 
					LogEventLevel.Debug => "DEBUG", 
					LogEventLevel.Information => "INFO", 
					LogEventLevel.Warning => "WARN", 
					LogEventLevel.Error => "ERROR", 
					LogEventLevel.Fatal => "ERROR", 
					_ => "INFO", 
				};
				if (logEvent.Exception != null)
				{
					text = text + " | " + logEvent.Exception.GetType().Name + ": " + logEvent.Exception.Message;
				}
				Process_0020Materialize_0020Evaluate_002EMaterialize(text2, "[Interceptor] " + text);
			}
		}
	}

	private const string DefaultCrcSalt = "421C4417360637BA22478129C9AFB8C5";

	private static readonly ConcurrentDictionary<Guid, Interceptor> Transform_0020Acquire_0020Resolve_002EFinalize;

	private static readonly ConcurrentDictionary<Guid, bool> Observe_0020Acquire_0020Observe_002ETransform;

	private static readonly ConcurrentDictionary<Guid, List<Initialize_0020Finalize_0020Process_002EEvaluate>> Validate_0020Synchronize_0020Observe_002ESchedule;

	private static readonly ConcurrentDictionary<Guid, Initialize_0020Finalize_0020Process_002EEvaluate> Finalize_0020Resolve_0020Dispatch_002ETransform;

	private static readonly ConcurrentDictionary<Guid, Validate_0020Normalize_0020Validate_002EAcquire> Transform_0020Observe_0020Resolve_002ESynchronize;

	private static readonly ConcurrentDictionary<string, string> Worker_0020Dispatch_0020Finalize_002EResolve;

	private static readonly ConcurrentDictionary<string, DateTimeOffset> Evaluate_0020Resolve_0020Finalize_002EFinalize;

	private static readonly ConcurrentDictionary<string, SemaphoreSlim> Finalize_0020Observe_0020Schedule_002EFinalize;

	private static readonly ConcurrentQueue<ProxyLogEntry> Finalize_0020Resolve_0020Evaluate_002EWorker;

	private static readonly ConcurrentQueue<ProxyProgressEntry> Resolve_0020Process_0020Observe_002EInitialize;

	private static readonly ConcurrentQueue<ProxyTokenUpdate> Synchronize_0020Evaluate_0020Dispatch_002EEvaluate;

	private static readonly ConcurrentQueue<ProxyReloginRequest> Validate_0020Process_0020Schedule_002EInitialize;

	private static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> Materialize_0020Observe_0020Schedule_002EValidate;

	private static volatile bool Dispatch_0020Evaluate_0020Transform_002EDispatch;

	private static volatile bool Evaluate_0020Observe_0020Materialize_002EEvaluate;

	private static string Evaluate_0020Synchronize_0020Initialize_002EResolve;

	private static readonly WPFLauncher Worker_0020Process_0020Resolve_002EResolve;

	private static readonly HttpClient Dispatch_0020Finalize_0020Materialize_002EEvaluate;

	private static readonly YggdrasilAuthServer[] Materialize_0020Acquire_0020Validate_002EDispatch;

	static ProxyBridge()
	{
		Transform_0020Acquire_0020Resolve_002EFinalize = new ConcurrentDictionary<Guid, Interceptor>();
		Observe_0020Acquire_0020Observe_002ETransform = new ConcurrentDictionary<Guid, bool>();
		Validate_0020Synchronize_0020Observe_002ESchedule = new ConcurrentDictionary<Guid, List<Initialize_0020Finalize_0020Process_002EEvaluate>>();
		Finalize_0020Resolve_0020Dispatch_002ETransform = new ConcurrentDictionary<Guid, Initialize_0020Finalize_0020Process_002EEvaluate>();
		Transform_0020Observe_0020Resolve_002ESynchronize = new ConcurrentDictionary<Guid, Validate_0020Normalize_0020Validate_002EAcquire>();
		Worker_0020Dispatch_0020Finalize_002EResolve = new ConcurrentDictionary<string, string>();
		Evaluate_0020Resolve_0020Finalize_002EFinalize = new ConcurrentDictionary<string, DateTimeOffset>();
		Finalize_0020Observe_0020Schedule_002EFinalize = new ConcurrentDictionary<string, SemaphoreSlim>();
		Finalize_0020Resolve_0020Evaluate_002EWorker = new ConcurrentQueue<ProxyLogEntry>();
		Resolve_0020Process_0020Observe_002EInitialize = new ConcurrentQueue<ProxyProgressEntry>();
		Synchronize_0020Evaluate_0020Dispatch_002EEvaluate = new ConcurrentQueue<ProxyTokenUpdate>();
		Validate_0020Process_0020Schedule_002EInitialize = new ConcurrentQueue<ProxyReloginRequest>();
		Materialize_0020Observe_0020Schedule_002EValidate = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
		Dispatch_0020Evaluate_0020Transform_002EDispatch = true;
		Evaluate_0020Observe_0020Materialize_002EEvaluate = true;
		Evaluate_0020Synchronize_0020Initialize_002EResolve = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Transform_0020Observe_0020Finalize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		Worker_0020Process_0020Resolve_002EResolve = new WPFLauncher();
		Dispatch_0020Finalize_0020Materialize_002EEvaluate = new HttpClient();
		Materialize_0020Acquire_0020Validate_002EDispatch = new YggdrasilAuthServer[4]
		{
			new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Resolve_0020Synchronize_0020Worker_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 10801, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Finalize_0020Evaluate_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Evaluate_0020Dispatch_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11001, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Finalize_0020Transform_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Validate_0020Initialize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11201, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Finalize_0020Dispatch_0020Synchronize_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Finalize_0020Observe_0020Finalize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11401, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Validate_0020Materialize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform))
		};
		Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Sink(new BridgeLogSink()).CreateLogger();
		PacketManager.Instance.RegisterPacketFromAssembly(Protocol772Registration.PacketType.Assembly);
		int[] array = new int[2] { 772, 773 };
		foreach (int version in array)
		{
			PacketManager.Instance.RegisterPacket(new RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ServerBound, 1, (EnumProtocolVersion)version), typeof(CPacketEncryptionResponse));
			PacketManager.Instance.RegisterPacket(new RegisterPacket(EnumConnectionState.Handshaking, EnumPacketDirection.ServerBound, 0, (EnumProtocolVersion)version), typeof(CHandshake));
		}
		Process_0020Materialize_0020Evaluate_002EMaterialize(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Normalize_0020Materialize_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Observe_0020Synchronize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}

	public static async Task UpdateTokenAsync(string entityId, string accessToken)
	{
		await StoreTokenAsync(entityId, accessToken, markRefresh: false);
	}

	public static async Task UpdateRefreshedTokenAsync(string entityId, string accessToken)
	{
		await StoreTokenAsync(entityId, accessToken, markRefresh: true);
	}

	private static async Task StoreTokenAsync(string entityId, string accessToken, bool markRefresh)
	{
		if (!string.IsNullOrWhiteSpace(entityId) && !string.IsNullOrWhiteSpace(accessToken))
		{
			Worker_0020Dispatch_0020Finalize_002EResolve[entityId] = accessToken;
			if (markRefresh)
			{
				Evaluate_0020Resolve_0020Finalize_002EFinalize[entityId] = DateTimeOffset.UtcNow;
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", "[Token] 已同步：UID=" + entityId + "，来源=" + (markRefresh ? "AuthenticationUpdate" : "登录会话"));
			await RefreshSessionsForUserAsync(entityId, accessToken);
			if (Materialize_0020Observe_0020Schedule_002EValidate.TryGetValue(entityId, out TaskCompletionSource<string> value))
			{
				value.TrySetResult(accessToken);
			}
		}
	}

	public static ProxyLogEntry[] DrainLogs()
	{
		List<ProxyLogEntry> list = new List<ProxyLogEntry>();
		ProxyLogEntry result;
		while (Finalize_0020Resolve_0020Evaluate_002EWorker.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyProgressEntry[] DrainProgress()
	{
		List<ProxyProgressEntry> list = new List<ProxyProgressEntry>();
		ProxyProgressEntry result;
		while (Resolve_0020Process_0020Observe_002EInitialize.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyTokenUpdate[] DrainTokenUpdates()
	{
		List<ProxyTokenUpdate> list = new List<ProxyTokenUpdate>();
		ProxyTokenUpdate result;
		while (Synchronize_0020Evaluate_0020Dispatch_002EEvaluate.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyReloginRequest[] DrainReloginRequests()
	{
		List<ProxyReloginRequest> list = new List<ProxyReloginRequest>();
		ProxyReloginRequest result;
		while (Validate_0020Process_0020Schedule_002EInitialize.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static void SetProgramAccessState(bool accessGranted, string reason)
	{
		Dispatch_0020Evaluate_0020Transform_002EDispatch = accessGranted;
		Evaluate_0020Synchronize_0020Initialize_002EResolve = (string.IsNullOrWhiteSpace(reason) ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Evaluate_0020Worker_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) : reason.Trim());
		Process_0020Materialize_0020Evaluate_002EMaterialize(accessGranted ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Materialize_0020Synchronize_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) : Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Dispatch_0020Process_0020Normalize_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), accessGranted ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Materialize_0020Acquire_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) : (Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Resolve_0020Worker_0020Normalize_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) + Evaluate_0020Synchronize_0020Initialize_002EResolve));
	}

	public static void SetLanBroadcastEnabled(bool enabled)
	{
		Evaluate_0020Observe_0020Materialize_002EEvaluate = enabled;
		int num = 0;
		foreach (KeyValuePair<Guid, Interceptor> item in Transform_0020Acquire_0020Resolve_002EFinalize)
		{
			if (Observe_0020Acquire_0020Observe_002ETransform.GetOrAdd(item.Key, value: true) != enabled)
			{
				if (enabled)
				{
					Process_0020Transform_0020Evaluate_002EDispatch(item.Value);
				}
				else
				{
					Schedule_0020Synchronize_0020Dispatch_002EFinalize(item.Value);
				}
				Observe_0020Acquire_0020Observe_002ETransform[item.Key] = enabled;
				num++;
			}
		}
		string text = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Observe_0020Resolve_0020Process_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Normalize_0020Process_0020Materialize_002EWorker, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendFormatted(enabled ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Initialize_0020Materialize_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) : Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Validate_0020Acquire_002EWorker, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Schedule_0020Resolve_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Evaluate_0020Process_002ENormalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		defaultInterpolatedStringHandler.AppendFormatted(num);
		defaultInterpolatedStringHandler.AppendLiteral(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Worker_0020Evaluate_0020Resolve_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		Process_0020Materialize_0020Evaluate_002EMaterialize(text, defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private static async Task<string?> RequestAccountReloginAsync(string entityId, string reason)
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
		TaskCompletionSource<string> waiter = Materialize_0020Observe_0020Schedule_002EValidate.GetOrAdd(entityId, taskCompletionSource);
		if (waiter == taskCompletionSource)
		{
			Validate_0020Process_0020Schedule_002EInitialize.Enqueue(new ProxyReloginRequest(entityId, reason));
			Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", $"[Token] 已请求客户端重新登录账号并保持游戏连接：UID={entityId}，原因={reason}");
		}
		else
		{
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", "[Token] 复用进行中的账号重新登录：UID=" + entityId);
		}
		try
		{
			string result = await waiter.Task.WaitAsync(TimeSpan.FromSeconds(90L));
			Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", "[Token] 客户端重新登录完成，继续当前游戏连接：UID=" + entityId);
			return result;
		}
		catch (TimeoutException)
		{
			Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", "[Token] 等待客户端重新登录超时：UID=" + entityId);
			return null;
		}
		finally
		{
			if (Materialize_0020Observe_0020Schedule_002EValidate.TryGetValue(entityId, out TaskCompletionSource<string> value) && value == waiter)
			{
				Materialize_0020Observe_0020Schedule_002EValidate.TryRemove(entityId, out TaskCompletionSource<string> _);
			}
		}
	}

	private static string Schedule_0020Transform_0020Initialize_002EDispatch(string Acquire_0020Acquire_0020Synchronize_002ENormalize, string Dispatch_0020Acquire_0020Acquire_002EResolve)
	{
		if (!Worker_0020Dispatch_0020Finalize_002EResolve.TryGetValue(Acquire_0020Acquire_0020Synchronize_002ENormalize, out string value) || string.IsNullOrWhiteSpace(value))
		{
			return Dispatch_0020Acquire_0020Acquire_002EResolve;
		}
		return value;
	}

	private static bool Process_0020Materialize_0020Finalize_002EValidate(string Finalize_0020Dispatch_0020Process_002ETransform)
	{
		if (Evaluate_0020Resolve_0020Finalize_002EFinalize.TryGetValue(Finalize_0020Dispatch_0020Process_002ETransform, out var value))
		{
			return DateTimeOffset.UtcNow - value >= TimeSpan.FromMinutes(5L);
		}
		return true;
	}

	public static async Task<ProxyStartResult> StartAsync(string entityId, string accessToken, string serverId, string serverName, string role, string gameId, bool socksEnabled = false, string socksAddress = "", int socksPort = 0, string socksUsername = "", string socksPassword = "", string crcSalt = "", bool skipResourcePreparation = false)
	{
		if (string.IsNullOrWhiteSpace(entityId) || string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(serverId) || string.IsNullOrWhiteSpace(role))
		{
			return new ProxyStartResult(Success: false, "", "", "", "Missing required parameters");
		}
		Interceptor interceptor = null;
		Initialize_0020Finalize_0020Process_002EEvaluate upstreamProxy = null;
		List<Initialize_0020Finalize_0020Process_002EEvaluate> statusProxies = new List<Initialize_0020Finalize_0020Process_002EEvaluate>();
		try
		{
			Worker_0020Dispatch_0020Finalize_002EResolve[entityId] = accessToken;
			string currentToken = Schedule_0020Transform_0020Initialize_002EDispatch(entityId, accessToken);
			Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", $"[Proxy] 开始启动：UID={entityId}，服务器={serverId}，角色={role}");
			WPFLauncher wPFLauncher = Worker_0020Process_0020Resolve_002EResolve;
			Entities<EntityGameCharacter> entities = wPFLauncher.QueryNetGameCharacters(entityId, currentToken, serverId);
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Proxy] 角色校验响应：服务器={serverId}，角色数={entities.Data.Length}");
			if (entities.Data.All((EntityGameCharacter item) => item.Name != role))
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "启动代理失败：角色不存在 " + role);
				return new ProxyStartResult(Success: false, "", "", "", "Selected role was not found");
			}
			Entity<EntityQueryNetGameDetailItem> entity = wPFLauncher.QueryNetGameDetailById(entityId, currentToken, serverId);
			Entity<EntityNetGameServerAddress> address = wPFLauncher.GetNetGameServerAddress(entityId, currentToken, serverId);
			EntityMcVersion version = entity.Data.McVersionList[0];
			string authVersionName = Dispatch_0020Observe_0020Normalize_002EProcess.Transform_0020Worker_0020Initialize_002EObserve(version.Name);
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Proxy] 服务器信息解析完成：版本={version.Name}，认证版本={authVersionName}，上游={address.Data.Ip}:{address.Data.Port}");
			string text = authVersionName;
			EnumGameVersion enumGameVersion;
			if (text == "1.21.8")
			{
				enumGameVersion = (EnumGameVersion)1021008u;
			}
			else
			{
				enumGameVersion = ((!(text == "1.21.10")) ? GameVersionUtil.GetEnumFromGameVersion(authVersionName) : ((EnumGameVersion)1021010u));
			}
			EnumGameVersion gameVersion = enumGameVersion;
			EntityModsList entityModsList = (skipResourcePreparation ? Acquire_0020Process_0020Acquire_002EAcquire.Materialize_0020Resolve_0020Normalize_002EWorker(serverId, Transform_0020Resolve_0020Dispatch_002EFinalize) : null);
			if (entityModsList == null)
			{
				if (skipResourcePreparation)
				{
					Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Resource] 服务器 " + serverId + " 没有可复用的批次资源，回退到完整准备");
				}
				entityModsList = await Acquire_0020Process_0020Acquire_002EAcquire.InstallGameMods(entityId, currentToken, gameVersion, wPFLauncher, serverId, isRental: false, Transform_0020Resolve_0020Dispatch_002EFinalize);
				Acquire_0020Process_0020Acquire_002EAcquire.Dispatch_0020Observe_0020Process_002EAcquire(serverId, entityModsList);
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", skipResourcePreparation ? ("[Resource] 服务器 " + serverId + " 已跳过下载与校验") : ("[Resource] 服务器 " + serverId + " 已完成完整资源准备"));
			string mods = JsonSerializer.Serialize(entityModsList);
			Dispatch_0020Observe_0020Normalize_002EProcess.Md5Pair pair = Dispatch_0020Observe_0020Normalize_002EProcess.Synchronize_0020Acquire_0020Evaluate_002EInitialize(authVersionName);
			X19 x = new X19();
			YggdrasilData yggdrasilData = new YggdrasilData
			{
				LauncherVersion = x.GameVersion,
				Channel = "netease",
				CrcSalt = (string.IsNullOrWhiteSpace(crcSalt) ? "421C4417360637BA22478129C9AFB8C5" : crcSalt.Trim().ToUpperInvariant())
			};
			EntitySocks5 entitySocks = new EntitySocks5
			{
				Enabled = socksEnabled,
				Address = socksAddress,
				Port = socksPort,
				Username = socksUsername,
				Password = socksPassword
			};
			string displayMotd = "§b404 §f" + address.Data.Ip + " -> " + role;
			string text2 = address.Data.Ip;
			int port = address.Data.Port;
			if (serverId == "4658261307332877912")
			{
				Func<string, int, CancellationToken, Task<TcpClient>> upstreamConnector = (socksEnabled ? ((Func<string, int, CancellationToken, Task<TcpClient>>)((string host, int port2, CancellationToken cancellation) => Acquire_0020Resolve_0020Validate_002EEvaluate.ConnectThroughSocksAsync(host, port2, socksAddress, socksPort, socksUsername, socksPassword, cancellation))) : null);
				upstreamProxy = new Initialize_0020Finalize_0020Process_002EEvaluate(31000, IPAddress.Loopback.ToString(), text2, port, text2, port, displayMotd, null, Process_0020Materialize_0020Evaluate_002EMaterialize, forwardToUpstream: true, upstreamConnector);
				upstreamProxy.Acquire_0020Resolve_0020Evaluate_002ENormalize();
				text2 = upstreamProxy.ListenAddress;
				port = upstreamProxy.Port;
				entitySocks.Enabled = false;
			}
			int localPort = Worker_0020Evaluate_0020Finalize_002EEvaluate(30000);
			interceptor = Interceptor.CreateInterceptor(entitySocks, mods, serverId, displayMotd, version.Name, text2, port, role, entityId, currentToken, (string certification) =>
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", $"[Yggdrasil] 收到认证请求：UID={entityId}，服务器={serverId}，版本={version.Name}，认证版本={authVersionName}");
				Log.Information("Proxy received server certification");
				ManualResetEventSlim authorization = new ManualResetEventSlim(initialState: false);
				try
				{
					Exception authorizationError = null;
					Task.Run(async () =>
					{
						_ = 2;
						try
						{
							string token = Schedule_0020Transform_0020Initialize_002EDispatch(entityId, accessToken);
							Exception lastError = await AuthenticateAsync(token);
							if (lastError != null)
							{
								Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Yggdrasil] 首轮认证失败，检查 Token 刷新冷却");
								string text3 = await TryRefreshTokenIfDueAsync(entityId, "Yggdrasil 认证失败");
								if (!string.IsNullOrWhiteSpace(text3))
								{
									token = text3;
									Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", "[Yggdrasil] Token 已刷新，重新提交认证");
									lastError = await AuthenticateAsync(token);
								}
							}
							if (lastError != null)
							{
								throw lastError;
							}
							Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", "[Yggdrasil] 认证成功，可以继续登录服务器");
							Log.Information("Proxy Yggdrasil authentication succeeded");
						}
						catch (Exception ex2)
						{
							Exception ex3 = (authorizationError = ex2);
							Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", "Yggdrasil 认证失败：" + ex3.Message);
							Log.Error(ex3, "Proxy Yggdrasil authentication failed");
						}
						finally
						{
							authorization.Set();
						}
					});
					if (!authorization.Wait(TimeSpan.FromSeconds(35L)))
					{
						Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", "Yggdrasil 认证等待超过 35 秒");
						throw new TimeoutException("Yggdrasil authentication timed out after 35 seconds");
					}
					if (authorizationError != null)
					{
						throw authorizationError;
					}
				}
				finally
				{
					if (authorization != null)
					{
						((IDisposable)authorization).Dispose();
					}
				}
				async Task<Exception?> AuthenticateAsync(string token)
				{
					GameProfile profile = CreateProfile(token);
					Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Yggdrasil] 认证参数已构建：UID={entityId}，服务器={serverId}，版本={version.Name}，认证版本={authVersionName}，ModsJsonBytes={mods.Length}（敏感字段已隐藏）");
					Exception lastError = null;
					foreach (YggdrasilAuthServer authServer in await GetYggdrasilAuthServersAsync())
					{
						try
						{
							Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Yggdrasil] 正在连接认证节点 {authServer.IP}:{authServer.Port}" + (string.IsNullOrWhiteSpace(authServer.ServerType) ? "" : ("，类型=" + authServer.ServerType)));
							Result result = await new StandardYggdrasil(yggdrasilData, authServer.IP, authServer.Port).JoinServerAsync(profile, certification).WaitAsync(TimeSpan.FromSeconds(12L));
							Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Yggdrasil] 节点响应：{authServer.IP}:{authServer.Port}，成功={result.IsSuccess}，错误={result.Error ?? "无"}");
							if (!result.IsSuccess)
							{
								throw new InvalidOperationException(result.Error ?? "Yggdrasil authentication failed");
							}
							return null;
						}
						catch (Exception ex2)
						{
							lastError = ex2;
							Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", $"[Yggdrasil] 认证节点 {authServer.IP}:{authServer.Port} 失败：{ex2.Message}");
						}
					}
					return lastError;
				}
			}, "127.0.0.1", localPort);
			Observe_0020Acquire_0020Observe_002ETransform[interceptor.Identifier] = true;
			if (!Evaluate_0020Observe_0020Materialize_002EEvaluate)
			{
				await StopLanBroadcastWhenStartedAsync(interceptor);
				Observe_0020Acquire_0020Observe_002ETransform[interceptor.Identifier] = false;
			}
			string listenAddress = Transform_0020Worker_0020Evaluate_002EDispatch();
			Initialize_0020Finalize_0020Process_002EEvaluate obj = new Initialize_0020Finalize_0020Process_002EEvaluate(25565, listenAddress, interceptor.LocalAddress, interceptor.LocalPort, address.Data.Ip, address.Data.Port, displayMotd, () => ValidateLoginPreflightAsync(entityId, serverId, role), Process_0020Materialize_0020Evaluate_002EMaterialize);
			obj.Acquire_0020Resolve_0020Evaluate_002ENormalize();
			statusProxies.Add(obj);
			Initialize_0020Finalize_0020Process_002EEvaluate obj2 = new Initialize_0020Finalize_0020Process_002EEvaluate(25565, IPAddress.Loopback.ToString(), interceptor.LocalAddress, interceptor.LocalPort, address.Data.Ip, address.Data.Port, displayMotd, () => ValidateLoginPreflightAsync(entityId, serverId, role), Process_0020Materialize_0020Evaluate_002EMaterialize);
			obj2.Acquire_0020Resolve_0020Evaluate_002ENormalize();
			statusProxies.Add(obj2);
			await InterConn.GameStart(entityId, currentToken, string.IsNullOrWhiteSpace(gameId) ? serverId : gameId);
			await X19.InterconnectionApi.GameStartAsync(entityId, currentToken, serverId);
			if (Observe_0020Acquire_0020Observe_002ETransform.GetOrAdd(interceptor.Identifier, value: true) != Evaluate_0020Observe_0020Materialize_002EEvaluate)
			{
				if (Evaluate_0020Observe_0020Materialize_002EEvaluate)
				{
					Process_0020Transform_0020Evaluate_002EDispatch(interceptor);
				}
				else
				{
					Schedule_0020Synchronize_0020Dispatch_002EFinalize(interceptor);
				}
				Observe_0020Acquire_0020Observe_002ETransform[interceptor.Identifier] = Evaluate_0020Observe_0020Materialize_002EEvaluate;
			}
			Transform_0020Acquire_0020Resolve_002EFinalize[interceptor.Identifier] = interceptor;
			Validate_0020Synchronize_0020Observe_002ESchedule[interceptor.Identifier] = statusProxies;
			Transform_0020Observe_0020Resolve_002ESynchronize[interceptor.Identifier] = new Validate_0020Normalize_0020Validate_002EAcquire(entityId, serverId, string.IsNullOrWhiteSpace(gameId) ? serverId : gameId);
			Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", "代理启动成功：" + serverName + " / " + role);
			if (upstreamProxy != null)
			{
				Finalize_0020Resolve_0020Dispatch_002ETransform[interceptor.Identifier] = upstreamProxy;
				Acquire_0020Resolve_0020Validate_002EEvaluate.Worker_0020Observe_0020Acquire_002EResolve(upstreamProxy, "netease1.yszscraft.cn", 25019, Process_0020Materialize_0020Evaluate_002EMaterialize);
			}
			return new ProxyStartResult(Success: true, string.Join(" / ", statusProxies.Select((Initialize_0020Finalize_0020Process_002EEvaluate proxy) => $"{proxy.ListenAddress}:{proxy.Port}")), interceptor.Identifier.ToString(), $"{address.Data.Ip}:{address.Data.Port}", "");
			GameProfile CreateProfile(string token)
			{
				return new GameProfile
				{
					GameId = serverId,
					GameVersion = authVersionName,
					BootstrapMd5 = pair.BootstrapMd5,
					DatFileMd5 = pair.DatFileMd5,
					Mods = JsonSerializer.Deserialize<ModList>(mods),
					User = new UserProfile
					{
						UserId = int.Parse(entityId),
						UserToken = token
					}
				};
			}
		}
		catch (Exception ex)
		{
			upstreamProxy?.Dispose();
			Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", "代理启动失败：" + ex.Message);
			foreach (Initialize_0020Finalize_0020Process_002EEvaluate item in statusProxies)
			{
				item.Dispose();
			}
			if (interceptor != null)
			{
				Observe_0020Acquire_0020Observe_002ETransform.TryRemove(interceptor.Identifier, out var _);
				interceptor.ShutdownAsync();
			}
			return new ProxyStartResult(Success: false, "", "", "", ex.Message);
		}
	}

	private static void Transform_0020Resolve_0020Dispatch_002EFinalize(Schedule_0020Materialize_0020Dispatch_002ENormalize Evaluate_0020Evaluate_0020Process_002ESynchronize)
	{
		Resolve_0020Process_0020Observe_002EInitialize.Enqueue(new ProxyProgressEntry(DateTime.Now.ToString(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Worker_0020Process_0020Dispatch_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Evaluate_0020Process_002ESynchronize.Stage, Evaluate_0020Evaluate_0020Process_002ESynchronize.ResourceName, Evaluate_0020Evaluate_0020Process_002ESynchronize.DownloadedBytes, Evaluate_0020Evaluate_0020Process_002ESynchronize.TotalBytes, Evaluate_0020Evaluate_0020Process_002ESynchronize.Percent, Evaluate_0020Evaluate_0020Process_002ESynchronize.SpeedBytesPerSecond, Evaluate_0020Evaluate_0020Process_002ESynchronize.Message));
	}

	private static async Task<string?> TryRefreshTokenIfDueAsync(string entityId, string reason)
	{
		SemaphoreSlim refreshLock = Finalize_0020Observe_0020Schedule_002EFinalize.GetOrAdd(entityId, (string _) => new SemaphoreSlim(1, 1));
		await refreshLock.WaitAsync();
		try
		{
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;
			if (Evaluate_0020Resolve_0020Finalize_002EFinalize.TryGetValue(entityId, out var value))
			{
				TimeSpan timeSpan = TimeSpan.FromMinutes(5L) - (utcNow - value);
				if (timeSpan > TimeSpan.Zero)
				{
					Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Token] 刷新被冷却阻止：UID={entityId}，场景={reason}，剩余={timeSpan.TotalSeconds:F0}秒");
					return null;
				}
			}
			string text = Schedule_0020Transform_0020Initialize_002EDispatch(entityId, "");
			if (string.IsNullOrWhiteSpace(text))
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Token] 无法执行认证失败刷新：UID=" + entityId + "，当前 Token 为空");
				return null;
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", $"[Token] 调用 AuthenticationUpdateAsync：UID={entityId}，场景={reason}");
			string refreshedToken = (await Worker_0020Process_0020Resolve_002EResolve.AuthenticationUpdateAsync(entityId, text))?.Token?.ToString();
			if (string.IsNullOrWhiteSpace(refreshedToken))
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Token] AuthenticationUpdateAsync 未返回新 Token：UID=" + entityId);
				return null;
			}
			Worker_0020Dispatch_0020Finalize_002EResolve[entityId] = refreshedToken;
			Evaluate_0020Resolve_0020Finalize_002EFinalize[entityId] = DateTimeOffset.UtcNow;
			Synchronize_0020Evaluate_0020Dispatch_002EEvaluate.Enqueue(new ProxyTokenUpdate(entityId, refreshedToken));
			await RefreshSessionsForUserAsync(entityId, refreshedToken);
			Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", "[Token] 刷新成功并已同步代理：UID=" + entityId + "，场景=" + reason);
			return refreshedToken;
		}
		catch (Exception ex)
		{
			Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", $"[Token] 刷新异常：UID={entityId}，场景={reason}，{ex.Message}");
			return null;
		}
		finally
		{
			refreshLock.Release();
		}
	}

	private static async Task RefreshSessionsForUserAsync(string entityId, string accessToken)
	{
		List<Validate_0020Normalize_0020Validate_002EAcquire> list = Transform_0020Observe_0020Resolve_002ESynchronize.Values.Where((Validate_0020Normalize_0020Validate_002EAcquire obj) => obj.EntityId == entityId).Distinct().ToList();
		foreach (Validate_0020Normalize_0020Validate_002EAcquire session in list)
		{
			try
			{
				await InterConn.GameStart(session.EntityId, accessToken, session.GameId);
				await X19.InterconnectionApi.GameStartAsync(session.EntityId, accessToken, session.ServerId);
				Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", "已同步运行中代理会话 Token：" + session.ServerId);
			}
			catch (Exception ex)
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "同步运行中代理会话 Token 失败：" + session.ServerId + "，" + ex.Message);
			}
		}
	}

	private static string Transform_0020Worker_0020Evaluate_002EDispatch()
	{
		try
		{
			using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Connect(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Dispatch_0020Initialize_0020Dispatch_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 53);
			if (socket.LocalEndPoint is IPEndPoint iPEndPoint && !IPAddress.IsLoopback(iPEndPoint.Address) && iPEndPoint.Address.AddressFamily == AddressFamily.InterNetwork)
			{
				return iPEndPoint.Address.ToString();
			}
		}
		catch
		{
		}
		return IPAddress.Loopback.ToString();
	}

	private static async Task<IReadOnlyList<YggdrasilAuthServer>> GetYggdrasilAuthServersAsync()
	{
		try
		{
			using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(8L));
			YggdrasilAuthServer[] array = JsonSerializer.Deserialize<YggdrasilAuthServer[]>(await Dispatch_0020Finalize_0020Materialize_002EEvaluate.GetStringAsync("https://x19.update.netease.com/authserver.list", cts.Token), new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			})?.Where((YggdrasilAuthServer server) => !string.IsNullOrWhiteSpace(server.IP) && server.Port > 0).DistinctBy((YggdrasilAuthServer server) => $"{server.IP}:{server.Port}").ToArray();
			if (array != null && array.Length > 0)
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", "[Yggdrasil] 已从官方列表获取认证节点：" + string.Join(", ", array.Select((YggdrasilAuthServer server) => $"{server.IP}:{server.Port}" + (string.IsNullOrWhiteSpace(server.ServerType) ? "" : ("(" + server.ServerType + ")")))));
				return array;
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Yggdrasil] 官方认证节点列表为空，使用兜底节点");
		}
		catch (Exception ex)
		{
			Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Yggdrasil] 获取官方认证节点失败，使用兜底节点：" + ex.Message);
		}
		return Materialize_0020Acquire_0020Validate_002EDispatch;
	}

	private static async Task<string?> ValidateLoginPreflightAsync(string entityId, string serverId, string role)
	{
		try
		{
			if (!Dispatch_0020Evaluate_0020Transform_002EDispatch)
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", $"[Proxy.Preflight] 程序订阅无效，拒绝新连接：UID={entityId}，服务器={serverId}，角色={role}");
				return Evaluate_0020Synchronize_0020Initialize_002EResolve;
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Proxy.Preflight] 客户端准备登录：UID={entityId}，服务器={serverId}，角色={role}");
			bool refreshWasDue = Process_0020Materialize_0020Finalize_002EValidate(entityId);
			string text = await TryRefreshTokenIfDueAsync(entityId, "客户端连接代理");
			if (refreshWasDue && string.IsNullOrWhiteSpace(text))
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", "[Proxy.Preflight] Token 刷新失败：UID=" + entityId + "，保持连接并尝试重新登录账号");
				text = await RequestAccountReloginAsync(entityId, "自动刷新 Token 失败");
				if (string.IsNullOrWhiteSpace(text))
				{
					return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
				}
			}
			string currentToken = text ?? Schedule_0020Transform_0020Initialize_002EDispatch(entityId, "");
			if (string.IsNullOrWhiteSpace(currentToken))
			{
				currentToken = await RequestAccountReloginAsync(entityId, "代理没有可用的登录 Token");
				if (string.IsNullOrWhiteSpace(currentToken))
				{
					return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
				}
			}
			Entities<EntityGameCharacter> roles = await Task.Run(() => Worker_0020Process_0020Resolve_002EResolve.QueryNetGameCharacters(entityId, currentToken, serverId));
			if (roles?.Data == null)
			{
				return "无法验证账号状态，请稍后重试。";
			}
			if (roles.Data.All((EntityGameCharacter item) => item.Name != role))
			{
				Process_0020Materialize_0020Evaluate_002EMaterialize("WARN", $"[Proxy.Preflight] 角色查询未返回目标角色：UID={entityId}，服务器={serverId}，返回数量={roles.Data.Length}，目标角色={role}");
				if (roles.Data.Length == 0)
				{
					string reloginToken = await RequestAccountReloginAsync(entityId, "服务器未返回角色列表");
					if (!string.IsNullOrWhiteSpace(reloginToken))
					{
						roles = await Task.Run(() => Worker_0020Process_0020Resolve_002EResolve.QueryNetGameCharacters(entityId, reloginToken, serverId));
						Entities<EntityGameCharacter> entities = roles;
						if (entities != null && entities.Data?.Any((EntityGameCharacter item) => item.Name == role) == true)
						{
							Process_0020Materialize_0020Evaluate_002EMaterialize("INFO", $"[Proxy.Preflight] 重新登录后角色验证通过：UID={entityId}，服务器={serverId}，角色={role}");
							return null;
						}
					}
				}
				return (roles.Data.Length == 0) ? "账号自动重新登录后仍未返回角色列表。" : ("角色不存在或已失效：" + role);
			}
			Process_0020Materialize_0020Evaluate_002EMaterialize("DEBUG", $"[Proxy.Preflight] 登录预检通过：UID={entityId}，服务器={serverId}，角色={role}");
			return null;
		}
		catch (Exception ex)
		{
			string message = ex.Message;
			if (message.Contains("token", StringComparison.OrdinalIgnoreCase) || message.Contains("登录", StringComparison.OrdinalIgnoreCase) || message.Contains("过期", StringComparison.OrdinalIgnoreCase))
			{
				string reloginToken2 = await RequestAccountReloginAsync(entityId, "登录预检检测到账号会话过期");
				if (!string.IsNullOrWhiteSpace(reloginToken2))
				{
					try
					{
						Entities<EntityGameCharacter> entities2 = await Task.Run(() => Worker_0020Process_0020Resolve_002EResolve.QueryNetGameCharacters(entityId, reloginToken2, serverId));
						if (entities2 != null && entities2.Data?.Any((EntityGameCharacter item) => item.Name == role) == true)
						{
							return null;
						}
					}
					catch (Exception ex2)
					{
						Process_0020Materialize_0020Evaluate_002EMaterialize("ERROR", $"[Proxy.Preflight] 重新登录后的验证仍失败：UID={entityId}，{ex2.Message}");
					}
				}
				return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
			}
			return "进入服务器前验证失败：" + message;
		}
	}

	public static Task<bool> StopAsync(string identifier)
	{
		if (!Guid.TryParse(identifier, out var result) || !Transform_0020Acquire_0020Resolve_002EFinalize.TryRemove(result, out Interceptor value))
		{
			return Task.FromResult(result: false);
		}
		if (Validate_0020Synchronize_0020Observe_002ESchedule.TryRemove(result, out List<Initialize_0020Finalize_0020Process_002EEvaluate> value2))
		{
			foreach (Initialize_0020Finalize_0020Process_002EEvaluate item in value2)
			{
				item.Dispose();
			}
		}
		Transform_0020Observe_0020Resolve_002ESynchronize.TryRemove(result, out Validate_0020Normalize_0020Validate_002EAcquire _);
		if (Finalize_0020Resolve_0020Dispatch_002ETransform.TryRemove(result, out Initialize_0020Finalize_0020Process_002EEvaluate value4))
		{
			value4.Dispose();
		}
		Observe_0020Acquire_0020Observe_002ETransform.TryRemove(result, out var _);
		value.ShutdownAsync();
		Process_0020Materialize_0020Evaluate_002EMaterialize(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Schedule_0020Process_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Resolve_0020Synchronize_0020Finalize_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) + identifier);
		return Task.FromResult(result: true);
	}

	private static void Process_0020Materialize_0020Evaluate_002EMaterialize(string Validate_0020Materialize_0020Acquire_002ESynchronize, string Resolve_0020Finalize_0020Normalize_002EAcquire)
	{
		string message = Evaluate_0020Resolve_0020Worker_002EAcquire(Resolve_0020Finalize_0020Normalize_002EAcquire);
		Finalize_0020Resolve_0020Evaluate_002EWorker.Enqueue(new ProxyLogEntry(DateTime.Now.ToString(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Observe_0020Synchronize_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Validate_0020Materialize_0020Acquire_002ESynchronize, message));
	}

	private static string Evaluate_0020Resolve_0020Worker_002EAcquire(string Process_0020Normalize_0020Worker_002EProcess)
	{
		return Regex.Replace(Regex.Replace(Regex.Replace(Process_0020Normalize_0020Worker_002EProcess, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Resolve_0020Synchronize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Finalize_0020Resolve_0020Validate_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Worker_0020Synchronize_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Normalize_0020Transform_0020Initialize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Schedule_0020Evaluate_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Initialize_0020Evaluate_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}

	private static int Worker_0020Evaluate_0020Finalize_002EEvaluate(int Acquire_0020Dispatch_0020Synchronize_002EWorker)
	{
		for (int i = Acquire_0020Dispatch_0020Synchronize_002EWorker; i <= 65535; i++)
		{
			try
			{
				TcpListener tcpListener = new TcpListener(IPAddress.Loopback, i);
				tcpListener.Start();
				tcpListener.Stop();
				return i;
			}
			catch (SocketException)
			{
			}
		}
		throw new InvalidOperationException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Acquire_0020Worker_002ENormalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}

	private static object? Synchronize_0020Schedule_0020Normalize_002EAcquire(Interceptor Transform_0020Worker_0020Evaluate_002EObserve)
	{
		return typeof(Interceptor).GetProperty(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Worker_0020Finalize_0020Dispatch_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Transform_0020Worker_0020Evaluate_002EObserve);
	}

	private static async Task StopLanBroadcastWhenStartedAsync(Interceptor interceptor)
	{
		for (int attempt = 0; attempt < 30; attempt++)
		{
			object obj = Synchronize_0020Schedule_0020Normalize_002EAcquire(interceptor);
			if (obj != null && obj.GetType().GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) != null)
			{
				Schedule_0020Synchronize_0020Dispatch_002EFinalize(interceptor);
				return;
			}
			await Task.Delay(50);
		}
		Schedule_0020Synchronize_0020Dispatch_002EFinalize(interceptor);
	}

	private static void Process_0020Transform_0020Evaluate_002EDispatch(Interceptor Process_0020Evaluate_0020Dispatch_002EValidate)
	{
		try
		{
			object obj = Synchronize_0020Schedule_0020Normalize_002EAcquire(Process_0020Evaluate_0020Dispatch_002EValidate);
			obj?.GetType().GetMethod(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Transform_0020Worker_0020Resolve_002EObserve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(obj, null);
		}
		catch (Exception exception)
		{
			Log.Warning(exception, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Normalize_0020Synchronize_0020Transform_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		}
	}

	private static void Schedule_0020Synchronize_0020Dispatch_002EFinalize(Interceptor Synchronize_0020Materialize_0020Transform_002ENormalize)
	{
		try
		{
			object obj = Synchronize_0020Schedule_0020Normalize_002EAcquire(Synchronize_0020Materialize_0020Transform_002ENormalize);
			obj?.GetType().GetMethod(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Normalize_0020Process_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(obj, null);
		}
		catch (Exception exception)
		{
			Log.Warning(exception, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Observe_0020Process_002EWorker, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
		}
	}
}
