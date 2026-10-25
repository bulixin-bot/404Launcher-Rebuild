namespace Codexus.OpenSDK.Cipher;

public static class Skip32Cipher
{
	private static readonly byte[] FTable = new byte[256]
	{
		163, 215, 9, 131, 248, 72, 246, 244, 179, 33,
		21, 120, 153, 177, 175, 249, 231, 45, 77, 138,
		206, 76, 202, 46, 82, 149, 217, 30, 78, 56,
		68, 40, 10, 223, 2, 160, 23, 241, 96, 104,
		18, 183, 122, 195, 233, 250, 61, 83, 150, 132,
		107, 186, 242, 99, 154, 25, 124, 174, 229, 245,
		247, 22, 106, 162, 57, 182, 123, 15, 193, 147,
		129, 27, 238, 180, 26, 234, 208, 145, 47, 184,
		85, 185, 218, 133, 63, 65, 191, 224, 90, 88,
		128, 95, 102, 11, 216, 144, 53, 213, 192, 167,
		51, 6, 101, 105, 69, 0, 148, 86, 109, 152,
		155, 118, 151, 252, 178, 194, 176, 254, 219, 32,
		225, 235, 214, 228, 221, 71, 74, 29, 66, 237,
		158, 110, 73, 60, 205, 67, 39, 210, 7, 212,
		222, 199, 103, 24, 137, 203, 48, 31, 141, 198,
		143, 170, 200, 116, 220, 201, 93, 92, 49, 164,
		112, 136, 97, 44, 159, 13, 43, 135, 80, 130,
		84, 100, 38, 125, 3, 64, 52, 75, 28, 115,
		209, 196, 253, 59, 204, 251, 127, 171, 230, 62,
		91, 165, 173, 4, 35, 156, 20, 81, 34, 240,
		41, 121, 113, 126, 255, 140, 14, 226, 12, 239,
		188, 114, 117, 111, 55, 161, 236, 211, 142, 98,
		139, 134, 16, 232, 8, 119, 17, 190, 146, 79,
		36, 197, 50, 54, 157, 207, 243, 166, 187, 172,
		94, 108, 169, 19, 87, 37, 181, 227, 189, 168,
		58, 1, 5, 89, 42, 70
	};

	private static int G(byte[] key, int k, int w)
	{
		int num = w >> 8;
		int num2 = w & 0xFF;
		int num3 = FTable[num2 ^ (key[4 * k % 10] & 0xFF)] ^ num;
		int num4 = FTable[num3 ^ (key[(4 * k + 1) % 10] & 0xFF)] ^ num2;
		int num5 = FTable[num4 ^ (key[(4 * k + 2) % 10] & 0xFF)] ^ num3;
		int num6 = FTable[num5 ^ (key[(4 * k + 3) % 10] & 0xFF)] ^ num4;
		return (num5 << 8) + num6;
	}

	private static void Skip32(byte[] key, int[] buf, bool encrypt)
	{
		int num;
		int num2;
		if (encrypt)
		{
			num = 1;
			num2 = 0;
		}
		else
		{
			num = -1;
			num2 = 23;
		}
		int num3 = (buf[0] << 8) + buf[1];
		int num4 = (buf[2] << 8) + buf[3];
		for (int i = 0; i < 12; i++)
		{
			num4 ^= G(key, num2, num3) ^ num2;
			num2 += num;
			num3 ^= G(key, num2, num4) ^ num2;
			num2 += num;
		}
		buf[0] = num4 >> 8;
		buf[1] = num4 & 0xFF;
		buf[2] = num3 >> 8;
		buf[3] = num3 & 0xFF;
	}

	public static uint Encrypt(uint value, byte[] key)
	{
		return (uint)Encrypt((int)value, key);
	}

	public static int Encrypt(int value, byte[] key)
	{
		int[] array = new int[4]
		{
			(value >> 24) & 0xFF,
			(value >> 16) & 0xFF,
			(value >> 8) & 0xFF,
			value & 0xFF
		};
		Skip32(key, array, encrypt: true);
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}

	public static uint Decrypt(uint value, byte[] key)
	{
		return (uint)Decrypt((int)value, key);
	}

	public static int Decrypt(int value, byte[] key)
	{
		int[] array = new int[4]
		{
			(value >> 24) & 0xFF,
			(value >> 16) & 0xFF,
			(value >> 8) & 0xFF,
			value & 0xFF
		};
		Skip32(key, array, encrypt: false);
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}
}
