using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Initialize_0020Finalize_0020Finalize_002ETransform
{
	internal static string Schedule_0020Acquire_0020Transform_002ESynchronize(byte[] P_0, byte[] P_1, byte[] P_2)
	{
		byte[] bytes = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.KeySize = 256;
			rijndaelManaged.BlockSize = 128;
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(P_2, P_1, 1000);
			rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
			rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
			rijndaelManaged.Mode = CipherMode.CBC;
			using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cryptoStream.Write(P_0, 0, P_0.Length);
				cryptoStream.Close();
			}
			bytes = memoryStream.ToArray();
			rfc2898DeriveBytes.Dispose();
		}
		return Encoding.UTF8.GetString(bytes);
	}
}
