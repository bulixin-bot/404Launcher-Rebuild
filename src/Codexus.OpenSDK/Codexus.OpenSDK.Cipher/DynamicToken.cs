using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Codexus.OpenSDK.Extensions;

namespace Codexus.OpenSDK.Cipher;

public static class DynamicToken
{
	private const string TokenSalt = "0eGsBkhl";

	private static readonly Aes Aes;

	static DynamicToken()
	{
		Aes = System.Security.Cryptography.Aes.Create();
		Aes.Mode = CipherMode.CBC;
		Aes.Padding = PaddingMode.Zeros;
		Aes.KeySize = 128;
		Aes.BlockSize = 128;
		Aes.Key = "debbde3548928fab"u8.ToArray();
		Aes.IV = "afd4c5c5a7c456a1"u8.ToArray();
	}

	public static Dictionary<string, string> Compute(string requestPath, string sendBody, string userId, string userToken)
	{
		return Compute(requestPath, Encoding.UTF8.GetBytes(sendBody), userId, userToken);
	}

	private static Dictionary<string, string> Compute(string requestPath, byte[] sendBody, string userId, string userToken)
	{
		requestPath = (requestPath.StartsWith('/') ? requestPath : ("/" + requestPath));
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(Encoding.UTF8.GetBytes(userToken.EncodeMd5().ToLowerInvariant()));
		memoryStream.Write(sendBody);
		memoryStream.Write(Encoding.UTF8.GetBytes("0eGsBkhl"));
		memoryStream.Write(Encoding.UTF8.GetBytes(requestPath));
		string text = memoryStream.ToArray().EncodeMd5().ToLowerInvariant();
		string text2 = HexToBinary(text);
		string text3 = text2;
		text2 = text3.Substring(6, text3.Length - 6) + text2.Substring(0, 6);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		ProcessBinaryBlock(text2, bytes);
		string value = (Convert.ToBase64String(bytes, 0, 12) + "1").Replace('+', 'm').Replace('/', 'o');
		return new Dictionary<string, string>
		{
			["user-id"] = userId,
			["user-token"] = value
		};
	}

	private static void ProcessBinaryBlock(string secretBin, byte[] httpToken)
	{
		for (int i = 0; i < secretBin.Length / 8; i++)
		{
			ReadOnlySpan<char> readOnlySpan = secretBin.AsSpan(i * 8, Math.Min(8, secretBin.Length - i * 8));
			byte b = 0;
			for (int j = 0; j < readOnlySpan.Length; j++)
			{
				if (readOnlySpan[7 - j] == '1')
				{
					b |= (byte)(1 << j);
				}
			}
			httpToken[i] ^= b;
		}
	}

	private static string HexToBinary(string hexString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in hexString.Select((char hex) => Convert.ToString(hex, 2).PadLeft(8, '0')))
		{
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}
}
