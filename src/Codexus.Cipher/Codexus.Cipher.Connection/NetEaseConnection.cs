using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Connection.ChaCha;
using Codexus.Cipher.Entities.Connection;
using Codexus.Cipher.Extensions;
using Codexus.Development.SDK.Utils;
using Serilog;

namespace Codexus.Cipher.Connection;

public static class NetEaseConnection
{
	private static readonly byte[] TokenKey = new byte[16]
	{
		172, 36, 156, 105, 199, 44, 179, 180, 78, 192,
		204, 108, 84, 58, 129, 149
	};

	private static readonly byte[] ChaChaNonce = "163 NetEase\n"u8.ToArray();

	public static int RandomAuthPort()
	{
		int[] array = new int[4] { 10200, 10600, 10400, 10000 };
		return array[new Random().Next(0, array.Length)];
	}

	public static async Task CreateAuthenticatorAsync(string serverId, string gameId, string gameVersion, string modInfo, string nexusToken, int userId, string userToken, string authAddress, int authPort, Action handleSuccess, Func<string, string, int, string, byte[], string, byte[]>? buildEstablishing = null, Func<string, ChaChaOfSalsa, string, long, string, string, string, int, byte[], byte[]>? buildJoinServerMessage = null)
	{
		if (buildEstablishing == null)
		{
			buildEstablishing = DefaultBuildEstablishing;
		}
		if (buildJoinServerMessage == null)
		{
			buildJoinServerMessage = DefaultBuildJoinServerMessage;
		}
		TcpClient client = new TcpClient();
		try
		{
			await client.ConnectAsync(IPAddress.Parse(authAddress), authPort);
			if (!client.Connected)
			{
				throw new TimeoutException($"Connecting to server {authAddress}:{authPort} timed out");
			}
			Log.Information("Connected to server {Address}:{Port}", authAddress, authPort);
			NetworkStream stream = client.GetStream();
			using MemoryStream details = await stream.ReadSteamWithInt16Async();
			byte[] arg = details.ToArray();
			byte[] remoteKey = new byte[16];
			byte[] array = new byte[256];
			details.Position = 0L;
			details.ReadExactly(remoteKey);
			details.ReadExactly(array);
			byte[] array2 = buildEstablishing(nexusToken, gameVersion, userId, userToken, arg, "netease");
			await stream.WriteAsync(array2);
			using MemoryStream statusStream = await stream.ReadSteamWithInt16Async();
			byte b = (byte)statusStream.ReadByte();
			if (b != 0)
			{
				throw new Exception("Establishing error: " + Convert.ToHexString(new ReadOnlySpan<byte>((byte)b)));
			}
			Log.Information("Establishing successfully");
			byte[] array3 = Encoding.ASCII.GetBytes(userToken).Xor(TokenKey);
			ChaChaOfSalsa arg2 = new ChaChaOfSalsa(array3.CombineWith(remoteKey), ChaChaNonce, encryption: true);
			ChaChaOfSalsa decrypt = new ChaChaOfSalsa(remoteKey.CombineWith(array3), ChaChaNonce, encryption: false);
			byte[] array4 = buildJoinServerMessage(nexusToken, arg2, serverId, long.Parse(gameId), gameVersion, modInfo, "netease", userId, remoteKey);
			await stream.WriteAsync(array4);
			using MemoryStream memoryStream = await stream.ReadSteamWithInt16Async();
			byte[] data = memoryStream.ToArray();
			var (b2, array5) = decrypt.UnpackMessage(data);
			if (b2 != 9 || array5[0] != 0)
			{
				throw new Exception("Authentication of message failed: " + array5[0]);
			}
			handleSuccess();
		}
		catch (HttpRequestException ex)
		{
			client.Close();
			if (ex.StatusCode == HttpStatusCode.Unauthorized)
			{
				Log.Error("Access token is invalid or expired.");
			}
		}
		catch (Exception ex2)
		{
			client.Close();
			throw new Exception("Failed to create connection: " + ex2.Message, ex2);
		}
	}

	private static byte[] DefaultBuildEstablishing(string nexusToken, string gameVersion, int userId, string userToken, byte[] context, string channel)
	{
		Log.Information("Building establishing message");
		return Convert.FromBase64String(JsonSerializer.Deserialize<EntityHandshake>(new WebNexusApi(nexusToken).ComputeHandshakeBodyAsync(userId, userToken, Convert.ToBase64String(context), channel, gameVersion).GetAwaiter().GetResult()).HandshakeBody);
	}

	private static byte[] DefaultBuildJoinServerMessage(string nexusToken, ChaChaOfSalsa cipher, string serverId, long gameId, string gameVersion, string modInfo, string channel, int userId, byte[] handshakeKey)
	{
		Log.Information("Building join server message");
		Dictionary<string, string> dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(new WebNexusApi(nexusToken).ComputeAuthenticationBodyAsync(serverId, gameId, gameVersion, modInfo, channel, userId, Convert.ToBase64String(handshakeKey)).GetAwaiter().GetResult());
		return cipher.PackMessage(9, Convert.FromBase64String(dictionary["authBody"]));
	}
}
