using System;
using System.IO;
using System.IO.Hashing;
using System.Security.Cryptography;
using System.Text;
using Codexus.OpenSDK.Cipher;

namespace Codexus.OpenSDK.Extensions;

public static class YggdrasilExtensions
{
	extension(byte[] input)
	{
		public byte[] EncodeSha256()
		{
			return SHA256.HashData(input);
		}
	}

	extension(int value)
	{
		public byte[] ToByteArray(bool littleEndian = true)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian != littleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public byte[] ToShortByteArray(bool littleEndian = true)
		{
			byte[] bytes = BitConverter.GetBytes((short)value);
			if (BitConverter.IsLittleEndian != littleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
	}

	extension(long value)
	{
		public byte[] ToByteArray(bool littleEndian = true)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian != littleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		public byte[] ToShortByteArray(bool littleEndian = true)
		{
			byte[] bytes = BitConverter.GetBytes((short)value);
			if (BitConverter.IsLittleEndian != littleEndian)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}
	}

	extension(ChaChaPacker packer)
	{
		public byte[] PackMessage(byte type, byte[] data)
		{
			byte[] array = new byte[data.Length + 10];
			Array.Copy(BitConverter.GetBytes((short)(array.Length - 2)), 0, array, 0, 2);
			array[6] = type;
			array[7] = 136;
			array[8] = 136;
			array[9] = 136;
			Array.Copy(data, 0, array, 10, data.Length);
			Array.Copy(Crc32.Hash(array.AsSpan(6)), 0, array, 2, 4);
			packer.ProcessBytes(array, 2, array.Length - 2, array, 2);
			return array;
		}

		public (byte, byte[]) UnpackMessage(byte[] data)
		{
			packer.ProcessBytes(data, 0, data.Length, data, 0);
			byte[] array = new byte[4];
			Crc32.Hash(data.AsSpan(4, data.Length - 4), array);
			for (int i = 0; i < 4; i++)
			{
				if (array[i] != data[i])
				{
					throw new Exception("Unpacking failed");
				}
			}
			byte[] array2 = new byte[data.Length - 8];
			Array.Copy(data, 8, array2, 0, array2.Length);
			return (data[4], array2);
		}
	}

	extension(MemoryStream stream)
	{
		public void WriteInt(int value, bool littleEndian = true)
		{
			byte[] array = value.ToByteArray(littleEndian);
			stream.Write(array);
		}

		public void WriteShort(int value, bool littleEndian = true)
		{
			byte[] array = value.ToShortByteArray(littleEndian);
			stream.Write(array);
		}

		public void WriteLong(long value, bool littleEndian = true)
		{
			byte[] array = value.ToByteArray(littleEndian);
			stream.Write(array);
		}

		public void WriteBytes(byte[] data)
		{
			stream.Write(data);
		}

		public void WriteString(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			stream.WriteByte((byte)bytes.Length);
			stream.Write(bytes);
		}

		public void WriteByteLengthString(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			stream.WriteByte((byte)bytes.Length);
			stream.Write(bytes);
		}

		public void WriteShortString(string value, bool littleEndian = true)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			stream.WriteShort(bytes.Length, littleEndian);
			stream.Write(bytes);
		}

		public void WriteShortBytes(byte[] data, bool littleEndian = true)
		{
			stream.WriteShort(data.Length, littleEndian);
			stream.Write(data);
		}
	}
}
