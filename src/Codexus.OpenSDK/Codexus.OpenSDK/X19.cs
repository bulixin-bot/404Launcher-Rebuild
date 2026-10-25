using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.OpenSDK.Cipher;
using Codexus.OpenSDK.Entities.MPay;
using Codexus.OpenSDK.Entities.X19;
using Codexus.OpenSDK.Extensions;
using Codexus.OpenSDK.Http;

namespace Codexus.OpenSDK;

public class X19 : UniSdkMPay
{
	public static class InterconnectionApi
	{
		public static async Task LoginStart(string userId, string userToken)
		{
			await ObtCorePostAsync("/interconn/web/game-play-v2/login-start", "{\"strict_mode\":true}", userId, userToken);
		}

		public static async Task GameStartAsync(string userId, string userToken, string gameId, X19EnumGameType gameType = X19EnumGameType.NetGame, string skin = "10000")
		{
			X19GameStart x19GameStart = new X19GameStart { GameType = gameType.ToOrdinalString(), GameId = gameId, ItemList = new string[1] { skin } };
			string body = JsonSerializer.Serialize(x19GameStart);
			await ObtCorePostAsync("/interconn/web/game-play-v2/start", body, userId, userToken);
		}
	}

	private static readonly HttpWrapper ObtCore = new HttpWrapper("https://x19obtcore.nie.netease.com:8443", (Codexus.OpenSDK.Http.HttpRequestOptions options) =>
	{
		options.WithUserAgent("WPFLauncher/0.0.0.0");
	});

	private static readonly HttpWrapper ApiGateway = new HttpWrapper("https://x19apigatewayobt.nie.netease.com", (Codexus.OpenSDK.Http.HttpRequestOptions options) =>
	{
		options.WithUserAgent("WPFLauncher/0.0.0.0");
	});

	private readonly MgbSdk _sdk = new MgbSdk("x19");

	public X19()
		: base("aecfrxodyqaaaajp-g-x19", GetLatestVersion())
	{
	}

	public async Task<(X19AuthenticationOtp, string)> ContinueAsync(MPayUserWrapper? user, MPayDevice? device)
	{
		ArgumentNullException.ThrowIfNull(user, "user");
		ArgumentNullException.ThrowIfNull(device, "device");
		X19SAuthJsonWrapper wrapper = new X19SAuthJsonWrapper
		{
			Json = JsonSerializer.Serialize(new X19SAuthJson
			{
				SdkUid = user.User.Id,
				SessionId = user.User.Token,
				Udid = Guid.NewGuid().ToString("N").ToUpper(),
				DeviceId = device.Id
			})
		};
		return await ContinueAsync(wrapper);
	}

	public async Task<(X19AuthenticationOtp, string)> ContinueAsync(string json)
	{
		X19SAuthJsonWrapper wrapper;
		try
		{
			wrapper = JsonSerializer.Deserialize<X19SAuthJsonWrapper>(json);
		}
		catch (Exception)
		{
			wrapper = new X19SAuthJsonWrapper
			{
				Json = json
			};
		}
		return await ContinueAsync(wrapper);
	}

	public async Task<(X19AuthenticationOtp, string)> ContinueAsync(X19SAuthJsonWrapper wrapper)
	{
		X19SAuthJson authJson = JsonSerializer.Deserialize<X19SAuthJson>(wrapper.Json);
		if (authJson.LoginChannel != "netease")
		{
			await _sdk.AuthenticateSessionAsync(wrapper.Json);
		}
		X19AuthenticationOtp user = await AuthenticationOtpAsync(wrapper, await LoginOtpAsync(wrapper));
		await InterconnectionApi.LoginStart(user.EntityId, user.Token);
		return (user, authJson.LoginChannel);
	}

	private async Task<X19AuthenticationOtp> AuthenticationOtpAsync(X19SAuthJsonWrapper wrapper, X19LoginOtp otp)
	{
		X19SAuthJson x19SAuthJson = JsonSerializer.Deserialize<X19SAuthJson>(wrapper.Json);
		X19AuthenticationDetail value = new X19AuthenticationDetail
		{
			Udid = x19SAuthJson.Udid,
			AppVersion = GameVersion,
			PayChannel = x19SAuthJson.AppChannel
		};
		string s = JsonSerializer.Serialize(new X19AuthenticationData
		{
			SaData = JsonSerializer.Serialize(value),
			AuthJson = wrapper.Json,
			Version = new X19AuthenticationVersion
			{
				Version = GameVersion
			},
			Aid = otp.Aid.ToString(),
			OtpToken = otp.OtpToken,
			LockTime = otp.LockTime
		});
		byte[] content = HttpCipher.HttpEncrypt(Encoding.UTF8.GetBytes(s));
		return JsonSerializer.Deserialize<X19Entity<X19AuthenticationOtp>>(HttpCipher.HttpDecrypt(await (await ObtCore.PostAsync("/authentication-otp", content)).Content.ReadAsByteArrayAsync()) ?? throw new Exception("Cannot decrypt data")).Data;
	}

	private async Task<X19LoginOtp> LoginOtpAsync(X19SAuthJsonWrapper wrapper)
	{
		string text = await (await ObtCore.PostAsync("/login-otp", JsonSerializer.Serialize(wrapper))).Content.ReadAsStringAsync();
		X19Entity<JsonElement?> x19Entity = JsonSerializer.Deserialize<X19Entity<JsonElement?>>(text);
		if (x19Entity == null)
		{
			throw new Exception("Failed to deserialize: " + text);
		}
		if (x19Entity.Code != 0 || !x19Entity.Data.HasValue)
		{
			throw new Exception("Failed to deserialize: " + x19Entity.Message);
		}
		return JsonSerializer.Deserialize<X19LoginOtp>(x19Entity.Data.Value.GetRawText());
	}

	private static string GetLatestVersion()
	{
		return GetPatchVersionsAsync().GetAwaiter().GetResult().Keys.Last();
	}

	public static async Task<Dictionary<string, X19PatchVersion>> GetPatchVersionsAsync()
	{
		string text = await (await new HttpWrapper().GetAsync("https://x19.update.netease.com/pl/x19_java_patchlist")).Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<Dictionary<string, X19PatchVersion>>("{" + text[..text.LastIndexOf(',')] + "}");
	}

	public static async Task<HttpResponseMessage> ObtCorePostAsync(string url, string body, string userId, string userToken)
	{
		return await PostAsync(url, body, userId, userToken, ObtCore);
	}

	public static async Task<HttpResponseMessage> ApiPostAsync(string url, string body, string userId, string userToken)
	{
		return await PostAsync(url, body, userId, userToken, ApiGateway);
	}

	public static async Task<HttpResponseMessage> PostAsync(string url, string body, string userId, string userToken, HttpWrapper http)
	{
		return await http.PostAsync(url, body, "application/json", (Codexus.OpenSDK.Http.HttpRequestOptions options) =>
		{
			options.AddHeaders(DynamicToken.Compute(url, body, userId, userToken));
		});
	}
}
