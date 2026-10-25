using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Threading.Tasks;
using Codexus.OpenSDK.Cipher;
using Codexus.OpenSDK.Entities;
using Codexus.OpenSDK.Entities.Yggdrasil;
using Codexus.OpenSDK.Extensions;
using Codexus.OpenSDK.Generator;
using Codexus.OpenSDK.Http;

namespace Codexus.OpenSDK.Yggdrasil;

public class StandardYggdrasil(YggdrasilData data, string address, int port)
{
	private static readonly byte[] ChaChaNonce = "163 NetEase\n"u8.ToArray();

	private readonly YggdrasilGenerator _generator = new YggdrasilGenerator(data);

	private string Address { get; } = address;

	private int Port { get; } = port;

	public YggdrasilData Data => _generator.Data;

	public StandardYggdrasil(YggdrasilData data, string address)
		: this(data, ParseAddress(address).address, ParseAddress(address).port)
	{
	}

	public StandardYggdrasil(YggdrasilData data)
		: this(data, RandomAuthServer())
	{
	}

	public async Task<Result> JoinServerAsync(GameProfile profile, string serverId, bool login = false)
	{
		using TcpClient client = new TcpClient();
		_ = 3;
		try
		{
			await client.ConnectAsync((await ResolveAddressAsync(Address))[0], Port);
			if (!client.Connected)
			{
				throw new TimeoutException($"Connecting to server {Address}:{Port} timed out");
			}
			NetworkStream stream = client.GetStream();
			Result<byte[]> result = await InitializeConnection(stream, profile);
			if (login)
			{
				return result.IsSuccess ? Result.Success() : Result.Clone(result);
			}
			return (!result.IsFailure) ? (await MakeRequest(stream, profile, serverId, result.Value)) : Result.Clone(result);
		}
		catch (SocketException ex)
		{
			return Result.Failure("Network error: " + ex.Message);
		}
		catch (Exception ex2)
		{
			return Result.Failure("Unexpected error: " + ex2.Message);
		}
	}

	private async Task<Result<byte[]>> InitializeConnection(NetworkStream stream, GameProfile profile)
	{
		using MemoryStream receive = await stream.ReadSteamWithInt16Async();
		if (receive.Length < 272)
		{
			return Result<byte[]>.Failure("Invalid response length");
		}
		byte[] loginSeed = new byte[16];
		byte[] array = new byte[256];
		receive.ReadExactly(loginSeed);
		receive.ReadExactly(array);
		byte[] array2 = _generator.GenerateInitializeMessage(profile, loginSeed, array);
		await stream.WriteAsync(array2);
		using MemoryStream memoryStream = await stream.ReadSteamWithInt16Async();
		if (memoryStream.Length < 1)
		{
			return Result<byte[]>.Failure("Empty response");
		}
		byte b = (byte)memoryStream.ReadByte();
		return (b != 0) ? Result<byte[]>.Failure($"Initialization failed with status: 0x{b:X2}") : Result<byte[]>.Success(loginSeed);
	}

	private async Task<Result> MakeRequest(NetworkStream stream, GameProfile profile, string serverId, byte[] loginSeed)
	{
		byte[] authToken = profile.User.GetAuthToken();
		ChaChaPacker packer = new ChaChaPacker(authToken.CombineWith(loginSeed), ChaChaNonce, encryption: true);
		ChaChaPacker unpacker = new ChaChaPacker(loginSeed.CombineWith(authToken), ChaChaNonce, encryption: false);
		byte[] array = packer.PackMessage(9, _generator.GenerateJoinMessage(profile, serverId, loginSeed));
		await stream.WriteAsync(array);
		using MemoryStream memoryStream = await stream.ReadSteamWithInt16Async();
		byte[] data = memoryStream.ToArray();
		var (b, array2) = unpacker.UnpackMessage(data);
		if (b != 9 || array2[0] != 0)
		{
			return Result.Failure(Convert.ToHexString(new ReadOnlySpan<byte>((byte)array2[0])));
		}
		return Result.Success();
	}

	private static (string address, int port) ParseAddress(string baseAddress)
	{
		Match match = AddressRegex().Match(baseAddress);
		if (!match.Success)
		{
			throw new FormatException("Invalid address format: " + baseAddress);
		}
		string value = match.Groups["address"].Value;
		if (!match.Groups["port"].Success)
		{
			throw new FormatException("Invalid port format: " + baseAddress);
		}
		int item = int.Parse(match.Groups["port"].Value);
		return (address: value, port: item);
	}

	private static async Task<IPAddress[]> ResolveAddressAsync(string address)
	{
		if (IPAddress.TryParse(address, out IPAddress address2))
		{
			return new IPAddress[1] { address2 };
		}
		try
		{
			IPAddress[] array = await Dns.GetHostAddressesAsync(address);
			if (array.Length != 0)
			{
				return array;
			}
			throw new InvalidOperationException("Unable to resolve host: " + address);
		}
		catch (SocketException innerException)
		{
			throw new InvalidOperationException("DNS resolution failed for " + address, innerException);
		}
	}

	private static string RandomAuthServer()
	{
		YggdrasilServer[] result = new HttpWrapper().GetAsync<YggdrasilServer[]>("https://x19.update.netease.com/authserver.list").GetAwaiter().GetResult();
		if (result == null || result.Length == 0)
		{
			throw new Exception("No servers found.");
		}
		Random random = new Random();
		YggdrasilServer yggdrasilServer = result[random.Next(result.Length)];
		return yggdrasilServer.Ip + ":" + yggdrasilServer.Port;
	}

	[GeneratedRegex("^(?<address>[^:]+):(?<port>\\d+)$")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.13.1716")]
	private static Regex AddressRegex()
	{
		return _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__AddressRegex_0.Instance;
	}
}
