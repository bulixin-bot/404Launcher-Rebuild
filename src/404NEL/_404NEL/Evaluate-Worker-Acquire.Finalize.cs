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

internal static class Evaluate_0020Worker_0020Acquire_002EFinalize
{
	private static readonly ConcurrentDictionary<string, string> Finalize_0020Schedule_0020Validate_002EDispatch = new ConcurrentDictionary<string, string>();

	private static readonly HttpClient Finalize_0020Resolve_0020Schedule_002EValidate = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(20L)
	};

	public static void Materialize_0020Normalize_0020Transform_002EProcess(string Observe_0020Worker_0020Finalize_002ENormalize, EntityModsList? Transform_0020Synchronize_0020Evaluate_002EFinalize)
	{
		if (Transform_0020Synchronize_0020Evaluate_002EFinalize != null)
		{
			Finalize_0020Schedule_0020Validate_002EDispatch[Observe_0020Worker_0020Finalize_002ENormalize] = JsonSerializer.Serialize(Transform_0020Synchronize_0020Evaluate_002EFinalize);
		}
	}

	public static EntityModsList? Acquire_0020Worker_0020Synchronize_002EInitialize(string Materialize_0020Initialize_0020Schedule_002ESchedule, Action<Validate_0020Observe_0020Synchronize_002EEvaluate> Dispatch_0020Synchronize_0020Schedule_002EEvaluate)
	{
		if (!Finalize_0020Schedule_0020Validate_002EDispatch.TryGetValue(Materialize_0020Initialize_0020Schedule_002ESchedule, out string value))
		{
			return null;
		}
		EntityModsList? entityModsList = JsonSerializer.Deserialize<EntityModsList>(value);
		if (entityModsList != null)
		{
			Dispatch_0020Synchronize_0020Schedule_002EEvaluate(new Validate_0020Observe_0020Synchronize_002EEvaluate(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000C6.Finalize_0020Validate_0020Dispatch_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000C7.Transform_0020Synchronize_0020Transform_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), 0L, 0L, 100, 0.0, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000C8.Finalize_0020Resolve_0020Initialize_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)));
		}
		return entityModsList;
	}

	public static async Task<EntityModsList?> InstallGameMods(string userId, string userToken, EnumGameVersion gameVersion, WPFLauncher launcher, string gameId, bool isRental, Action<Validate_0020Observe_0020Synchronize_002EEvaluate> report)
	{
		report(new Validate_0020Observe_0020Synchronize_002EEvaluate("校验", "服务器资源清单", 0L, 0L, 0, 0.0, "正在获取资源清单"));
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
			report(new Validate_0020Observe_0020Synchronize_002EEvaluate("校验", displayName, 0L, 0L, 0, 0.0, $"正在校验核心模组 {index + 1}/{totalComponents}"));
			if (!File.Exists(jar) || !FileUtil.ComputeMd5FromFile(jar).Equals(entityComponentDownloadInfoResponseSub.JarMd5, StringComparison.OrdinalIgnoreCase))
			{
				await DownloadWithProgress(entityComponentDownloadInfoResponseSub.ResUrl, archive, displayName, report);
				await ExtractWithBundled7ZipAsync(archive, extractDirectory, displayName, $"正在解压核心模组 {index + 1}/{totalComponents}", report);
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("安装", displayName, 0L, 0L, 100, 0.0, $"正在安装核心模组 {index + 1}/{totalComponents}"));
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
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("完成", "服务器游戏资源", 0L, 0L, 100, 0.0, "已使用本地资源缓存"));
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
			report(new Validate_0020Observe_0020Synchronize_002EEvaluate("校验", jar, 0L, 0L, 100, 0.0, "正在校验服务器游戏资源"));
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
			report(new Validate_0020Observe_0020Synchronize_002EEvaluate("安装", jar, 0L, 0L, 100, 0.0, "正在安装服务器游戏资源"));
			modList.Mods.AddRange(serverMods.Mods);
			await File.WriteAllTextAsync(extractDirectory, component.ResMd5);
			await File.WriteAllTextAsync(archive, JsonSerializer.Serialize(serverMods));
			FileUtil.DeleteFileSafe(componentArchive);
			FileUtil.DeleteFileSafe(componentResumeMarker);
		}
		report(new Validate_0020Observe_0020Synchronize_002EEvaluate("完成", "服务器游戏资源", 0L, 0L, 100, 0.0, "游戏资源已准备完成"));
		return modList;
	}

	private static async Task DownloadWithProgress(string url, string destinationPath, string resourceName, Action<Validate_0020Observe_0020Synchronize_002EEvaluate> report)
	{
		long totalBytes = await TryGetContentLength(url);
		Exception lastError = null;
		for (int attempt = 1; attempt <= 3; attempt++)
		{
			report(new Validate_0020Observe_0020Synchronize_002EEvaluate("下载", resourceName, 0L, totalBytes, 0, 0.0, (attempt == 1) ? "正在使用 aria2 下载（8 线程）" : $"aria2 正在重试下载（{attempt}/{3}）"));
			try
			{
				await DownloadWithAria2Async(url, destinationPath, resourceName, totalBytes, report);
				long num = (File.Exists(destinationPath) ? new FileInfo(destinationPath).Length : totalBytes);
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("下载", resourceName, num, (totalBytes > 0) ? totalBytes : num, 100, 0.0, "aria2 资源下载完成"));
				return;
			}
			catch (Exception ex)
			{
				lastError = ex;
				string value = Dispatch_0020Initialize_0020Transform_002ESynchronize(ex);
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("下载", resourceName, 0L, totalBytes, 0, 0.0, (attempt < 3) ? $"下载失败（{attempt}/{3}），准备重试：{value}" : $"下载失败（{attempt}/{3}）：{value}"));
			}
			if (attempt < 3)
			{
				await Task.Delay(TimeSpan.FromSeconds(attempt));
			}
		}
		string value2 = ((lastError == null) ? "未知下载错误" : Dispatch_0020Initialize_0020Transform_002ESynchronize(lastError));
		throw new InvalidOperationException($"资源下载失败（已重试 {3} 次）：{resourceName}；原因：{value2}", lastError);
	}

	private static async Task DownloadWithAria2Async(string url, string destinationPath, string resourceName, long totalBytes, Action<Validate_0020Observe_0020Synchronize_002EEvaluate> report)
	{
		string text = Validate_0020Transform_0020Acquire_002EDispatch();
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
				if (Resolve_0020Finalize_0020Resolve_002EDispatch(data, out var Schedule_0020Initialize_0020Process_002EWorker, out var Worker_0020Normalize_0020Materialize_002EProcess, out var Materialize_0020Initialize_0020Resolve_002ENormalize, out var Resolve_0020Process_0020Synchronize_002EInitialize))
				{
					parsedProgressAt = Stopwatch.GetTimestamp();
					report(new Validate_0020Observe_0020Synchronize_002EEvaluate("下载", resourceName, Schedule_0020Initialize_0020Process_002EWorker, (Worker_0020Normalize_0020Materialize_002EProcess > 0) ? Worker_0020Normalize_0020Materialize_002EProcess : totalBytes, Materialize_0020Initialize_0020Resolve_002ENormalize, Resolve_0020Process_0020Synchronize_002EInitialize, "aria2 下载中（8 线程）"));
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
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("下载", resourceName, num, totalBytes, percent, speedBytesPerSecond, "aria2 下载中（8 线程）"));
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

	private static bool Resolve_0020Finalize_0020Resolve_002EDispatch(string Materialize_0020Process_0020Evaluate_002EAcquire, out long Schedule_0020Initialize_0020Process_002EWorker, out long Worker_0020Normalize_0020Materialize_002EProcess, out int Materialize_0020Initialize_0020Resolve_002ENormalize, out double Resolve_0020Process_0020Synchronize_002EInitialize)
	{
		Schedule_0020Initialize_0020Process_002EWorker = 0L;
		Worker_0020Normalize_0020Materialize_002EProcess = 0L;
		Materialize_0020Initialize_0020Resolve_002ENormalize = 0;
		Resolve_0020Process_0020Synchronize_002EInitialize = 0.0;
		Match match = Regex.Match(Materialize_0020Process_0020Evaluate_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000C9.Finalize_0020Synchronize_0020Normalize_002EProcess, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), RegexOptions.CultureInvariant);
		if (!match.Success)
		{
			return false;
		}
		Schedule_0020Initialize_0020Process_002EWorker = Worker_0020Dispatch_0020Evaluate_002EDispatch(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CA.Acquire_0020Process_0020Initialize_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value);
		Worker_0020Normalize_0020Materialize_002EProcess = Worker_0020Dispatch_0020Evaluate_002EDispatch(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CB.Resolve_0020Acquire_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value);
		Resolve_0020Process_0020Synchronize_002EInitialize = Worker_0020Dispatch_0020Evaluate_002EDispatch(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CC.Initialize_0020Transform_0020Dispatch_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value);
		Materialize_0020Initialize_0020Resolve_002ENormalize = (match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CD.Normalize_0020Worker_0020Initialize_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Success ? Math.Clamp(int.Parse(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CE.Materialize_0020Evaluate_0020Evaluate_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value), 0, 99) : ((Worker_0020Normalize_0020Materialize_002EProcess > 0) ? Math.Clamp((int)(Schedule_0020Initialize_0020Process_002EWorker * 100 / Worker_0020Normalize_0020Materialize_002EProcess), 0, 99) : 0));
		return true;
	}

	private static long Worker_0020Dispatch_0020Evaluate_002EDispatch(string Evaluate_0020Transform_0020Schedule_002ENormalize)
	{
		Match match = Regex.Match(Evaluate_0020Transform_0020Schedule_002ENormalize.Trim(), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000CF.Transform_0020Evaluate_0020Schedule_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		if (!match.Success || !double.TryParse(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D0.Acquire_0020Materialize_0020Acquire_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return 0L;
		}
		int num = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D1.Observe_0020Resolve_0020Validate_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate).IndexOf(match.Groups[Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D2.Observe_0020Finalize_0020Initialize_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)].Value.ToUpperInvariant(), StringComparison.Ordinal);
		if (num < 0)
		{
			num = 0;
		}
		return (long)(result * Math.Pow(1024.0, num));
	}

	private static async Task ExtractWithBundled7ZipAsync(string archivePath, string outputDirectory, string resourceName, string message, Action<Validate_0020Observe_0020Synchronize_002EEvaluate> report)
	{
		string text = Synchronize_0020Observe_0020Synchronize_002ESynchronize();
		if (text == null)
		{
			throw new FileNotFoundException("未找到内置 7z.exe，请检查 tools\\7zip 文件是否完整。");
		}
		string fullPath = Path.GetFullPath(archivePath);
		string fullPath2 = Path.GetFullPath(outputDirectory);
		Directory.CreateDirectory(fullPath2);
		report(new Validate_0020Observe_0020Synchronize_002EEvaluate("解压", resourceName, 0L, 0L, 0, 0.0, message + "（内置 7-Zip）"));
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
				report(new Validate_0020Observe_0020Synchronize_002EEvaluate("解压", resourceName, 0L, 0L, percent, 0.0, $"{message}（内置 7-Zip，{percent}%）"));
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
		report(new Validate_0020Observe_0020Synchronize_002EEvaluate("解压", resourceName, 0L, 0L, 100, 0.0, "服务器资源包解压完成（内置 7-Zip）"));
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

	private static string? Synchronize_0020Observe_0020Synchronize_002ESynchronize()
	{
		string directoryName = Path.GetDirectoryName(typeof(Evaluate_0020Worker_0020Acquire_002EFinalize).Assembly.Location);
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return null;
		}
		string text = Directory.GetParent(directoryName)?.FullName;
		string text2 = ((text == null) ? null : Directory.GetParent(text)?.FullName);
		string[] array = new string[3]
		{
			Path.Combine(directoryName, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D3.Acquire_0020Finalize_0020Materialize_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			(text2 == null) ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D7.Worker_0020Process_0020Validate_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : Path.Combine(text2, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D4.Synchronize_0020Transform_0020Transform_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D5.Initialize_0020Validate_0020Synchronize_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D6.Process_0020Materialize_0020Process_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			null
		};
		string text3;
		if (text != null)
		{
			_003C_003Ey__InlineArray5<string> buffer = default;
			buffer[0] = text;
			buffer[1] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D8.Process_0020Initialize_0020Schedule_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[2] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000D9.Evaluate_0020Normalize_0020Worker_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[3] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DA.Normalize_0020Process_0020Evaluate_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[4] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DB.Initialize_0020Initialize_0020Synchronize_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			text3 = Path.Combine(buffer);
		}
		else
		{
			text3 = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DC.Transform_0020Finalize_0020Process_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
		}
		array[2] = text3;
		return array.FirstOrDefault(File.Exists);
	}

	private static string? Validate_0020Transform_0020Acquire_002EDispatch()
	{
		string directoryName = Path.GetDirectoryName(typeof(Evaluate_0020Worker_0020Acquire_002EFinalize).Assembly.Location);
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return null;
		}
		string text = Directory.GetParent(directoryName)?.FullName;
		string text2 = ((text == null) ? null : Directory.GetParent(text)?.FullName);
		string[] array = new string[3]
		{
			Path.Combine(directoryName, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DD.Transform_0020Materialize_0020Acquire_002EProcess, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			(text2 == null) ? Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E1.Worker_0020Transform_0020Validate_002ETransform, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) : Path.Combine(text2, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DE.Process_0020Process_0020Worker_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000DF.Validate_0020Dispatch_0020Evaluate_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E0.Finalize_0020Finalize_0020Worker_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)),
			null
		};
		string text3;
		if (text != null)
		{
			_003C_003Ey__InlineArray5<string> buffer = default;
			buffer[0] = text;
			buffer[1] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E2.Transform_0020Normalize_0020Acquire_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[2] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E3.Validate_0020Acquire_0020Evaluate_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[3] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E4.Synchronize_0020Worker_0020Resolve_002EValidate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			buffer[4] = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E5.Process_0020Finalize_0020Synchronize_002EValidate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			text3 = Path.Combine(buffer);
		}
		else
		{
			text3 = Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E6.Schedule_0020Transform_0020Acquire_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
		}
		array[2] = text3;
		return array.FirstOrDefault(File.Exists);
	}

	private static string Dispatch_0020Initialize_0020Transform_002ESynchronize(Exception Finalize_0020Synchronize_0020Acquire_002EFinalize)
	{
		string text = Finalize_0020Synchronize_0020Acquire_002EFinalize.GetBaseException().Message.Replace(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E7.Observe_0020Synchronize_0020Process_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E8.Observe_0020Acquire_0020Validate_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)).Replace(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000E9.Resolve_0020Validate_0020Evaluate_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000EA.Validate_0020Initialize_0020Dispatch_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)).Trim();
		if (!string.IsNullOrWhiteSpace(text))
		{
			if (text.Length > 240)
			{
				return text.Substring(0, 240) + Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_020000EB.Observe_0020Process_0020Initialize_002ESynchronize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate);
			}
			return text;
		}
		return Finalize_0020Synchronize_0020Acquire_002EFinalize.GetType().Name;
	}

	private static async Task<long> TryGetContentLength(string url)
	{
		try
		{
			using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url);
			using HttpResponseMessage httpResponseMessage = await Finalize_0020Resolve_0020Schedule_002EValidate.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
			return httpResponseMessage.Content.Headers.ContentLength.GetValueOrDefault();
		}
		catch
		{
			return 0L;
		}
	}
}
