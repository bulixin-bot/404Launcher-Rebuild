using System;
using System.IO;

namespace Downloader.Extensions.Helpers;

internal static class FileHelper
{
	public static Stream CreateFile(string filename)
	{
		string directoryName = Path.GetDirectoryName(filename);
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return Stream.Null;
		}
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		return new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
	}

	public static string GetTempFile(string baseDirectory, string fileExtension)
	{
		if (string.IsNullOrWhiteSpace(baseDirectory))
		{
			baseDirectory = Path.GetTempPath();
		}
		string text = Path.Combine(baseDirectory, Guid.NewGuid().ToString("N") + fileExtension);
		CreateFile(text).Dispose();
		return text;
	}

	public static long GetAvailableFreeSpaceOnDisk(string directory)
	{
		try
		{
			DriveInfo driveInfo = new DriveInfo(directory);
			return driveInfo.IsReady ? driveInfo.AvailableFreeSpace : 0;
		}
		catch (ArgumentException)
		{
			return 0L;
		}
	}

	public static void ThrowIfNotEnoughSpace(long actualNeededSize, params string[] directories)
	{
		if (directories == null)
		{
			return;
		}
		foreach (string text in directories)
		{
			long availableFreeSpaceOnDisk = GetAvailableFreeSpaceOnDisk(text);
			if (availableFreeSpaceOnDisk > 0 && availableFreeSpaceOnDisk < actualNeededSize)
			{
				throw new IOException($"There is not enough space on the disk `{text}` with {availableFreeSpaceOnDisk} bytes");
			}
		}
	}
}
