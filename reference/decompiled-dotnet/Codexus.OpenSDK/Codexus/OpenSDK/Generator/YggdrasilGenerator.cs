using System;
using System.IO;
using System.Text;
using Codexus.OpenSDK.Cipher;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Codexus.OpenSDK.Extensions;
using Org.BouncyCastle.Crypto;

namespace Codexus.OpenSDK.Generator;

public class YggdrasilGenerator(YggdrasilData data)
{
	private const string PrcCheck = "[]";

	private const int ClientKeyLength = 19;

	private const int CheckSumLength = 32;

	private static readonly byte[] McVersionSalt = new byte[9] { 1, 0, 4, 128, 210, 62, 247, 17, 1 };

	private static readonly byte[] TcpSalt = new byte[16]
	{
		47, 132, 174, 163, 153, 33, 41, 38, 218, 191,
		149, 163, 171, 175, 55, 224
	};

	private static readonly AsymmetricKeyParameter PublicKey = Rsa.LoadPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4HJFrYdVTeoSvH6qsnJElfXuf7FnxxFQdz3gRCs66LDrZfaoGoWt2e/aGIOv8uGHliBWnZP42Ike9Qf5aiYVtQ4mlj2bXZjifHG35LlS1Bq6yCA6k1WevWcrGWOuLzny3jo8Wbdi0lIFMTT2hN98sF2k4YcvyE9zhqxfRNFGVI5kLyxm9CeTKAXGBU5mw3yQWJ8cPRR4866jpGGOhBWlJdilWt2NES9bid8SbhTT55wqumnVO5J5/DaMyTgKIQngH7NyZQljAhdK5I23dzpGop322n2eQ+mTNLuquwU453o1cbyQobgC6vh5/F1QT2INBR2qYCnRzzJ6hrhE5kIMZwIDAQAB");

	private static readonly AsymmetricKeyParameter PrivateKey = Rsa.LoadPrivateKey("MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQDobJGIddxcs08xzTIVFanc4/84J1DbxiW7wLokIrap3txzyyMXj+AcDa8jLopLJkMg9rZLzL50Dwp+hmTTgiIcjkM1DVREsRBltzqjVNyiYPL2VGHLn+/eEivjhLNWUMcWrlAoJ5JJBi22oGPLlKDVJpg33JPI5nPZpwufdB0ecn2V0CAeeGyswQyAaqIjoiYOoP3HipjEYGQHp1RsADf4ozGRgv+2HGiSOyhlv00ixnF0nRUTzVh18ka0N3LdLMpAN/YAPkO8tmoWp0asyU3X+nyZFd29povvrRy4rL1lYmo8jdpfpSL+Yk+9+RybjBAhXRx6uDoBxaa2kwE4fyrrAgMBAAECggEAWT42szruHfoLkofDjyz+R/6TZLBT788pdeoOjwl1McyMwTlihA2Oc7cdZFjeaPSMGgAhBwHarx2HXgWkeUIibuyBCcHQdX+3WBb+wPA4t3CaWdMUqecDZzV6/KVbZu0lRKQxyvlGxhtFOjZjmyu6hZ2IHQrpA97Y5N2rLNKcy69W+QYJZappmBfbVgWM0NRmgmpg6siQ0Cm6Ryil3SBAPBVv+EUPiD9jdXbtVq7VwN4YmwUGScp2Fib1oUnqEAja1hfihVnRFQ246nKXhIc/YVrNmwBrxAVwaaPFRka6XjKkSF0WVbpqQLXhbY1fS8FoXGpVhiF6o4rTQJbpQxhQgQKBgQD7XysfPNt9G63gTrkZvjvEk5LKsRG42MAYEkuzxEal9E/AQv0jJrk0f1WO09hCcYQeaOQXhM9mezNQv5jXEnmepXqM3NTw1Di5yh3uvjSXQjdUt+7haNw+QjggBqZxyQZjtadYairSzfmWe7OwJIkCmdgaJKxm4qMExk9kUgApGwKBgQDstBdJHU/KEBqVpsIlu185vFFuaAvxiHjXHqGwytJMQ/5aVqaphIiQCAxaEogPSzPSm28UHVQiZeFO059EKOpSJscW4pV95Dr5BAbHuYecacqnZKbQqb69//Cfpne9tGYXlmP6QnYfPoc4wYfTfPyU2x3KtDhVxtEDutpSNU1ycQKBgBabWn92s66uvJZ9vfvotetZ8ku0XQmoxK3lh1Vlg40NSdbar3Vn2CQ2h3VO7BYdq2oouMq8sQJgdh7+/DnreXChJUJh4ey+yVM8MDD2fjhURjGiUSOIkLYwsmd+8Z0uHRr+jUxQUAWhbJ7yBRkEUCYhu+OuBKtEGrElPKKjFUydAoGAQLj9pQBe0OGWY1U1wRt67k6P9aB9o42tfSTjEXRkDHaLFiibab7TmI6a0gY/Le9iPDREKzvZxY4WDXfQFNMbP1tbFObf+Yxuk6iGMhaI/jvvLdZXxrajcVCKex0JoNWzFMAKlmOV6PUwBFTmzu1eI1XGz6Z3wPycKmjtSY1JoAECgYBBOfaUDMaG1xLzv+q1jPPs2U4lXPK2BXFE5RaliUGC+LIQREXPishII2LYFW3gtXj5QWfIGq6x0d6ca6Bja2vYRDDe5tlT/2VbZahiHpb2PYL/2WgeoHl7sT9Bb/nsKyo85Sv+doop6huy4+aeTiQHgrGR9JYMVBSIx6P8Tt5phA==");

	public YggdrasilData Data { get; } = data;

	public byte[] GenerateJoinMessage(GameProfile profile, string serverId, byte[] loginSeed)
	{
		int num = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		byte[] data = BuildHashData(profile, num, profile.User.UserId, loginSeed);
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteLong(long.Parse(profile.GameId));
		memoryStream.WriteString(serverId);
		memoryStream.WriteString(Data.LauncherVersion);
		memoryStream.WriteString(profile.GameVersion);
		memoryStream.WriteInt(num);
		memoryStream.WriteBytes(data);
		string modInfo = profile.GetModInfo();
		memoryStream.WriteShortString(modInfo);
		memoryStream.WriteShortString("[]");
		memoryStream.WriteShort(0);
		memoryStream.WriteByteLengthString(Data.Channel);
		return memoryStream.ToArray();
	}

	public byte[] GenerateInitializeMessage(GameProfile profile, byte[] loginSeed, byte[] signContent)
	{
		int authId = profile.User.GetAuthId();
		byte[] authToken = profile.User.GetAuthToken();
		byte[] array = AesNoPadding.Encrypt(loginSeed, authToken);
		byte[] second = BuildSign(profile, authId, array).EncodeSha256();
		byte[] array2 = Rsa.RsaWithPkcs1(PublicKey, signContent, forEncryption: false);
		if (array2.Length < 51)
		{
			throw new ArgumentException("Invalid sign content length");
		}
		byte[] content = array2.AsSpan(0, 19).ToArray();
		if (!array2.AsSpan(19, 32).ToArray().SequenceEqual(loginSeed.EncodeSha256()))
		{
			throw new InvalidOperationException("CheckSum validation failed");
		}
		byte[] data = Rsa.RsaWithPkcs1(PrivateKey, content.CombineWith(second), forEncryption: true);
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteInt(authId);
		memoryStream.WriteBytes(array);
		memoryStream.WriteShortString(Data.LauncherVersion, littleEndian: false);
		memoryStream.WriteByteLengthString(Data.Channel);
		memoryStream.WriteBytes(TcpSalt);
		memoryStream.WriteShortBytes(data);
		memoryStream.WriteByteLengthString(profile.GameVersion);
		memoryStream.WriteBytes(McVersionSalt);
		using MemoryStream memoryStream2 = new MemoryStream();
		memoryStream2.WriteShort((int)memoryStream.Length);
		memoryStream2.Write(memoryStream.ToArray());
		return memoryStream2.ToArray();
	}

	private byte[] BuildSign(GameProfile profile, int id, byte[] seed)
	{
		using MemoryStream memoryStream = new MemoryStream();
		Encoding uTF = Encoding.UTF8;
		memoryStream.WriteInt(id);
		memoryStream.WriteBytes(seed);
		memoryStream.WriteBytes(uTF.GetBytes(Data.LauncherVersion));
		memoryStream.WriteBytes(uTF.GetBytes(Data.Channel));
		memoryStream.WriteBytes(uTF.GetBytes(Data.CrcSalt));
		memoryStream.WriteBytes(uTF.GetBytes(profile.GameVersion));
		memoryStream.WriteBytes(McVersionSalt);
		return memoryStream.ToArray();
	}

	private byte[] BuildHashData(GameProfile profile, int time, int id, byte[] loginSeed)
	{
		string s = $"{Data.LauncherVersion}{profile.GameVersion}{time}{Data.CrcSalt}{profile.GetModInfo()}{profile.BootstrapMd5}{profile.DatFileMd5}{"[]"}";
		return Encoding.UTF8.GetBytes(s).CombineWith(id.ToByteArray()).CombineWith(loginSeed)
			.EncodeSha256();
	}
}
