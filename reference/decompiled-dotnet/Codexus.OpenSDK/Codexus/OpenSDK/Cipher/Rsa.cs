using System;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace Codexus.OpenSDK.Cipher;

public static class Rsa
{
	public static AsymmetricKeyParameter LoadPublicKey(string base64PublicKey)
	{
		return PublicKeyFactory.CreateKey(Convert.FromBase64String(base64PublicKey));
	}

	public static AsymmetricKeyParameter LoadPrivateKey(string base64PrivateKey)
	{
		return PrivateKeyFactory.CreateKey(Convert.FromBase64String(base64PrivateKey));
	}

	public static byte[] RsaWithPkcs1(AsymmetricKeyParameter key, byte[] data, bool forEncryption)
	{
		Pkcs1Encoding pkcs1Encoding = new Pkcs1Encoding(new RsaEngine());
		pkcs1Encoding.Init(forEncryption, key);
		int inputBlockSize = pkcs1Encoding.GetInputBlockSize();
		using MemoryStream memoryStream = new MemoryStream();
		int num;
		for (int i = 0; i < data.Length; i += num)
		{
			num = Math.Min(inputBlockSize, data.Length - i);
			byte[] array = pkcs1Encoding.ProcessBlock(data, i, num);
			memoryStream.Write(array, 0, array.Length);
		}
		return memoryStream.ToArray();
	}
}
