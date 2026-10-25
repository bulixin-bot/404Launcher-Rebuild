using System.Security.Cryptography;

namespace Codexus.OpenSDK.Cipher;

public static class AesNoPadding
{
	public static byte[] Encrypt(byte[] data, byte[] key, bool encryption = true)
	{
		using Aes aes = Aes.Create();
		aes.Key = key;
		aes.Mode = CipherMode.ECB;
		aes.Padding = PaddingMode.None;
		return (encryption ? aes.CreateEncryptor() : aes.CreateDecryptor()).TransformFinalBlock(data, 0, data.Length);
	}
}
