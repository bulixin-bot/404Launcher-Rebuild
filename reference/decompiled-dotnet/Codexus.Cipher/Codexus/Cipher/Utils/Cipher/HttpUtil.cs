using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Codexus.Cipher.Utils.Cipher;

public static class HttpUtil
{
	private const string SKeys = "MK6mipwmOUedplb6,OtEylfId6dyhrfdn,VNbhn5mvUaQaeOo9,bIEoQGQYjKd02U0J,fuaJrPwaH2cfXXLP,LEkdyiroouKQ4XN1,jM1h27H4UROu427W,DhReQada7gZybTDk,ZGXfpSTYUvcdKqdY,AZwKf7MWZrJpGR5W,amuvbcHw38TcSyPU,SI4QotspbjhyFdT0,VP4dhjKnDGlSJtbB,UXDZx4KhZywQ2tcn,NIK73ZNvNqzva4kd,WeiW7qU766Q1YQZI";

	private static Aes Aes
	{
		get
		{
			Aes aes = System.Security.Cryptography.Aes.Create();
			aes.Padding = PaddingMode.None;
			return aes;
		}
	}

	private static byte[][] HttpKeys => (from skey in "MK6mipwmOUedplb6,OtEylfId6dyhrfdn,VNbhn5mvUaQaeOo9,bIEoQGQYjKd02U0J,fuaJrPwaH2cfXXLP,LEkdyiroouKQ4XN1,jM1h27H4UROu427W,DhReQada7gZybTDk,ZGXfpSTYUvcdKqdY,AZwKf7MWZrJpGR5W,amuvbcHw38TcSyPU,SI4QotspbjhyFdT0,VP4dhjKnDGlSJtbB,UXDZx4KhZywQ2tcn,NIK73ZNvNqzva4kd,WeiW7qU766Q1YQZI".Split(',')
		select Encoding.GetEncoding("us-ascii").GetBytes(skey)).ToArray();

	public static byte[] HttpEncrypt(byte[] bodyIn)
	{
		byte[] array = new byte[(int)Math.Ceiling((double)(bodyIn.Length + 16) / 16.0) * 16];
		Array.Copy(bodyIn, array, bodyIn.Length);
		byte[] bytes = Encoding.ASCII.GetBytes(StringGenerator.GenerateRandomString(16, includeNumbers: false));
		for (int i = 0; i < bytes.Length; i++)
		{
			array[i + bodyIn.Length] = bytes[i];
		}
		byte b = (byte)((Random.Shared.Next(0, HttpKeys.Length - 1) << 4) | 2);
		byte[] array2 = Aes.CreateEncryptor(HttpKeys[(b >> 4) & 0xF], bytes).TransformFinalBlock(array, 0, array.Length);
		byte[] array3 = new byte[16 + array2.Length + 1];
		Array.Copy(bytes, array3, 16);
		Array.Copy(array2, 0, array3, 16, array2.Length);
		array3[^1] = b;
		return array3;
	}

	public static byte[]? HttpDecrypt(byte[] body)
	{
		if (body.Length < 18)
		{
			return null;
		}
		byte[] array = body.Skip(16).Take(body.Length - 1 - 16).ToArray();
		byte[] array2 = Aes.CreateDecryptor(HttpKeys[(body[^1] >> 4) & 0xF], body.Take(16).ToArray()).TransformFinalBlock(array, 0, array.Length);
		int num = 0;
		int num2 = array2.Length - 1;
		while (num < 16)
		{
			if (array2[num2--] != 0)
			{
				num++;
			}
		}
		return array2.Take(num2 + 1).ToArray();
	}
}
