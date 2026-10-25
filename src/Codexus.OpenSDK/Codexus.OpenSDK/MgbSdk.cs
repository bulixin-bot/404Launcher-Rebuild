using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.OpenSDK.Entities.MgbSdk;
using Codexus.OpenSDK.Http;

namespace Codexus.OpenSDK;

public class MgbSdk(string gameId) : IDisposable
{
	private readonly HttpWrapper _sdk = new HttpWrapper("https://mgbsdk.matrix.netease.com");

	public void Dispose()
	{
		_sdk.Dispose();
		GC.SuppressFinalize(this);
	}

	public async Task AuthenticateSessionAsync(string cookie)
	{
		if (string.IsNullOrWhiteSpace(cookie))
		{
			throw new ArgumentException("Cookie cannot be null or empty.", "cookie");
		}
		HttpResponseMessage httpResponseMessage = await _sdk.PostAsync("/" + gameId + "/sdk/uni_sauth", cookie);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new HttpRequestException($"HTTP Error: {httpResponseMessage.StatusCode} - {httpResponseMessage.ReasonPhrase}");
		}
		string json = await httpResponseMessage.Content.ReadAsStringAsync();
		Dictionary<string, object> dictionary;
		try
		{
			dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
		}
		catch (JsonException inner)
		{
			throw new HttpRequestException("Failed to parse SDK response JSON.", inner);
		}
		if (dictionary == null || !dictionary.TryGetValue("code", out var value))
		{
			throw new HttpRequestException("Unexpected response format: missing 'code' field.");
		}
		if (!string.Equals(value.ToString(), "200", StringComparison.Ordinal))
		{
			string text = (dictionary.TryGetValue("status", out var value2) ? value2.ToString() : "Unknown");
			throw new HttpRequestException("Authentication failed. Status: " + text);
		}
	}

	public string GenerateSAuth(string userId, string sdkUid, string sessionId, string timestamp, string channel, string platform = "pc")
	{
		string text = Guid.NewGuid().ToString("N");
		return JsonSerializer.Serialize(new MgbSdkSAuthJson
		{
			AppChannel = channel,
			ClientLoginSn = text,
			DeviceId = text,
			GameId = gameId,
			LoginChannel = channel,
			SdkUid = sdkUid,
			SessionId = sessionId,
			Timestamp = timestamp,
			Platform = platform,
			SourcePlatform = platform,
			Udid = text,
			UserId = userId
		});
	}
}
