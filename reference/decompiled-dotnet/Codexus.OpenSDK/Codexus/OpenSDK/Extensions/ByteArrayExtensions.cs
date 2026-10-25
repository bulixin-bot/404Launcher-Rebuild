using System;
using System.Linq;

namespace Codexus.OpenSDK.Extensions;

public static class ByteArrayExtensions
{
	extension(byte[] content)
	{
		public byte[] Xor(byte[] key)
		{
			if (content.Length != key.Length)
			{
				throw new ArgumentException("Key length must be equal to content length.");
			}
			byte[] array = new byte[content.Length];
			for (int i = 0; i < content.Length; i++)
			{
				array[i] = (byte)(content[i] ^ key[i]);
			}
			return array;
		}

		public byte[] CombineWith(byte[] second)
		{
			return content.Concat(second).ToArray();
		}
	}
}
