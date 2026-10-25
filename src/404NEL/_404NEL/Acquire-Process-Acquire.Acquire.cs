using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Mods;
using Codexus.Cipher.Protocol;
using Codexus.Game.Launcher.Utils;

namespace _404NEL;

internal static class Acquire_0020Process_0020Acquire_002EAcquire
{
	private static readonly ConcurrentDictionary<string, string> Synchronize_0020Materialize_0020Worker_002EProcess = new ConcurrentDictionary<string, string>();

	private static readonly HttpClient Validate_0020Normalize_0020Normalize_002EProcess = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(20L)
	};

	public static void Dispatch_0020Observe_0020Process_002EAcquire(string Resolve_0020Transform_0020Materialize_002EMaterialize, EntityModsList? Synchronize_0020Materialize_0020Dispatch_002EInitialize)
	{
		if (Synchronize_0020Materialize_0020Dispatch_002EInitialize != null)
		{
			Synchronize_0020Materialize_0020Worker_002EProcess[Resolve_0020Transform_0020Materialize_002EMaterialize] = JsonSerializer.Serialize(Synchronize_0020Materialize_0020Dispatch_002EInitialize);
		}
	}

	public static EntityModsList? Materialize_0020Resolve_0020Normalize_002EWorker(string Evaluate_0020Transform_0020Synchronize_002ENormalize, Action<Schedule_0020Materialize_0020Dispatch_002ENormalize> Dispatch_0020Materialize_0020Validate_002ETransform)
	{
		if (!Synchronize_0020Materialize_0020Worker_002EProcess.TryGetValue(Evaluate_0020Transform_0020Synchronize_002ENormalize, out string value))
		{
			return null;
		}
		EntityModsList? entityModsList = JsonSerializer.Deserialize<EntityModsList>(value);
		if (entityModsList != null)
		{
			Dispatch_0020Materialize_0020Validate_002ETransform(new Schedule_0020Materialize_0020Dispatch_002ENormalize(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000CD.Synchronize_0020Transform_0020Worker_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000CE.Observe_0020Initialize_0020Normalize_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), 0L, 0L, 100, 0.0, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000CF.Dispatch_0020Transform_0020Worker_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)));
		}
		return entityModsList;
	}

	public static async Task<EntityModsList?> InstallGameMods(string userId, string userToken, EnumGameVersion gameVersion, WPFLauncher launcher, string gameId, bool isRental, Action<Schedule_0020Materialize_0020Dispatch_002ENormalize> report)
	{
		report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("校验", "服务器资源清单", 0L, 0L, 0, 0.0, "正在获取资源清单"));
		Entity<EntityQuerySearchByGameResponse> entity = await launcher.GetGameCoreModListAsync(userId, userToken, gameVersion, isRental);
		if (entity.Data?.IidList == null)
		{
			return null;
		}
		Entities<EntityComponentDownloadInfoResponse> entities = await launcher.GetGameCoreModDetailsListAsync(userId, userToken, entity.Data.IidList);
		EntityModsList modList = new EntityModsList();
		EntityComponentDownloadInfoResponse[] data = entities.Data;
		foreach (EntityComponentDownloadInfoResponse entityComponentDownloadInfoResponse in data)
		{
			foreach (EntityComponentDownloadInfoResponseSub subEntity in entityComponentDownloadInfoResponse.SubEntities)
			{
				modList.Mods.Add(new EntityModsInfo
				{
					ModPath = $"{entityComponentDownloadInfoResponse.ItemId}@{entityComponentDownloadInfoResponse.MTypeId}@0.jar",
					Id = $"{entityComponentDownloadInfoResponse.ItemId}@{entityComponentDownloadInfoResponse.MTypeId}@0.jar",
					Iid = entityComponentDownloadInfoResponse.ItemId,
					Md5 = subEntity.JarMd5.ToUpperInvariant(),
					Name = "",
					Version = ""
				});
			}
		}
		string corePath = Path.Combine(PathUtil.GameModsPath, gameId);
		HashSet<string> expectedCoreJars = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		int totalComponents = Math.Max(entities.Data.Length, 1);
		for (int index = 0; index < entities.Data.Length; index++)
		{
			EntityComponentDownloadInfoResponse entityComponentDownloadInfoResponse2 = entities.Data[index];
			EntityComponentDownloadInfoResponseSub entityComponentDownloadInfoResponseSub = entityComponentDownloadInfoResponse2.SubEntities[0];
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entityComponentDownloadInfoResponseSub.ResName);
			string displayName = (string.IsNullOrWhiteSpace(entityComponentDownloadInfoResponseSub.ResName) ? $"核心模组 {index + 1}/{totalComponents}" : entityComponentDownloadInfoResponseSub.ResName);
			string jar = Path.Combine(corePath, $"{fileNameWithoutExtension}@{entityComponentDownloadInfoResponse2.MTypeId}@{entityComponentDownloadInfoResponse2.EntityId}.jar");
			expectedCoreJars.Add(Path.GetFullPath(jar));
			string archive = Path.Combine(corePath, entityComponentDownloadInfoResponseSub.ResName);
			string extractDirectory = Path.Combine(corePath, fileNameWithoutExtension);
			report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("校验", displayName, 0L, 0L, 0, 0.0, $"正在校验核心模组 {index + 1}/{totalComponents}"));
			if (!File.Exists(jar) || !FileUtil.ComputeMd5FromFile(jar).Equals(entityComponentDownloadInfoResponseSub.JarMd5, StringComparison.OrdinalIgnoreCase))
			{
				await DownloadWithProgress(entityComponentDownloadInfoResponseSub.ResUrl, archive, displayName, report);
				await ExtractWithBundled7ZipAsync(archive, extractDirectory, displayName, $"正在解压核心模组 {index + 1}/{totalComponents}", report);
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("安装", displayName, 0L, 0L, 100, 0.0, $"正在安装核心模组 {index + 1}/{totalComponents}"));
				string[] array = FileUtil.EnumerateFiles(extractDirectory, "jar");
				for (int i = 0; i < array.Length; i++)
				{
					FileUtil.CopyFileSafe(array[i], jar);
				}
				FileUtil.DeleteDirectorySafe(extractDirectory);
				FileUtil.DeleteFileSafe(archive);
			}
		}
		if (Directory.Exists(corePath))
		{
			foreach (string item in Directory.EnumerateFiles(corePath, "*.jar", SearchOption.TopDirectoryOnly))
			{
				if (!expectedCoreJars.Contains(Path.GetFullPath(item)))
				{
					FileUtil.DeleteFileSafe(item);
				}
			}
		}
		string componentDirectory = Path.Combine(PathUtil.CachePath, "Game", gameId);
		string componentArchive = componentDirectory + ".7z";
		string componentResumeMarker = componentArchive + ".md5";
		FileUtil.CreateDirectorySafe(componentDirectory);
		Entity<EntityComponentDownloadInfoResponse> netGameComponentDownloadList = launcher.GetNetGameComponentDownloadList(userId, userToken, gameId);
		if (netGameComponentDownloadList?.Data != null && netGameComponentDownloadList.Code == 0)
		{
			EntityComponentDownloadInfoResponseSub component = netGameComponentDownloadList.Data.SubEntities[0];
			string extractDirectory = Path.Combine(componentDirectory, gameId + ".MD5");
			string archive = Path.Combine(componentDirectory, gameId + ".json");
			bool flag = !File.Exists(extractDirectory);
			if (!flag)
			{
				flag = await File.ReadAllTextAsync(extractDirectory) != component.ResMd5;
			}
			if (!flag && File.Exists(archive))
			{
				EntityModsList entityModsList = JsonSerializer.Deserialize<EntityModsList>(await File.ReadAllTextAsync(archive));
				if (entityModsList != null)
				{
					modList.Mods.AddRange(entityModsList.Mods);
				}
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("完成", "服务器游戏资源", 0L, 0L, 100, 0.0, "已使用本地资源缓存"));
				return modList;
			}
			string jar = (string.IsNullOrWhiteSpace(component.ResName) ? "服务器游戏资源" : component.ResName);
			string text = ((!File.Exists(componentResumeMarker)) ? "" : (await File.ReadAllTextAsync(componentResumeMarker)));
			if (!string.Equals(text.Trim(), component.ResMd5, StringComparison.OrdinalIgnoreCase))
			{
				FileUtil.DeleteFileSafe(componentArchive);
				FileUtil.DeleteFileSafe(componentArchive + ".aria2");
				await File.WriteAllTextAsync(componentResumeMarker, component.ResMd5);
			}
			await DownloadWithProgress(component.ResUrl, componentArchive, jar, report);
			FileUtil.DeleteDirectorySafe(componentDirectory);
			await ExtractWithBundled7ZipAsync(componentArchive, componentDirectory, jar, "正在解压服务器游戏资源", report);
			report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("校验", jar, 0L, 0L, 100, 0.0, "正在校验服务器游戏资源"));
			EntityModsList serverMods = new EntityModsList();
			string[] array2 = FileUtil.EnumerateFiles(Path.Combine(componentDirectory, ".minecraft", "mods"), "jar");
			foreach (string path in array2)
			{
				string displayName = Path.GetFileName(path);
				string md = Convert.ToHexString(MD5.HashData(await File.ReadAllBytesAsync(path))).ToUpperInvariant();
				serverMods.Mods.Add(new EntityModsInfo
				{
					Name = "",
					Version = "",
					ModPath = displayName,
					Id = displayName,
					Iid = displayName.Split('@')[0],
					Md5 = md
				});
			}
			report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("安装", jar, 0L, 0L, 100, 0.0, "正在安装服务器游戏资源"));
			modList.Mods.AddRange(serverMods.Mods);
			await File.WriteAllTextAsync(extractDirectory, component.ResMd5);
			await File.WriteAllTextAsync(archive, JsonSerializer.Serialize(serverMods));
			FileUtil.DeleteFileSafe(componentArchive);
			FileUtil.DeleteFileSafe(componentResumeMarker);
		}
		report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("完成", "服务器游戏资源", 0L, 0L, 100, 0.0, "游戏资源已准备完成"));
		return modList;
	}

	private static async Task DownloadWithProgress(string url, string destinationPath, string resourceName, Action<Schedule_0020Materialize_0020Dispatch_002ENormalize> report)
	{
		long totalBytes = await TryGetContentLength(url);
		Exception lastError = null;
		for (int attempt = 1; attempt <= 3; attempt++)
		{
			report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("下载", resourceName, 0L, totalBytes, 0, 0.0, (attempt == 1) ? "正在使用 aria2 下载（8 线程）" : $"aria2 正在重试下载（{attempt}/{3}）"));
			try
			{
				await DownloadWithAria2Async(url, destinationPath, resourceName, totalBytes, report);
				long num = (File.Exists(destinationPath) ? new FileInfo(destinationPath).Length : totalBytes);
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("下载", resourceName, num, (totalBytes > 0) ? totalBytes : num, 100, 0.0, "aria2 资源下载完成"));
				return;
			}
			catch (Exception ex)
			{
				lastError = ex;
				string value = Observe_0020Dispatch_0020Observe_002EObserve(ex);
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("下载", resourceName, 0L, totalBytes, 0, 0.0, (attempt < 3) ? $"下载失败（{attempt}/{3}），准备重试：{value}" : $"下载失败（{attempt}/{3}）：{value}"));
			}
			if (attempt < 3)
			{
				await Task.Delay(TimeSpan.FromSeconds(attempt));
			}
		}
		string value2 = ((lastError == null) ? "未知下载错误" : Observe_0020Dispatch_0020Observe_002EObserve(lastError));
		throw new InvalidOperationException($"资源下载失败（已重试 {3} 次）：{resourceName}；原因：{value2}", lastError);
	}

	private static async Task DownloadWithAria2Async(string url, string destinationPath, string resourceName, long totalBytes, Action<Schedule_0020Materialize_0020Dispatch_002ENormalize> report)
	{
		string text = Process_0020Worker_0020Acquire_002EWorker();
		if (text == null)
		{
			throw new FileNotFoundException("未找到内置 aria2c.exe，请检查 tools\\aria2 文件是否完整。");
		}
		string fullDestinationPath = Path.GetFullPath(destinationPath);
		string text2 = Path.GetDirectoryName(fullDestinationPath) ?? throw new InvalidOperationException("资源下载目录无效。");
		Directory.CreateDirectory(text2);
		ProcessStartInfo processStartInfo = new ProcessStartInfo
		{
			FileName = text,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
			WorkingDirectory = text2
		};
		string[] array = new string[18]
		{
			"--allow-overwrite=true",
			"--auto-file-renaming=false",
			"--continue=true",
			"--file-allocation=none",
			"--max-connection-per-server=8",
			"--split=8",
			"--min-split-size=1M",
			"--max-tries=5",
			"--retry-wait=2",
			"--connect-timeout=20",
			"--timeout=60",
			"--summary-interval=1",
			"--console-log-level=notice",
			"--enable-color=false",
			"--download-result=hide",
			"--dir=" + text2,
			"--out=" + Path.GetFileName(fullDestinationPath),
			url
		};
		foreach (string item in array)
		{
			processStartInfo.ArgumentList.Add(item);
		}
		using Process process = Process.Start(processStartInfo) ?? throw new InvalidOperationException("无法启动内置 aria2c.exe。");
		ConcurrentQueue<string> outputLines = new ConcurrentQueue<string>();
		long parsedProgressAt = Stopwatch.GetTimestamp();
		process.OutputDataReceived += (object sender, DataReceivedEventArgs eventArgs) =>
		{
			string data = eventArgs.Data;
			if (!string.IsNullOrWhiteSpace(data))
			{
				outputLines.Enqueue(data);
				string result;
				while (outputLines.Count > 40 && outputLines.TryDequeue(out result))
				{
				}
				if (Process_0020Finalize_0020Synchronize_002EResolve(data, out var Dispatch_0020Worker_0020Observe_002ETransform, out var Validate_0020Observe_0020Materialize_002EEvaluate, out var Validate_0020Evaluate_0020Worker_002EProcess, out var Dispatch_0020Materialize_0020Materialize_002ESynchronize))
				{
					parsedProgressAt = Stopwatch.GetTimestamp();
					report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("下载", resourceName, Dispatch_0020Worker_0020Observe_002ETransform, (Validate_0020Observe_0020Materialize_002EEvaluate > 0) ? Validate_0020Observe_0020Materialize_002EEvaluate : totalBytes, Validate_0020Evaluate_0020Worker_002EProcess, Dispatch_0020Materialize_0020Materialize_002ESynchronize, "aria2 下载中（8 线程）"));
				}
			}
		};
		process.ErrorDataReceived += (object sender, DataReceivedEventArgs eventArgs) =>
		{
			if (!string.IsNullOrWhiteSpace(eventArgs.Data))
			{
				outputLines.Enqueue(eventArgs.Data);
			}
		};
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		Stopwatch stopwatch = Stopwatch.StartNew();
		long previousBytes = (File.Exists(fullDestinationPath) ? new FileInfo(fullDestinationPath).Length : 0);
		double previousElapsed = stopwatch.Elapsed.TotalSeconds;
		while (!process.HasExited)
		{
			await Task.Delay(500);
			if (!(Stopwatch.GetElapsedTime(parsedProgressAt).TotalSeconds < 2.0))
			{
				long num = (File.Exists(fullDestinationPath) ? new FileInfo(fullDestinationPath).Length : 0);
				double totalSeconds = stopwatch.Elapsed.TotalSeconds;
				double num2 = Math.Max(0.001, totalSeconds - previousElapsed);
				double speedBytesPerSecond = (double)Math.Max(0L, num - previousBytes) / num2;
				previousBytes = num;
				previousElapsed = totalSeconds;
				int percent = ((totalBytes > 0) ? Math.Clamp((int)(num * 100 / totalBytes), 0, 99) : 0);
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("下载", resourceName, num, totalBytes, percent, speedBytesPerSecond, "aria2 下载中（8 线程）"));
			}
		}
		await process.WaitForExitAsync();
		process.WaitForExit();
		string text3 = string.Join(" ", outputLines).Trim();
		if (process.ExitCode != 0)
		{
			string text4 = text3.Replace("\r", " ").Replace("\n", " ").Trim();
			if (text4.Length > 800)
			{
				string text5 = text4;
				int i = text5.Length;
				int num3 = i - 800;
				text4 = text5.Substring(num3, i - num3);
			}
			throw new InvalidOperationException($"aria2 下载失败（退出码 {process.ExitCode}）：{text4}");
		}
		if (!File.Exists(fullDestinationPath))
		{
			throw new FileNotFoundException("aria2 已退出，但资源文件不存在。", fullDestinationPath);
		}
	}

	private static bool Process_0020Finalize_0020Synchronize_002EResolve(string Dispatch_0020Validate_0020Finalize_002ESchedule, out long Dispatch_0020Worker_0020Observe_002ETransform, out long Validate_0020Observe_0020Materialize_002EEvaluate, out int Validate_0020Evaluate_0020Worker_002EProcess, out double Dispatch_0020Materialize_0020Materialize_002ESynchronize)
	{
		Dispatch_0020Worker_0020Observe_002ETransform = 0L;
		Validate_0020Observe_0020Materialize_002EEvaluate = 0L;
		Validate_0020Evaluate_0020Worker_002EProcess = 0;
		Dispatch_0020Materialize_0020Materialize_002ESynchronize = 0.0;
		Match match = Regex.Match(Dispatch_0020Validate_0020Finalize_002ESchedule, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D0.Resolve_0020Dispatch_0020Transform_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), RegexOptions.CultureInvariant);
		if (!match.Success)
		{
			return false;
		}
		Dispatch_0020Worker_0020Observe_002ETransform = Validate_0020Initialize_0020Validate_002EProcess(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D1.Observe_0020Validate_0020Schedule_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value);
		Validate_0020Observe_0020Materialize_002EEvaluate = Validate_0020Initialize_0020Validate_002EProcess(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D2.Initialize_0020Normalize_0020Materialize_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value);
		Dispatch_0020Materialize_0020Materialize_002ESynchronize = Validate_0020Initialize_0020Validate_002EProcess(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D3.Validate_0020Evaluate_0020Materialize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value);
		Validate_0020Evaluate_0020Worker_002EProcess = (match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D4.Schedule_0020Synchronize_0020Materialize_002ENormalize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Success ? Math.Clamp(int.Parse(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D5.Worker_0020Resolve_0020Evaluate_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value), 0, 99) : ((Validate_0020Observe_0020Materialize_002EEvaluate > 0) ? Math.Clamp((int)(Dispatch_0020Worker_0020Observe_002ETransform * 100 / Validate_0020Observe_0020Materialize_002EEvaluate), 0, 99) : 0));
		return true;
	}

	private static long Validate_0020Initialize_0020Validate_002EProcess(string Schedule_0020Acquire_0020Schedule_002EDispatch)
	{
		Match match = Regex.Match(Schedule_0020Acquire_0020Schedule_002EDispatch.Trim(), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D6.Initialize_0020Normalize_0020Observe_002EAcquire, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		if (!match.Success || !double.TryParse(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D7.Dispatch_0020Evaluate_0020Synchronize_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return 0L;
		}
		int num = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D8.Schedule_0020Evaluate_0020Validate_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform).IndexOf(match.Groups[Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000D9.Normalize_0020Materialize_0020Validate_002EObserve, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)].Value.ToUpperInvariant(), StringComparison.Ordinal);
		if (num < 0)
		{
			num = 0;
		}
		return (long)(result * Math.Pow(1024.0, num));
	}

	private static async Task ExtractWithBundled7ZipAsync(string archivePath, string outputDirectory, string resourceName, string message, Action<Schedule_0020Materialize_0020Dispatch_002ENormalize> report)
	{
		string text = Acquire_0020Dispatch_0020Dispatch_002EInitialize();
		if (text == null)
		{
			throw new FileNotFoundException("未找到内置 7z.exe，请检查 tools\\7zip 文件是否完整。");
		}
		string fullPath = Path.GetFullPath(archivePath);
		string fullPath2 = Path.GetFullPath(outputDirectory);
		Directory.CreateDirectory(fullPath2);
		report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("解压", resourceName, 0L, 0L, 0, 0.0, message + "（内置 7-Zip）"));
		ProcessStartInfo processStartInfo = new ProcessStartInfo
		{
			FileName = text,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
			WorkingDirectory = Path.GetDirectoryName(text)
		};
		processStartInfo.ArgumentList.Add("x");
		processStartInfo.ArgumentList.Add("-y");
		processStartInfo.ArgumentList.Add("-aoa");
		processStartInfo.ArgumentList.Add("-bb0");
		processStartInfo.ArgumentList.Add("-bsp1");
		processStartInfo.ArgumentList.Add("-o" + fullPath2);
		processStartInfo.ArgumentList.Add(fullPath);
		using Process process = Process.Start(processStartInfo) ?? throw new InvalidOperationException("无法启动内置 7z.exe。");
		StringBuilder stdout = new StringBuilder();
		int lastPercent = 0;
		Task stdoutTask = PumpSevenZipProgressAsync(process.StandardOutput, stdout, (int percent) =>
		{
			if (percent > lastPercent && percent < 100)
			{
				lastPercent = percent;
				report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("解压", resourceName, 0L, 0L, percent, 0.0, $"{message}（内置 7-Zip，{percent}%）"));
			}
		});
		Task<string> stderrTask = process.StandardError.ReadToEndAsync();
		await process.WaitForExitAsync();
		await stdoutTask;
		string text2 = stdout?.ToString();
		string text3 = text2 + "\n" + await stderrTask;
		if (process.ExitCode != 0)
		{
			string text4 = text3.Replace("\r", " ").Replace("\n", " ").Trim();
			if (text4.Length > 800)
			{
				string text5 = text4;
				int length = text5.Length;
				int num = length - 800;
				text4 = text5.Substring(num, length - num);
			}
			throw new InvalidOperationException($"内置 7-Zip 解压失败（退出码 {process.ExitCode}）：{text4}");
		}
		report(new Schedule_0020Materialize_0020Dispatch_002ENormalize("解压", resourceName, 0L, 0L, 100, 0.0, "服务器资源包解压完成（内置 7-Zip）"));
	}

	private static async Task PumpSevenZipProgressAsync(StreamReader reader, StringBuilder output, Action<int> reportPercent)
	{
		char[] buffer = new char[512];
		string parseTail = "";
		while (true)
		{
			int num = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length));
			if (num <= 0)
			{
				break;
			}
			string text = new string(buffer, 0, num);
			output.Append(text);
			string text2 = parseTail + text;
			foreach (Match item in Regex.Matches(text2, "(?<!\\d)(?<percent>\\d{1,3})%", RegexOptions.CultureInvariant))
			{
				if (int.TryParse(item.Groups["percent"].Value, out var result))
				{
					reportPercent(Math.Clamp(result, 0, 99));
				}
			}
			string text3;
			if (text2.Length <= 32)
			{
				text3 = text2;
			}
			else
			{
				int length = text2.Length;
				int num2 = length - 32;
				text3 = text2.Substring(num2, length - num2);
			}
			parseTail = text3;
		}
	}

	private static string? Acquire_0020Dispatch_0020Dispatch_002EInitialize()
	{
		string directoryName = Path.GetDirectoryName(typeof(Acquire_0020Process_0020Acquire_002EAcquire).Assembly.Location);
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return null;
		}
		string text = Directory.GetParent(directoryName)?.FullName;
		string text2 = ((text == null) ? null : Directory.GetParent(text)?.FullName);
		string[] array = new string[3]
		{
			Path.Combine(directoryName, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DA.Normalize_0020Acquire_0020Worker_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			(text2 == null) ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DE.Process_0020Validate_0020Evaluate_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform) : Path.Combine(text2, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DB.Materialize_0020Materialize_0020Dispatch_002EResolve, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DC.Normalize_0020Observe_0020Dispatch_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DD.Resolve_0020Process_0020Resolve_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			null
		};
		string text3;
		if (text != null)
		{
			_003C_003Ey__InlineArray5<string> buffer = default;
			buffer[0] = text;
			buffer[1] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000DF.Evaluate_0020Transform_0020Process_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[2] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E0.Initialize_0020Normalize_0020Acquire_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[3] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E1.Observe_0020Acquire_0020Evaluate_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[4] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E2.Schedule_0020Dispatch_0020Acquire_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			text3 = Path.Combine(buffer);
		}
		else
		{
			text3 = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E3.Transform_0020Validate_0020Dispatch_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		}
		array[2] = text3;
		return array.FirstOrDefault(File.Exists);
	}

	private static string? Process_0020Worker_0020Acquire_002EWorker()
	{
		string directoryName = Path.GetDirectoryName(typeof(Acquire_0020Process_0020Acquire_002EAcquire).Assembly.Location);
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return null;
		}
		string text = Directory.GetParent(directoryName)?.FullName;
		string text2 = ((text == null) ? null : Directory.GetParent(text)?.FullName);
		string[] array = new string[3]
		{
			Path.Combine(directoryName, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E4.Resolve_0020Finalize_0020Finalize_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			(text2 == null) ? Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E8.Observe_0020Materialize_0020Synchronize_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform) : Path.Combine(text2, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E5.Worker_0020Transform_0020Evaluate_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E6.Observe_0020Materialize_0020Synchronize_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E7.Finalize_0020Finalize_0020Worker_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)),
			null
		};
		string text3;
		if (text != null)
		{
			_003C_003Ey__InlineArray5<string> buffer = default;
			buffer[0] = text;
			buffer[1] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000E9.Synchronize_0020Validate_0020Acquire_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[2] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000EA.Resolve_0020Normalize_0020Worker_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[3] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000EB.Finalize_0020Worker_0020Synchronize_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			buffer[4] = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000EC.Normalize_0020Transform_0020Dispatch_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			text3 = Path.Combine(buffer);
		}
		else
		{
			text3 = Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000ED.Normalize_0020Evaluate_0020Observe_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
		}
		array[2] = text3;
		return array.FirstOrDefault(File.Exists);
	}

	private static string Observe_0020Dispatch_0020Observe_002EObserve(Exception Validate_0020Process_0020Normalize_002ESchedule)
	{
		string text = Validate_0020Process_0020Normalize_002ESchedule.GetBaseException().Message.Replace(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000EE.Acquire_0020Dispatch_0020Initialize_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000EF.Schedule_0020Materialize_0020Acquire_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)).Replace(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000F0.Evaluate_0020Finalize_0020Transform_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000F1.Validate_0020Evaluate_0020Observe_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform)).Trim();
		if (!string.IsNullOrWhiteSpace(text))
		{
			if (text.Length > 240)
			{
				return text.Substring(0, 240) + Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_020000F2.Synchronize_0020Dispatch_0020Initialize_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000063.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve.RecoveredType_02000062.Evaluate_0020Resolve_0020Evaluate_002ETransform);
			}
			return text;
		}
		return Validate_0020Process_0020Normalize_002ESchedule.GetType().Name;
	}

	private static async Task<long> TryGetContentLength(string url)
	{
		try
		{
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
			using HttpResponseMessage httpResponseMessage = await Validate_0020Normalize_0020Normalize_002EProcess.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			return httpResponseMessage.Content.Headers.ContentLength.GetValueOrDefault();
		}
		catch
		{
			return 0L;
		}
	}
}
