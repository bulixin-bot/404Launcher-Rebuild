using System.Security.Cryptography;
using System.Text;

namespace Codexus.Cipher.Utils;

public class RandomUtil
{
	public static string GetRandomString(int length, string? chars = null)
	{
		if (length <= 0)
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(chars))
		{
			chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghizklmnopqrstuvwxyz0123456789";
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		byte[] array = new byte[length];
		RandomNumberGenerator.Fill(array);
		for (int i = 0; i < length; i++)
		{
			int index = array[i] % chars.Length;
			stringBuilder.Append(chars[index]);
		}
		return stringBuilder.ToString();
	}

	public static string GenerateSessionId()
	{
		return "captchaReq" + GetRandomString(16);
	}
}
