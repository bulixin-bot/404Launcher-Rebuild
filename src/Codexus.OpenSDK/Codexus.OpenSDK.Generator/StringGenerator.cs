using System;
using System.Text;

namespace Codexus.OpenSDK.Generator;

public static class StringGenerator
{
	private const string Numbers = "0123456789";

	private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";

	private static readonly Random Random = new Random();

	public static string GenerateHexString(int length)
	{
		byte[] array = new byte[length];
		Random.NextBytes(array);
		return BitConverter.ToString(array);
	}

	public static string GenerateRandomString(int length, bool includeNumbers = true, bool includeUppercase = true, bool includeLowercase = true)
	{
		if (length <= 0)
		{
			throw new ArgumentException("Length must be greater than 0", "length");
		}
		if (!includeNumbers && !includeUppercase && !includeLowercase)
		{
			throw new ArgumentException("Must include at least one character type", "includeNumbers, includeUppercase, includeLowercase");
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (includeNumbers)
		{
			stringBuilder.Append("0123456789");
		}
		if (includeUppercase)
		{
			stringBuilder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
		}
		if (includeLowercase)
		{
			stringBuilder.Append("abcdefghijklmnopqrstuvwxyz");
		}
		int length2 = stringBuilder.Length;
		StringBuilder stringBuilder2 = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			stringBuilder2.Append(stringBuilder[Random.Next(length2)]);
		}
		return stringBuilder2.ToString();
	}

	public static string GenerateRandomMacAddress(string separator = ":", bool uppercase = true)
	{
		byte[] array = new byte[6];
		Random.NextBytes(array);
		array[0] = (byte)(array[0] & 0xFE);
		array[0] = (byte)(array[0] | 2);
		string text = (uppercase ? "X2" : "x2");
		_003C_003Ey__InlineArray6<string> buffer = default;
		buffer[0] = array[0].ToString(text);
		buffer[1] = array[1].ToString(text);
		buffer[2] = array[2].ToString(text);
		buffer[3] = array[3].ToString(text);
		buffer[4] = array[4].ToString(text);
		buffer[5] = array[5].ToString(text);
		return string.Join(separator, (ReadOnlySpan<string?>)buffer);
	}
}
