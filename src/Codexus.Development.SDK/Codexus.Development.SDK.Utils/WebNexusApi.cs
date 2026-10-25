using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Development.SDK.Entities;

namespace Codexus.Development.SDK.Utils;

public sealed class WebNexusApi : IDisposable
{
	private const string BaseUrl = "Codexus network access is disabled.";

	private readonly HttpClient _httpClient;

	private readonly Action<string, int> _progressCallback;

	private bool _disposed;

	public WebNexusApi(string nexusToken, Action<string, int>? progressCallback = null)
	{
		throw new InvalidOperationException("Codexus network access is disabled.");
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async Task<string> ComputeAuthenticationBodyAsync(string serverId, long gameId, string gameVersion, string modInfo, string channel, int userId, string handshakeKey)
	{
		var requestData = new { serverId, gameId, gameVersion, modInfo, channel, userId, handshakeKey };
		return await PostAsync("/api/GameCipher/compute/authentication/body", requestData);
	}

	public async Task<string> ComputeHandshakeBodyAsync(int userId, string userToken, string base64Context, string channel, string gameVersion)
	{
		var requestData = new { userId, userToken, base64Context, channel, gameVersion };
		return await PostAsync("/api/GameCipher/compute/authentication/handshake", requestData);
	}

	public async Task<string> PeAccountConvert(string body)
	{
		var requestData = new { body };
		return await PostAsync("/api/PeGameCipher/account/convert", requestData);
	}

	public async Task<string> PeAuthentication(string clientKey, string displayName, string serverId, string gameType, uint userId, string userToken)
	{
		var requestData = new { clientKey, displayName, serverId, gameType, userId, userToken };
		return await PostAsync("/api/PeGameCipher/authentication", requestData);
	}

	public async Task<string> PeHttpEncryptAsync(string body)
	{
		var requestData = new { body };
		return await PostAsync("/api/PeGameCipher/crypto/encrypt", requestData);
	}

	public async Task<string> PeHttpDecryptAsync(string body)
	{
		var requestData = new { body };
		return await PostAsync("/api/PeGameCipher/crypto/decrypt", requestData);
	}

	public string PeMcpGetCheckNum(string dynamicPyCode, string dynamicCheckSalt, string gamePlayerId)
	{
		var requestData = new { dynamicPyCode, dynamicCheckSalt, gamePlayerId };
		return JsonSerializer.Deserialize<BodyIn>(PostAsync("/api/PeZeroKnowledgeProof/zkp/get/check-num", requestData).GetAwaiter().GetResult()).Body;
	}

	public string PeMcpGetStartType(string signature, string userId)
	{
		var requestData = new { signature, userId };
		return JsonSerializer.Deserialize<BodyIn>(PostAsync("/api/PeZeroKnowledgeProof/zkp/get/start-type", requestData).GetAwaiter().GetResult()).Body;
	}

	public IdCard GetRandomIdCard()
	{
		return JsonSerializer.Deserialize<IdCard>(GetAsync("/api/app/get/app/id-card").GetAwaiter().GetResult());
	}

	public string ComputeCaptchaAsync(byte[] image)
	{
		var requestData = new
		{
			body = Convert.ToBase64String(image)
		};
		return JsonSerializer.Deserialize<BodyIn>(PostAsync("/api/app/compute/captcha", requestData).GetAwaiter().GetResult()).Body;
	}

	public async Task<string> GetAppVersionAsync(string appId, string appSecret)
	{
		var requestData = new { appId, appSecret };
		return await PostAsync("/api/app/get/app/version", requestData);
	}

	public byte[] DownloadFile(string url, string pluginId)
	{
		return DownloadFileAsync(url, pluginId).GetAwaiter().GetResult();
	}

	[DebuggerStepThrough]
	private Task<string> PostAsync<T>(string endpoint, T requestData, Dictionary<string, string>? headers = null)
	{
		throw new InvalidOperationException("Codexus network access is disabled.");
	}

	[DebuggerStepThrough]
	public Task<string> GetAsync(string endpoint, Dictionary<string, string>? headers = null)
	{
		throw new InvalidOperationException("Codexus network access is disabled.");
	}

	[DebuggerStepThrough]
	private Task<byte[]> DownloadFileAsync(string endpoint, string id, Dictionary<string, string>? headers = null)
	{
		throw new InvalidOperationException("Codexus network access is disabled.");
	}

	private void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_httpClient.Dispose();
			}
			_disposed = true;
		}
	}

	~WebNexusApi()
	{
		Dispose(disposing: false);
	}
}
