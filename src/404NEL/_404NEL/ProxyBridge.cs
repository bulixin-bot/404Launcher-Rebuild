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
				Initialize_0020Validate_0020Worker_002EObserve(text2, "[Interceptor] " + text);
			}
		}
	}

	private const string DefaultCrcSalt = "421C4417360637BA22478129C9AFB8C5";

	private static readonly ConcurrentDictionary<Guid, Interceptor> Process_0020Acquire_0020Initialize_002ESynchronize;

	private static readonly ConcurrentDictionary<Guid, bool> Synchronize_0020Normalize_0020Synchronize_002EMaterialize;

	private static readonly ConcurrentDictionary<Guid, List<Worker_0020Worker_0020Acquire_002EValidate>> Dispatch_0020Worker_0020Validate_002ESchedule;

	private static readonly ConcurrentDictionary<Guid, Evaluate_0020Transform_0020Initialize_002ETransform> Synchronize_0020Resolve_0020Synchronize_002EAcquire;

	private static readonly ConcurrentDictionary<string, string> Initialize_0020Schedule_0020Dispatch_002EEvaluate;

	private static readonly ConcurrentDictionary<string, DateTimeOffset> Synchronize_0020Synchronize_0020Initialize_002EAcquire;

	private static readonly ConcurrentDictionary<string, SemaphoreSlim> Evaluate_0020Acquire_0020Dispatch_002EWorker;

	private static readonly ConcurrentQueue<ProxyLogEntry> Initialize_0020Process_0020Transform_002EProcess;

	private static readonly ConcurrentQueue<ProxyProgressEntry> Observe_0020Transform_0020Worker_002ENormalize;

	private static readonly ConcurrentQueue<ProxyTokenUpdate> Resolve_0020Synchronize_0020Validate_002EInitialize;

	private static readonly ConcurrentQueue<ProxyReloginRequest> Normalize_0020Resolve_0020Acquire_002EMaterialize;

	private static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> Finalize_0020Dispatch_0020Resolve_002EEvaluate;

	private static volatile bool Process_0020Initialize_0020Validate_002ETransform;

	private static volatile bool Process_0020Finalize_0020Dispatch_002EValidate;

	private static string Worker_0020Initialize_0020Initialize_002EAcquire;

	private static readonly WPFLauncher Dispatch_0020Resolve_0020Initialize_002EEvaluate;

	private static readonly HttpClient Dispatch_0020Transform_0020Evaluate_002EFinalize;

	private static readonly YggdrasilAuthServer[] Dispatch_0020Finalize_0020Evaluate_002EResolve;

	static ProxyBridge()
	{
		Process_0020Acquire_0020Initialize_002ESynchronize = new ConcurrentDictionary<Guid, Interceptor>();
		Synchronize_0020Normalize_0020Synchronize_002EMaterialize = new ConcurrentDictionary<Guid, bool>();
		Dispatch_0020Worker_0020Validate_002ESchedule = new ConcurrentDictionary<Guid, List<Worker_0020Worker_0020Acquire_002EValidate>>();
		Synchronize_0020Resolve_0020Synchronize_002EAcquire = new ConcurrentDictionary<Guid, Evaluate_0020Transform_0020Initialize_002ETransform>();
		Initialize_0020Schedule_0020Dispatch_002EEvaluate = new ConcurrentDictionary<string, string>();
		Synchronize_0020Synchronize_0020Initialize_002EAcquire = new ConcurrentDictionary<string, DateTimeOffset>();
		Evaluate_0020Acquire_0020Dispatch_002EWorker = new ConcurrentDictionary<string, SemaphoreSlim>();
		Initialize_0020Process_0020Transform_002EProcess = new ConcurrentQueue<ProxyLogEntry>();
		Observe_0020Transform_0020Worker_002ENormalize = new ConcurrentQueue<ProxyProgressEntry>();
		Resolve_0020Synchronize_0020Validate_002EInitialize = new ConcurrentQueue<ProxyTokenUpdate>();
		Normalize_0020Resolve_0020Acquire_002EMaterialize = new ConcurrentQueue<ProxyReloginRequest>();
		Finalize_0020Dispatch_0020Resolve_002EEvaluate = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
		Process_0020Initialize_0020Validate_002ETransform = true;
		Process_0020Finalize_0020Dispatch_002EValidate = true;
		Worker_0020Initialize_0020Initialize_002EAcquire = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000086.Initialize_0020Evaluate_0020Process_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
		Dispatch_0020Resolve_0020Initialize_002EEvaluate = new WPFLauncher();
		Dispatch_0020Transform_0020Evaluate_002EFinalize = new HttpClient();
		Dispatch_0020Finalize_0020Evaluate_002EResolve = new YggdrasilAuthServer[4]
		{
			new YggdrasilAuthServer(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000087.Acquire_0020Worker_0020Schedule_002EProcess, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 10801, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000088.Worker_0020Schedule_0020Finalize_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			new YggdrasilAuthServer(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000089.Schedule_0020Observe_0020Finalize_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 11001, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008A.Process_0020Transform_0020Synchronize_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			new YggdrasilAuthServer(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008B.Dispatch_0020Transform_0020Initialize_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 11201, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008C.Process_0020Materialize_0020Worker_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			new YggdrasilAuthServer(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008D.Validate_0020Resolve_0020Materialize_002EValidate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 11401, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008E.Resolve_0020Materialize_0020Worker_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate))
		};
		Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Sink(new BridgeLogSink()).CreateLogger();
		PacketManager.Instance.RegisterPacketFromAssembly(Protocol772Registration.PacketType.Assembly);
		int[] array = new int[2] { 772, 773 };
		foreach (int version in array)
		{
			PacketManager.Instance.RegisterPacket(new RegisterPacket(EnumConnectionState.Login, EnumPacketDirection.ServerBound, 1, (EnumProtocolVersion)version), typeof(CPacketEncryptionResponse));
			PacketManager.Instance.RegisterPacket(new RegisterPacket(EnumConnectionState.Handshaking, EnumPacketDirection.ServerBound, 0, (EnumProtocolVersion)version), typeof(CHandshake));
		}
		Initialize_0020Validate_0020Worker_002EObserve(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200008F.Observe_0020Normalize_0020Schedule_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000090.Worker_0020Synchronize_0020Dispatch_002EProcess, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
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
			Initialize_0020Schedule_0020Dispatch_002EEvaluate[entityId] = accessToken;
			if (markRefresh)
			{
				Synchronize_0020Synchronize_0020Initialize_002EAcquire[entityId] = DateTimeOffset.UtcNow;
			}
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", "[Token] 已同步：UID=" + entityId + "，来源=" + (markRefresh ? "AuthenticationUpdate" : "登录会话"));
			await RefreshSessionsForUserAsync(entityId, accessToken);
			if (Finalize_0020Dispatch_0020Resolve_002EEvaluate.TryGetValue(entityId, out TaskCompletionSource<string> value))
			{
				value.TrySetResult(accessToken);
			}
		}
	}

	public static ProxyLogEntry[] DrainLogs()
	{
		List<ProxyLogEntry> list = new List<ProxyLogEntry>();
		ProxyLogEntry result;
		while (Initialize_0020Process_0020Transform_002EProcess.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyProgressEntry[] DrainProgress()
	{
		List<ProxyProgressEntry> list = new List<ProxyProgressEntry>();
		ProxyProgressEntry result;
		while (Observe_0020Transform_0020Worker_002ENormalize.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyTokenUpdate[] DrainTokenUpdates()
	{
		List<ProxyTokenUpdate> list = new List<ProxyTokenUpdate>();
		ProxyTokenUpdate result;
		while (Resolve_0020Synchronize_0020Validate_002EInitialize.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static ProxyReloginRequest[] DrainReloginRequests()
	{
		List<ProxyReloginRequest> list = new List<ProxyReloginRequest>();
		ProxyReloginRequest result;
		while (Normalize_0020Resolve_0020Acquire_002EMaterialize.TryDequeue(out result))
		{
			list.Add(result);
		}
		return list.ToArray();
	}

	public static void SetProgramAccessState(bool accessGranted, string reason)
	{
		Process_0020Initialize_0020Validate_002ETransform = accessGranted;
		Worker_0020Initialize_0020Initialize_002EAcquire = (string.IsNullOrWhiteSpace(reason) ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000091.Transform_0020Process_0020Acquire_002EValidate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : reason.Trim());
		Initialize_0020Validate_0020Worker_002EObserve(accessGranted ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000093.Acquire_0020Synchronize_0020Synchronize_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000092.Acquire_0020Initialize_0020Schedule_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), accessGranted ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000095.Initialize_0020Acquire_0020Observe_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : (Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000094.Evaluate_0020Worker_0020Normalize_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) + Worker_0020Initialize_0020Initialize_002EAcquire));
	}

	public static void SetLanBroadcastEnabled(bool enabled)
	{
		Process_0020Finalize_0020Dispatch_002EValidate = enabled;
		int num = 0;
		foreach (KeyValuePair<Guid, Interceptor> item in Process_0020Acquire_0020Initialize_002ESynchronize)
		{
			if (Synchronize_0020Normalize_0020Synchronize_002EMaterialize.GetOrAdd(item.Key, value: true) != enabled)
			{
				if (enabled)
				{
					Schedule_0020Finalize_0020Materialize_002ETransform(item.Value);
				}
				else
				{
					Schedule_0020Materialize_0020Schedule_002EFinalize(item.Value);
				}
				Synchronize_0020Normalize_0020Synchronize_002EMaterialize[item.Key] = enabled;
				num++;
			}
		}
		string text = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000096.Synchronize_0020Finalize_0020Resolve_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000097.Evaluate_0020Observe_0020Process_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendFormatted(enabled ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000099.Validate_0020Acquire_0020Transform_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000098.Dispatch_0020Observe_0020Initialize_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009A.Worker_0020Resolve_0020Synchronize_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009B.Resolve_0020Evaluate_0020Materialize_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		defaultInterpolatedStringHandler.AppendFormatted(num);
		defaultInterpolatedStringHandler.AppendLiteral(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009C.Dispatch_0020Worker_0020Acquire_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		Initialize_0020Validate_0020Worker_002EObserve(text, defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private static async Task<string?> RequestAccountReloginAsync(string entityId, string reason)
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
		TaskCompletionSource<string> waiter = Finalize_0020Dispatch_0020Resolve_002EEvaluate.GetOrAdd(entityId, taskCompletionSource);
		if (waiter == taskCompletionSource)
		{
			Normalize_0020Resolve_0020Acquire_002EMaterialize.Enqueue(new ProxyReloginRequest(entityId, reason));
			Initialize_0020Validate_0020Worker_002EObserve("WARN", $"[Token] 已请求客户端重新登录账号并保持游戏连接：UID={entityId}，原因={reason}");
		}
		else
		{
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", "[Token] 复用进行中的账号重新登录：UID=" + entityId);
		}
		try
		{
			string result = await waiter.Task.WaitAsync(TimeSpan.FromSeconds(90L));
			Initialize_0020Validate_0020Worker_002EObserve("INFO", "[Token] 客户端重新登录完成，继续当前游戏连接：UID=" + entityId);
			return result;
		}
		catch (TimeoutException)
		{
			Initialize_0020Validate_0020Worker_002EObserve("ERROR", "[Token] 等待客户端重新登录超时：UID=" + entityId);
			return null;
		}
		finally
		{
			if (Finalize_0020Dispatch_0020Resolve_002EEvaluate.TryGetValue(entityId, out TaskCompletionSource<string> value) && value == waiter)
			{
				Finalize_0020Dispatch_0020Resolve_002EEvaluate.TryRemove(entityId, out TaskCompletionSource<string> _);
			}
		}
	}

	private static string Materialize_0020Observe_0020Validate_002EResolve(string Acquire_0020Synchronize_0020Schedule_002EAcquire, string Validate_0020Synchronize_0020Evaluate_002EWorker)
	{
		if (!Initialize_0020Schedule_0020Dispatch_002EEvaluate.TryGetValue(Acquire_0020Synchronize_0020Schedule_002EAcquire, out string value) || string.IsNullOrWhiteSpace(value))
		{
			return Validate_0020Synchronize_0020Evaluate_002EWorker;
		}
		return value;
	}

	private static bool Validate_0020Normalize_0020Worker_002EValidate(string Evaluate_0020Finalize_0020Finalize_002EObserve)
	{
		if (Synchronize_0020Synchronize_0020Initialize_002EAcquire.TryGetValue(Evaluate_0020Finalize_0020Finalize_002EObserve, out var value))
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
		List<Worker_0020Worker_0020Acquire_002EValidate> statusProxies = new List<Worker_0020Worker_0020Acquire_002EValidate>();
		try
		{
			Initialize_0020Schedule_0020Dispatch_002EEvaluate[entityId] = accessToken;
			string currentToken = Materialize_0020Observe_0020Validate_002EResolve(entityId, accessToken);
			Initialize_0020Validate_0020Worker_002EObserve("INFO", $"[Proxy] 开始启动：UID={entityId}，服务器={serverId}，角色={role}");
			WPFLauncher wPFLauncher = Dispatch_0020Resolve_0020Initialize_002EEvaluate;
			Entities<EntityGameCharacter> entities = wPFLauncher.QueryNetGameCharacters(entityId, currentToken, serverId);
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Proxy] 角色校验响应：服务器={serverId}，角色数={entities.Data.Length}");
			if (entities.Data.All((EntityGameCharacter item) => item.Name != role))
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", "启动代理失败：角色不存在 " + role);
				return new ProxyStartResult(Success: false, "", "", "", "Selected role was not found");
			}
			Entity<EntityQueryNetGameDetailItem> entity = wPFLauncher.QueryNetGameDetailById(entityId, currentToken, serverId);
			Entity<EntityNetGameServerAddress> address = wPFLauncher.GetNetGameServerAddress(entityId, currentToken, serverId);
			EntityMcVersion version = entity.Data.McVersionList[0];
			string authVersionName = Synchronize_0020Normalize_0020Normalize_002EObserve.Dispatch_0020Initialize_0020Transform_002EInitialize(version.Name);
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Proxy] 服务器信息解析完成：版本={version.Name}，认证版本={authVersionName}，上游={address.Data.Ip}:{address.Data.Port}");
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
			EntityModsList entityModsList = (skipResourcePreparation ? Evaluate_0020Worker_0020Acquire_002EFinalize.Acquire_0020Worker_0020Synchronize_002EInitialize(serverId, Worker_0020Materialize_0020Finalize_002EInitialize) : null);
			if (entityModsList == null)
			{
				if (skipResourcePreparation)
				{
					Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Resource] 服务器 " + serverId + " 没有可复用的批次资源，回退到完整准备");
				}
				entityModsList = await Evaluate_0020Worker_0020Acquire_002EFinalize.InstallGameMods(entityId, currentToken, gameVersion, wPFLauncher, serverId, isRental: false, Worker_0020Materialize_0020Finalize_002EInitialize);
				Evaluate_0020Worker_0020Acquire_002EFinalize.Materialize_0020Normalize_0020Transform_002EProcess(serverId, entityModsList);
			}
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", skipResourcePreparation ? ("[Resource] 服务器 " + serverId + " 已跳过下载与校验") : ("[Resource] 服务器 " + serverId + " 已完成完整资源准备"));
			string mods = JsonSerializer.Serialize(entityModsList);
			Synchronize_0020Normalize_0020Normalize_002EObserve.Md5Pair pair = Synchronize_0020Normalize_0020Normalize_002EObserve.Finalize_0020Materialize_0020Materialize_002EInitialize(authVersionName);
			X19 x = new X19();
			YggdrasilData yggdrasilData = new YggdrasilData
			{
				LauncherVersion = x.GameVersion,
				Channel = "netease",
				CrcSalt = (string.IsNullOrWhiteSpace(crcSalt) ? "421C4417360637BA22478129C9AFB8C5" : crcSalt.Trim().ToUpperInvariant())
			};
			EntitySocks5 socks = new EntitySocks5
			{
				Enabled = socksEnabled,
				Address = socksAddress,
				Port = socksPort,
				Username = socksUsername,
				Password = socksPassword
			};
			string displayMotd = "§b404 §f" + address.Data.Ip + " -> " + role;
			int localPort = Initialize_0020Observe_0020Materialize_002ESynchronize(30000);
			interceptor = Interceptor.CreateInterceptor(socks, mods, serverId, displayMotd, version.Name, address.Data.Ip, address.Data.Port, role, entityId, currentToken, (string certification) =>
			{
				Initialize_0020Validate_0020Worker_002EObserve("INFO", $"[Yggdrasil] 收到认证请求：UID={entityId}，服务器={serverId}，版本={version.Name}，认证版本={authVersionName}");
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
							string token = Materialize_0020Observe_0020Validate_002EResolve(entityId, accessToken);
							Exception lastError = await AuthenticateAsync(token);
							if (lastError != null)
							{
								Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Yggdrasil] 首轮认证失败，检查 Token 刷新冷却");
								string text2 = await TryRefreshTokenIfDueAsync(entityId, "Yggdrasil 认证失败");
								if (!string.IsNullOrWhiteSpace(text2))
								{
									token = text2;
									Initialize_0020Validate_0020Worker_002EObserve("INFO", "[Yggdrasil] Token 已刷新，重新提交认证");
									lastError = await AuthenticateAsync(token);
								}
							}
							if (lastError != null)
							{
								throw lastError;
							}
							Initialize_0020Validate_0020Worker_002EObserve("INFO", "[Yggdrasil] 认证成功，可以继续登录服务器");
							Log.Information("Proxy Yggdrasil authentication succeeded");
						}
						catch (Exception ex2)
						{
							Exception ex3 = (authorizationError = ex2);
							Initialize_0020Validate_0020Worker_002EObserve("ERROR", "Yggdrasil 认证失败：" + ex3.Message);
							Log.Error(ex3, "Proxy Yggdrasil authentication failed");
						}
						finally
						{
							authorization.Set();
						}
					});
					if (!authorization.Wait(TimeSpan.FromSeconds(35L)))
					{
						Initialize_0020Validate_0020Worker_002EObserve("ERROR", "Yggdrasil 认证等待超过 35 秒");
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
					Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Yggdrasil] 认证参数已构建：UID={entityId}，服务器={serverId}，版本={version.Name}，认证版本={authVersionName}，ModsJsonBytes={mods.Length}（敏感字段已隐藏）");
					Exception lastError = null;
					foreach (YggdrasilAuthServer authServer in await GetYggdrasilAuthServersAsync())
					{
						try
						{
							Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Yggdrasil] 正在连接认证节点 {authServer.IP}:{authServer.Port}" + (string.IsNullOrWhiteSpace(authServer.ServerType) ? "" : ("，类型=" + authServer.ServerType)));
							Result result = await new StandardYggdrasil(yggdrasilData, authServer.IP, authServer.Port).JoinServerAsync(profile, certification).WaitAsync(TimeSpan.FromSeconds(12L));
							Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Yggdrasil] 节点响应：{authServer.IP}:{authServer.Port}，成功={result.IsSuccess}，错误={result.Error ?? "无"}");
							if (!result.IsSuccess)
							{
								throw new InvalidOperationException(result.Error ?? "Yggdrasil authentication failed");
							}
							return null;
						}
						catch (Exception ex2)
						{
							lastError = ex2;
							Initialize_0020Validate_0020Worker_002EObserve("WARN", $"[Yggdrasil] 认证节点 {authServer.IP}:{authServer.Port} 失败：{ex2.Message}");
						}
					}
					return lastError;
				}
			}, "127.0.0.1", localPort);
			Synchronize_0020Normalize_0020Synchronize_002EMaterialize[interceptor.Identifier] = true;
			if (!Process_0020Finalize_0020Dispatch_002EValidate)
			{
				await StopLanBroadcastWhenStartedAsync(interceptor);
				Synchronize_0020Normalize_0020Synchronize_002EMaterialize[interceptor.Identifier] = false;
			}
			string listenAddress = Finalize_0020Transform_0020Materialize_002ENormalize();
			Worker_0020Worker_0020Acquire_002EValidate obj = new Worker_0020Worker_0020Acquire_002EValidate(25565, listenAddress, interceptor.LocalAddress, interceptor.LocalPort, address.Data.Ip, address.Data.Port, displayMotd, () => ValidateLoginPreflightAsync(entityId, serverId, role), Initialize_0020Validate_0020Worker_002EObserve);
			obj.Resolve_0020Resolve_0020Validate_002EValidate();
			statusProxies.Add(obj);
			Worker_0020Worker_0020Acquire_002EValidate obj2 = new Worker_0020Worker_0020Acquire_002EValidate(25565, IPAddress.Loopback.ToString(), interceptor.LocalAddress, interceptor.LocalPort, address.Data.Ip, address.Data.Port, displayMotd, () => ValidateLoginPreflightAsync(entityId, serverId, role), Initialize_0020Validate_0020Worker_002EObserve);
			obj2.Resolve_0020Resolve_0020Validate_002EValidate();
			statusProxies.Add(obj2);
			await InterConn.GameStart(entityId, currentToken, string.IsNullOrWhiteSpace(gameId) ? serverId : gameId);
			await X19.InterconnectionApi.GameStartAsync(entityId, currentToken, serverId);
			if (Synchronize_0020Normalize_0020Synchronize_002EMaterialize.GetOrAdd(interceptor.Identifier, value: true) != Process_0020Finalize_0020Dispatch_002EValidate)
			{
				if (Process_0020Finalize_0020Dispatch_002EValidate)
				{
					Schedule_0020Finalize_0020Materialize_002ETransform(interceptor);
				}
				else
				{
					Schedule_0020Materialize_0020Schedule_002EFinalize(interceptor);
				}
				Synchronize_0020Normalize_0020Synchronize_002EMaterialize[interceptor.Identifier] = Process_0020Finalize_0020Dispatch_002EValidate;
			}
			Process_0020Acquire_0020Initialize_002ESynchronize[interceptor.Identifier] = interceptor;
			Dispatch_0020Worker_0020Validate_002ESchedule[interceptor.Identifier] = statusProxies;
			Synchronize_0020Resolve_0020Synchronize_002EAcquire[interceptor.Identifier] = new Evaluate_0020Transform_0020Initialize_002ETransform(entityId, serverId, string.IsNullOrWhiteSpace(gameId) ? serverId : gameId);
			Initialize_0020Validate_0020Worker_002EObserve("INFO", "代理启动成功：" + serverName + " / " + role);
			return new ProxyStartResult(Success: true, string.Join(" / ", statusProxies.Select((Worker_0020Worker_0020Acquire_002EValidate proxy) => $"{proxy.ListenAddress}:{proxy.Port}")), interceptor.Identifier.ToString(), $"{address.Data.Ip}:{address.Data.Port}", "");
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
			Initialize_0020Validate_0020Worker_002EObserve("ERROR", "代理启动失败：" + ex.Message);
			foreach (Worker_0020Worker_0020Acquire_002EValidate item in statusProxies)
			{
				item.Dispose();
			}
			if (interceptor != null)
			{
				Synchronize_0020Normalize_0020Synchronize_002EMaterialize.TryRemove(interceptor.Identifier, out var _);
				interceptor.ShutdownAsync();
			}
			return new ProxyStartResult(Success: false, "", "", "", ex.Message);
		}
	}

	private static void Worker_0020Materialize_0020Finalize_002EInitialize(Validate_0020Observe_0020Synchronize_002EEvaluate Schedule_0020Materialize_0020Worker_002EObserve)
	{
		Observe_0020Transform_0020Worker_002ENormalize.Enqueue(new ProxyProgressEntry(DateTime.Now.ToString(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009D.Resolve_0020Process_0020Validate_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Schedule_0020Materialize_0020Worker_002EObserve.Stage, Schedule_0020Materialize_0020Worker_002EObserve.ResourceName, Schedule_0020Materialize_0020Worker_002EObserve.DownloadedBytes, Schedule_0020Materialize_0020Worker_002EObserve.TotalBytes, Schedule_0020Materialize_0020Worker_002EObserve.Percent, Schedule_0020Materialize_0020Worker_002EObserve.SpeedBytesPerSecond, Schedule_0020Materialize_0020Worker_002EObserve.Message));
	}

	private static async Task<string?> TryRefreshTokenIfDueAsync(string entityId, string reason)
	{
		SemaphoreSlim refreshLock = Evaluate_0020Acquire_0020Dispatch_002EWorker.GetOrAdd(entityId, (string _) => new SemaphoreSlim(1, 1));
		await refreshLock.WaitAsync();
		try
		{
			DateTimeOffset utcNow = DateTimeOffset.UtcNow;
			if (Synchronize_0020Synchronize_0020Initialize_002EAcquire.TryGetValue(entityId, out var value))
			{
				TimeSpan timeSpan = TimeSpan.FromMinutes(5L) - (utcNow - value);
				if (timeSpan > TimeSpan.Zero)
				{
					Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Token] 刷新被冷却阻止：UID={entityId}，场景={reason}，剩余={timeSpan.TotalSeconds:F0}秒");
					return null;
				}
			}
			string text = Materialize_0020Observe_0020Validate_002EResolve(entityId, "");
			if (string.IsNullOrWhiteSpace(text))
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Token] 无法执行认证失败刷新：UID=" + entityId + "，当前 Token 为空");
				return null;
			}
			Initialize_0020Validate_0020Worker_002EObserve("INFO", $"[Token] 调用 AuthenticationUpdateAsync：UID={entityId}，场景={reason}");
			string refreshedToken = (await Dispatch_0020Resolve_0020Initialize_002EEvaluate.AuthenticationUpdateAsync(entityId, text))?.Token?.ToString();
			if (string.IsNullOrWhiteSpace(refreshedToken))
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Token] AuthenticationUpdateAsync 未返回新 Token：UID=" + entityId);
				return null;
			}
			Initialize_0020Schedule_0020Dispatch_002EEvaluate[entityId] = refreshedToken;
			Synchronize_0020Synchronize_0020Initialize_002EAcquire[entityId] = DateTimeOffset.UtcNow;
			Resolve_0020Synchronize_0020Validate_002EInitialize.Enqueue(new ProxyTokenUpdate(entityId, refreshedToken));
			await RefreshSessionsForUserAsync(entityId, refreshedToken);
			Initialize_0020Validate_0020Worker_002EObserve("INFO", "[Token] 刷新成功并已同步代理：UID=" + entityId + "，场景=" + reason);
			return refreshedToken;
		}
		catch (Exception ex)
		{
			Initialize_0020Validate_0020Worker_002EObserve("ERROR", $"[Token] 刷新异常：UID={entityId}，场景={reason}，{ex.Message}");
			return null;
		}
		finally
		{
			refreshLock.Release();
		}
	}

	private static async Task RefreshSessionsForUserAsync(string entityId, string accessToken)
	{
		List<Evaluate_0020Transform_0020Initialize_002ETransform> list = Synchronize_0020Resolve_0020Synchronize_002EAcquire.Values.Where((Evaluate_0020Transform_0020Initialize_002ETransform obj) => obj.EntityId == entityId).Distinct().ToList();
		foreach (Evaluate_0020Transform_0020Initialize_002ETransform session in list)
		{
			try
			{
				await InterConn.GameStart(session.EntityId, accessToken, session.GameId);
				await X19.InterconnectionApi.GameStartAsync(session.EntityId, accessToken, session.ServerId);
				Initialize_0020Validate_0020Worker_002EObserve("DEBUG", "已同步运行中代理会话 Token：" + session.ServerId);
			}
			catch (Exception ex)
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", "同步运行中代理会话 Token 失败：" + session.ServerId + "，" + ex.Message);
			}
		}
	}

	private static string Finalize_0020Transform_0020Materialize_002ENormalize()
	{
		try
		{
			using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Connect(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009E.Finalize_0020Finalize_0020Normalize_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 53);
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
			YggdrasilAuthServer[] array = JsonSerializer.Deserialize<YggdrasilAuthServer[]>(await Dispatch_0020Transform_0020Evaluate_002EFinalize.GetStringAsync("https://x19.update.netease.com/authserver.list", cts.Token), new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			})?.Where((YggdrasilAuthServer server) => !string.IsNullOrWhiteSpace(server.IP) && server.Port > 0).DistinctBy((YggdrasilAuthServer server) => $"{server.IP}:{server.Port}").ToArray();
			if (array != null && array.Length > 0)
			{
				Initialize_0020Validate_0020Worker_002EObserve("DEBUG", "[Yggdrasil] 已从官方列表获取认证节点：" + string.Join(", ", array.Select((YggdrasilAuthServer server) => $"{server.IP}:{server.Port}" + (string.IsNullOrWhiteSpace(server.ServerType) ? "" : ("(" + server.ServerType + ")")))));
				return array;
			}
			Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Yggdrasil] 官方认证节点列表为空，使用兜底节点");
		}
		catch (Exception ex)
		{
			Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Yggdrasil] 获取官方认证节点失败，使用兜底节点：" + ex.Message);
		}
		return Dispatch_0020Finalize_0020Evaluate_002EResolve;
	}

	private static async Task<string?> ValidateLoginPreflightAsync(string entityId, string serverId, string role)
	{
		try
		{
			if (!Process_0020Initialize_0020Validate_002ETransform)
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", $"[Proxy.Preflight] 程序订阅无效，拒绝新连接：UID={entityId}，服务器={serverId}，角色={role}");
				return Worker_0020Initialize_0020Initialize_002EAcquire;
			}
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Proxy.Preflight] 客户端准备登录：UID={entityId}，服务器={serverId}，角色={role}");
			bool refreshWasDue = Validate_0020Normalize_0020Worker_002EValidate(entityId);
			string text = await TryRefreshTokenIfDueAsync(entityId, "客户端连接代理");
			if (refreshWasDue && string.IsNullOrWhiteSpace(text))
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", "[Proxy.Preflight] Token 刷新失败：UID=" + entityId + "，保持连接并尝试重新登录账号");
				text = await RequestAccountReloginAsync(entityId, "自动刷新 Token 失败");
				if (string.IsNullOrWhiteSpace(text))
				{
					return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
				}
			}
			string currentToken = text ?? Materialize_0020Observe_0020Validate_002EResolve(entityId, "");
			if (string.IsNullOrWhiteSpace(currentToken))
			{
				currentToken = await RequestAccountReloginAsync(entityId, "代理没有可用的登录 Token");
				if (string.IsNullOrWhiteSpace(currentToken))
				{
					return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
				}
			}
			Entities<EntityGameCharacter> roles = await Task.Run(() => Dispatch_0020Resolve_0020Initialize_002EEvaluate.QueryNetGameCharacters(entityId, currentToken, serverId));
			if (roles?.Data == null)
			{
				return "无法验证账号状态，请稍后重试。";
			}
			if (roles.Data.All((EntityGameCharacter item) => item.Name != role))
			{
				Initialize_0020Validate_0020Worker_002EObserve("WARN", $"[Proxy.Preflight] 角色查询未返回目标角色：UID={entityId}，服务器={serverId}，返回数量={roles.Data.Length}，目标角色={role}");
				if (roles.Data.Length == 0)
				{
					string reloginToken = await RequestAccountReloginAsync(entityId, "服务器未返回角色列表");
					if (!string.IsNullOrWhiteSpace(reloginToken))
					{
						roles = await Task.Run(() => Dispatch_0020Resolve_0020Initialize_002EEvaluate.QueryNetGameCharacters(entityId, reloginToken, serverId));
						Entities<EntityGameCharacter> entities = roles;
						if (entities != null && entities.Data?.Any((EntityGameCharacter item) => item.Name == role) == true)
						{
							Initialize_0020Validate_0020Worker_002EObserve("INFO", $"[Proxy.Preflight] 重新登录后角色验证通过：UID={entityId}，服务器={serverId}，角色={role}");
							return null;
						}
					}
				}
				return (roles.Data.Length == 0) ? "账号自动重新登录后仍未返回角色列表。" : ("角色不存在或已失效：" + role);
			}
			Initialize_0020Validate_0020Worker_002EObserve("DEBUG", $"[Proxy.Preflight] 登录预检通过：UID={entityId}，服务器={serverId}，角色={role}");
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
						Entities<EntityGameCharacter> entities2 = await Task.Run(() => Dispatch_0020Resolve_0020Initialize_002EEvaluate.QueryNetGameCharacters(entityId, reloginToken2, serverId));
						if (entities2 != null && entities2.Data?.Any((EntityGameCharacter item) => item.Name == role) == true)
						{
							return null;
						}
					}
					catch (Exception ex2)
					{
						Initialize_0020Validate_0020Worker_002EObserve("ERROR", $"[Proxy.Preflight] 重新登录后的验证仍失败：UID={entityId}，{ex2.Message}");
					}
				}
				return "账号自动重新登录失败或超时，请在 404 客户端检查账号状态。";
			}
			return "进入服务器前验证失败：" + message;
		}
	}

	public static Task<bool> StopAsync(string identifier)
	{
		if (!Guid.TryParse(identifier, out var result) || !Process_0020Acquire_0020Initialize_002ESynchronize.TryRemove(result, out Interceptor value))
		{
			return Task.FromResult(result: false);
		}
		if (Dispatch_0020Worker_0020Validate_002ESchedule.TryRemove(result, out List<Worker_0020Worker_0020Acquire_002EValidate> value2))
		{
			foreach (Worker_0020Worker_0020Acquire_002EValidate item in value2)
			{
				item.Dispose();
			}
		}
		Synchronize_0020Resolve_0020Synchronize_002EAcquire.TryRemove(result, out Evaluate_0020Transform_0020Initialize_002ETransform _);
		Synchronize_0020Normalize_0020Synchronize_002EMaterialize.TryRemove(result, out var _);
		value.ShutdownAsync();
		Initialize_0020Validate_0020Worker_002EObserve(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200009F.Materialize_0020Validate_0020Acquire_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A0.Dispatch_0020Process_0020Normalize_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) + identifier);
		return Task.FromResult(result: true);
	}

	private static void Initialize_0020Validate_0020Worker_002EObserve(string Dispatch_0020Normalize_0020Worker_002ETransform, string Normalize_0020Validate_0020Acquire_002EWorker)
	{
		string message = Evaluate_0020Normalize_0020Synchronize_002EProcess(Normalize_0020Validate_0020Acquire_002EWorker);
		Initialize_0020Process_0020Transform_002EProcess.Enqueue(new ProxyLogEntry(DateTime.Now.ToString(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A1.Worker_0020Synchronize_0020Worker_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Dispatch_0020Normalize_0020Worker_002ETransform, message));
	}

	private static string Evaluate_0020Normalize_0020Synchronize_002EProcess(string Acquire_0020Worker_0020Acquire_002ETransform)
	{
		return Regex.Replace(Regex.Replace(Regex.Replace(Acquire_0020Worker_0020Acquire_002ETransform, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A2.Finalize_0020Worker_0020Process_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A3.Process_0020Schedule_0020Finalize_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A4.Acquire_0020Evaluate_0020Initialize_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A5.Initialize_0020Normalize_0020Finalize_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A6.Normalize_0020Materialize_0020Process_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A7.Synchronize_0020Validate_0020Materialize_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
	}

	private static int Initialize_0020Observe_0020Materialize_002ESynchronize(int Evaluate_0020Transform_0020Initialize_002ENormalize)
	{
		for (int i = Evaluate_0020Transform_0020Initialize_002ENormalize; i <= 65535; i++)
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
		throw new InvalidOperationException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A8.Validate_0020Normalize_0020Process_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
	}

	private static object? Materialize_0020Normalize_0020Worker_002EDispatch(Interceptor Process_0020Schedule_0020Evaluate_002ESynchronize)
	{
		return typeof(Interceptor).GetProperty(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000A9.Normalize_0020Resolve_0020Schedule_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Process_0020Schedule_0020Evaluate_002ESynchronize);
	}

	private static async Task StopLanBroadcastWhenStartedAsync(Interceptor interceptor)
	{
		for (int attempt = 0; attempt < 30; attempt++)
		{
			object obj = Materialize_0020Normalize_0020Worker_002EDispatch(interceptor);
			if (obj != null && obj.GetType().GetField("_cts", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) != null)
			{
				Schedule_0020Materialize_0020Schedule_002EFinalize(interceptor);
				return;
			}
			await Task.Delay(50);
		}
		Schedule_0020Materialize_0020Schedule_002EFinalize(interceptor);
	}

	private static void Schedule_0020Finalize_0020Materialize_002ETransform(Interceptor Transform_0020Acquire_0020Synchronize_002EWorker)
	{
		try
		{
			object obj = Materialize_0020Normalize_0020Worker_002EDispatch(Transform_0020Acquire_0020Synchronize_002EWorker);
			obj?.GetType().GetMethod(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AA.Finalize_0020Dispatch_0020Normalize_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(obj, null);
		}
		catch (Exception exception)
		{
			Log.Warning(exception, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AB.Synchronize_0020Acquire_0020Validate_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		}
	}

	private static void Schedule_0020Materialize_0020Schedule_002EFinalize(Interceptor Validate_0020Materialize_0020Process_002EWorker)
	{
		try
		{
			object obj = Materialize_0020Normalize_0020Worker_002EDispatch(Validate_0020Materialize_0020Process_002EWorker);
			obj?.GetType().GetMethod(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AC.Materialize_0020Observe_0020Validate_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(obj, null);
		}
		catch (Exception exception)
		{
			Log.Warning(exception, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000AD.Resolve_0020Worker_0020Worker_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
		}
	}
}
