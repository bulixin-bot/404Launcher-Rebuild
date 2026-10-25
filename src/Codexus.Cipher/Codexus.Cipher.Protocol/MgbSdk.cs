using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities.MgbSdk;
using Codexus.Cipher.Utils.Http;

namespace Codexus.Cipher.Protocol;

public class MgbSdk(string gameId) : IDisposable
{
	private readonly HttpWrapper _sdk = new HttpWrapper("https://mgbsdk.matrix.netease.com");

	private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	public void Dispose()
	{
		_sdk.Dispose();
		GC.SuppressFinalize(this);
	}

	public string GenerateSAuth(string deviceId, string userid, string sdkUid, string sessionId, string timestamp, string channel, string platform = "pc")
	{
		string sessionId2 = sessionId.ToUpper();
		return JsonSerializer.Serialize(new EntityMgbSdkCookie
		{
			Ip = InternalQuery.Gw,
			AimInfo = InternalQuery.ToAimInfo(),
			AppChannel = channel,
			ClientLoginSn = deviceId.ToUpper(),
			DeviceId = deviceId.ToUpper(),
			GameId = gameId,
			LoginChannel = channel,
			SdkUid = sdkUid,
			SessionId = sessionId2,
			Timestamp = timestamp,
			Platform = platform,
			SourcePlatform = platform,
			Udid = deviceId.ToUpper(),
			UserId = userid
		}, DefaultOptions);
	}

	public async Task AuthSession(string cookie)
	{
		HttpResponseMessage httpResponseMessage = await _sdk.PostAsync("/" + gameId + "/sdk/uni_sauth", cookie);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new HttpRequestException(httpResponseMessage.ReasonPhrase);
		}
		Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(await httpResponseMessage.Content.ReadAsStringAsync());
		if (dictionary["code"].ToString() != "200")
		{
			throw new HttpRequestException("Status: " + dictionary["status"]);
		}
	}
}
