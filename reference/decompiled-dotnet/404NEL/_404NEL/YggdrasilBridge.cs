using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Codexus.OpenSDK.Entities;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Codexus.OpenSDK.Yggdrasil;

namespace _404NEL;

public static class YggdrasilBridge
{
	private sealed class YggdrasilRequest
	{
		public string EntityId { get; set; } = string.Empty;

		public string GameToken { get; set; } = string.Empty;

		public string ServerHash { get; set; } = string.Empty;

		public string GameId { get; set; } = string.Empty;

		public string GameVersion { get; set; } = string.Empty;

		public string LauncherVersion { get; set; } = string.Empty;

		public string Channel { get; set; } = "netease";

		public string BootstrapMd5 { get; set; } = string.Empty;

		public string DatFileMd5 { get; set; } = string.Empty;

		public ModList? Mods { get; set; }
	}

	private const string AuthServerListUrl = "https://x19.update.netease.com/authserver.list";

	private static readonly HttpClient Materialize_0020Resolve_0020Process_002EObserve = new HttpClient();

	private static readonly YggdrasilAuthServer[] Validate_0020Observe_0020Initialize_002ESynchronize = new YggdrasilAuthServer[4]
	{
		new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Dispatch_0020Initialize_0020Resolve_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 10801, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Transform_0020Process_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
		new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Synchronize_0020Evaluate_0020Normalize_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11001, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Synchronize_0020Materialize_0020Validate_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
		new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Materialize_0020Observe_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11201, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Resolve_0020Acquire_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
		new YggdrasilAuthServer(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Acquire_0020Normalize_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), 11401, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Synchronize_0020Evaluate_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform))
	};

	public static async Task<string> AuthenticateAsync(string requestJson, string crcSalt)
	{
		_ = 1;
		try
		{
			YggdrasilRequest request = JsonSerializer.Deserialize<YggdrasilRequest>(requestJson, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			}) ?? throw new InvalidOperationException("认证请求为空");
			if (string.IsNullOrWhiteSpace(crcSalt))
			{
				throw new InvalidOperationException("CRC salt 为空");
			}
			if (string.IsNullOrWhiteSpace(request.ServerHash))
			{
				throw new InvalidOperationException("服务器握手哈希为空");
			}
			if (string.IsNullOrWhiteSpace(request.GameToken))
			{
				throw new InvalidOperationException("游戏 Token 为空");
			}
			if (!int.TryParse(request.EntityId, out var result))
			{
				throw new InvalidOperationException("UID 格式无效");
			}
			GameProfile profile = new GameProfile
			{
				GameId = (request.GameId ?? string.Empty),
				GameVersion = (request.GameVersion ?? string.Empty),
				BootstrapMd5 = (request.BootstrapMd5 ?? string.Empty),
				DatFileMd5 = (request.DatFileMd5 ?? string.Empty),
				Mods = (request.Mods ?? new ModList()),
				User = new UserProfile
				{
					UserId = result,
					UserToken = request.GameToken
				}
			};
			YggdrasilData yggdrasilData = new YggdrasilData
			{
				LauncherVersion = (request.LauncherVersion ?? string.Empty),
				Channel = (string.IsNullOrWhiteSpace(request.Channel) ? "netease" : request.Channel),
				CrcSalt = crcSalt
			};
			List<string> errors = new List<string>();
			foreach (YggdrasilAuthServer authServer in await GetAuthServersAsync())
			{
				try
				{
					Result result2 = await new StandardYggdrasil(yggdrasilData, authServer.IP, authServer.Port).JoinServerAsync(profile, request.ServerHash).WaitAsync(TimeSpan.FromSeconds(12L));
					if (result2.IsSuccess)
					{
						return JsonSerializer.Serialize(new
						{
							success = true,
							error = string.Empty,
							authServer = $"{authServer.IP}:{authServer.Port}"
						});
					}
					errors.Add($"{authServer.IP}:{authServer.Port}: " + (result2.Error ?? "认证失败"));
				}
				catch (Exception ex)
				{
					errors.Add($"{authServer.IP}:{authServer.Port}: " + ex.Message);
				}
			}
			return JsonSerializer.Serialize(new
			{
				success = false,
				error = "全部 Yggdrasil 认证节点失败：" + string.Join(" | ", errors)
			});
		}
		catch (Exception ex2)
		{
			return JsonSerializer.Serialize(new
			{
				success = false,
				error = ex2.Message
			});
		}
	}

	private static async Task<IReadOnlyList<YggdrasilAuthServer>> GetAuthServersAsync()
	{
		try
		{
			using CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(8L));
			YggdrasilAuthServer[] array = JsonSerializer.Deserialize<YggdrasilAuthServer[]>(await Materialize_0020Resolve_0020Process_002EObserve.GetStringAsync("https://x19.update.netease.com/authserver.list", cancellation.Token), new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			})?.Where((YggdrasilAuthServer server) => !string.IsNullOrWhiteSpace(server.IP) && server.Port > 0).DistinctBy((YggdrasilAuthServer server) => $"{server.IP}:{server.Port}").ToArray();
			if (array != null && array.Length > 0)
			{
				return array;
			}
		}
		catch
		{
		}
		return Validate_0020Observe_0020Initialize_002ESynchronize;
	}
}
