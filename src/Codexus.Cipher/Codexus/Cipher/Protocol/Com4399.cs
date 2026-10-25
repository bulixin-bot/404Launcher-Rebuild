using System;
using System.CodeDom.Compiler;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Threading;
using System.Threading.Tasks;
using Codexus.Cipher.Entities.Com4399;
using Codexus.Cipher.Extensions.Com4399Extensions;
using Codexus.Cipher.Utils.Http;
using Codexus.Development.SDK.Utils;
using Serilog;

namespace Codexus.Cipher.Protocol;

public class Com4399
{
	private const string Com4399File = "4399com.cds";

	private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	private readonly HttpWrapper _4399Api = new HttpWrapper("https://m.4399api.com");

	private readonly Lock _lock = new Lock();

	private readonly HttpWrapper _login = new HttpWrapper("https://ptlogin.4399.com", null, new HttpClientHandler
	{
		AllowAutoRedirect = false
	});

	private readonly MgbSdk _mgbSdk = new MgbSdk("x19");

	private readonly WebNexusApi _nexus = new WebNexusApi("YXBwSWQ9Q29kZXh1cy5HYXRld2F5LmFwcFNlY3JldD1hN0s5bTJYcUw4YkM0d1ox");

	private string _deviceIdentifier = string.Empty;

	private string _deviceIdentifierSm = string.Empty;

	private string _state = string.Empty;

	private string _udid = string.Empty;

	public Com4399()
	{
		CreateOrLoadDevice().GetAwaiter().GetResult();
	}

	private async Task CreateOrLoadDevice()
	{
		byte[] array = MPay.LoadFromFile("4399com.cds");
		Entity4399Device entity4399Device = ((array != null) ? LoadDevice(array) : (await CreateDevice()));
		Entity4399Device entity4399Device2 = entity4399Device;
		if (entity4399Device2.DeviceState == null)
		{
			entity4399Device2 = await CreateDevice();
		}
		_deviceIdentifier = entity4399Device2.DeviceIdentifier;
		_deviceIdentifierSm = entity4399Device2.DeviceIdentifierSm;
		_udid = entity4399Device2.DeviceUdid;
		_state = entity4399Device2.DeviceState;
	}

	private static Entity4399Device LoadDevice(byte[] data)
	{
		return JsonSerializer.Deserialize<Entity4399Device>(data);
	}

	private async Task<Entity4399Device> CreateDevice()
	{
		_deviceIdentifier = GenerateIdentifier();
		_deviceIdentifierSm = GenerateIdentifier();
		_udid = Guid.NewGuid().ToString();
		string deviceState = await OAuthDevice();
		Entity4399Device entity4399Device = new Entity4399Device
		{
			DeviceIdentifier = _deviceIdentifier,
			DeviceIdentifierSm = _deviceIdentifierSm,
			DeviceUdid = _udid,
			DeviceState = deviceState
		};
		using (_lock.EnterScope())
		{
			MPay.SaveToFile("4399com.cds", JsonSerializer.Serialize(entity4399Device, DefaultOptions));
			return entity4399Device;
		}
	}

	private async Task<string> OAuthDevice()
	{
		string body = new ParameterBuilder().Append("usernames", "").Append("top_bar", "1").Append("state", "")
			.Append("device", JsonSerializer.Serialize(new Entity4399OAuth
			{
				DeviceIdentifier = _deviceIdentifier,
				DeviceIdentifierSm = _deviceIdentifierSm,
				Udid = _udid
			}, DefaultOptions))
			.FormUrlEncode();
		HttpResponseMessage response = await _4399Api.PostAsync("/openapiv2/oauth.html", body, "application/x-www-form-urlencoded");
		response.EnsureSuccessStatusCode();
		return new ParameterBuilder((JsonSerializer.Deserialize<Entity4399OAuthResponse>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("Failed to deserialize: " + response.Content.ReadAsStringAsync().Result)).Result.LoginUrl).Get("state");
	}

	public async Task<string> LoginAndAuthorize(string username, string password, string? captcha = null, string? captchaId = null, int retry = 0)
	{
		if (retry > 5)
		{
			throw new Exception("Retry limit exceeded");
		}
		ParameterBuilder parameterBuilder = new ParameterBuilder().Append("_d", _deviceIdentifier).Append("access_token", "").Append("aid", "")
			.Append("auth_action", "ORILOGIN")
			.Append("auto_scroll", "")
			.Append("autoCreateAccount", "")
			.Append("bizId", "2100001792")
			.Append("cid", "")
			.Append("client_id", "40f9e9b95d6c71ba5c6e0bd14c0abeff")
			.Append("css", "")
			.Append("expand_ext_login_list", "")
			.Append("game_key", "115716")
			.Append("isInputRealname", "false")
			.Append("isValidRealname", "false")
			.Append("password", password)
			.Append("redirect_uri", "https://m.4399api.com/openapi/oauth-callback.html?gamekey=44770")
			.Append("ref", "{\"game\":\"115716\",\"channel\":\"\"}")
			.Append("reg_mode", "reg_phone")
			.Append("response_type", "TOKEN")
			.Append("scope", "basic")
			.Append("sec", "1")
			.Append("show_4399", "")
			.Append("show_back_button", "")
			.Append("show_close_button", "")
			.Append("show_ext_login", "")
			.Append("show_forget_password", "")
			.Append("show_topbar", "false")
			.Append("state", _state)
			.Append("uid", "")
			.Append("username_history", "")
			.Append("username", username.ToLowerInvariant());
		if (captcha != null && captchaId != null)
		{
			parameterBuilder.Append("captcha", captcha).Append("captcha_id", captchaId);
		}
		string body = parameterBuilder.FormUrlEncode();
		Log.Information("Executing loginAndAuthorize...");
		HttpResponseMessage redirect = await _login.PostAsync("/oauth2/loginAndAuthorize.do?channel=", body, "application/x-www-form-urlencoded");
		string text = await redirect.Content.ReadAsStringAsync();
		if (text.Contains("验证码"))
		{
			return await HandleCaptchaWithHtml(username, password, text, retry);
		}
		ParameterBuilder parameterBuilder2 = new ParameterBuilder(redirect.Headers.Location.AbsoluteUri);
		if (captcha != null && captchaId == null)
		{
			parameterBuilder2.Append("captcha", captcha);
		}
		HttpResponseMessage httpResponseMessage = await new HttpWrapper(parameterBuilder2.ToQueryUrl()).GetAsync("");
		httpResponseMessage.EnsureSuccessStatusCode();
		string text2 = await httpResponseMessage.Content.ReadAsStringAsync();
		if (text2.Contains("登录状态已失效，请重新登录"))
		{
			string text3 = await OAuthDevice();
			Entity4399Device value = new Entity4399Device
			{
				DeviceIdentifier = _deviceIdentifier,
				DeviceIdentifierSm = _deviceIdentifierSm,
				DeviceUdid = _udid,
				DeviceState = text3
			};
			_state = text3;
			using (_lock.EnterScope())
			{
				MPay.SaveToFile("4399com.cds", JsonSerializer.Serialize(value, DefaultOptions));
			}
			return await LoginAndAuthorize(username, password);
		}
		if (text2.Contains("登录成功，但账号存在异常，需要验证"))
		{
			return await LoginAndAuthorize(username, password, await CaptchaHandler.HandleLoginCaptchaAsync(text2), null, retry + 1);
		}
		Entity4399UserInfoResponse entity4399UserInfoResponse = JsonSerializer.Deserialize<Entity4399UserInfoResponse>(text2);
		if (entity4399UserInfoResponse == null || !(entity4399UserInfoResponse.Code == "100"))
		{
			throw new Exception("Failed to deserialize: " + text2);
		}
		Entity4399UserInfoResult result = entity4399UserInfoResponse.Result;
		return _mgbSdk.GenerateSAuth(Guid.NewGuid().ToString("N").ToUpper(), "", result.Uid.ToString(), result.State, "", "4399com", "ad");
	}

	private async Task<string> HandleCaptchaWithHtml(string username, string password, string html, int retry)
	{
		Match match = CaptchaRegex().Match(html);
		if (!match.Success)
		{
			throw new Exception("Cannot find captcha in html");
		}
		string matchedCaptchaId = match.Groups[1].Value;
		HttpResponseMessage httpResponseMessage = await _login.GetAsync("/ptlogin/captcha.do?captchaId=" + matchedCaptchaId + "&xx=1");
		WebNexusApi nexus = _nexus;
		string captcha = nexus.ComputeCaptchaAsync(await httpResponseMessage.Content.ReadAsByteArrayAsync());
		return await LoginAndAuthorize(username, password, captcha, matchedCaptchaId, retry + 1);
	}

	private static string GenerateIdentifier(DateTime? dateTime = null, string? additionalData = null)
	{
		string text = (dateTime ?? DateTime.Now).ToString("yyyyMMddHHmm");
		string text2 = GenerateHash50(additionalData);
		return text + text2;
	}

	private static string GenerateHash50(string? data = null)
	{
		if (string.IsNullOrEmpty(data))
		{
			data = Guid.NewGuid().ToString() + DateTime.Now.Ticks;
		}
		return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(data))).Substring(0, 50);
	}
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.13.1716")]
	private static Regex CaptchaRegex()
	{
		return _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__CaptchaRegex_0.Instance;
	}
}
