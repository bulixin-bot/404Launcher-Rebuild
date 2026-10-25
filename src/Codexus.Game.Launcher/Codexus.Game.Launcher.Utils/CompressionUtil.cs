using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;

namespace Codexus.Game.Launcher.Utils;

public static class CompressionUtil
{
	public static bool Extract7Z(string filePath, string outPath, Action<int> progressAction)
	{
		try
		{
			using SevenZipArchive sevenZipArchive = SevenZipArchive.Open(filePath);
			sevenZipArchive.ExtractToDirectory(outPath, (double dp) =>
			{
				progressAction((int)(dp * 100.0));
			});
			return true;
		}
		catch
		{
			return ExtractZip(filePath, outPath, progressAction);
		}
	}

	private static bool ExtractZip(string filePath, string outPath, Action<int> progressAction)
	{
		try
		{
			using ZipArchive zipArchive = ZipArchive.Open(filePath);
			zipArchive.ExtractToDirectory(outPath, (double dp) =>
			{
				progressAction((int)(dp * 100.0));
			});
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static async Task Extract7ZAsync(string archivePath, string outputDir, Action<int>? progress = null)
	{
		await Task.Run(() =>
		{
			using IArchive archive = ArchiveFactory.Open(archivePath);
			int num = archive.Entries.Count();
			int num2 = 0;
			foreach (IArchiveEntry entry in archive.Entries)
			{
				if (entry != null && !entry.IsDirectory && entry.Key != null)
				{
					string path = Path.Combine(outputDir, entry.Key);
					string directoryName = Path.GetDirectoryName(path);
					if (directoryName == null)
					{
						throw new ArgumentException("Invalid directory name");
					}
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					using Stream stream = entry.OpenEntryStream();
					using FileStream destination = File.Create(path);
					stream.CopyTo(destination);
				}
				num2++;
				progress?.Invoke((int)((double)num2 / (double)num * 100.0));
			}
		});
	}
}
