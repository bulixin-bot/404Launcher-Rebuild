using System;
using System.Security.Cryptography;
using System.Text;

namespace Codexus.OpenSDK.Extensions;

public static class MPayExtensions
{
	extension(byte[] input)
	{
		public string EncodeMd5()
		{
			return MD5.HashData(input).EncodeHex();
		}

		public string EncodeHex()
		{
			return Convert.ToHexString(input).Replace("-", "").ToLower();
		}
	}

	extension(string input)
	{
		public byte[] AesEncrypt(byte[] key)
		{
			using Aes aes = Aes.Create();
			aes.Key = key;
			aes.Mode = CipherMode.ECB;
			aes.Padding = PaddingMode.PKCS7;
			using ICryptoTransform cryptoTransform = aes.CreateEncryptor();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			return cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
		}

		public byte[] DecodeHex()
		{
			return Convert.FromHexString(input);
		}

		public string EncodeMd5()
		{
			return Encoding.UTF8.GetBytes(input).EncodeMd5();
		}

		public string EncodeBase64()
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
		}
	}
}
