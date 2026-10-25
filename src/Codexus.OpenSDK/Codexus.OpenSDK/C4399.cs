using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.OpenSDK.Entities.C4399;
using Codexus.OpenSDK.Exceptions;
using Codexus.OpenSDK.Generator;
using Codexus.OpenSDK.Http;

namespace Codexus.OpenSDK;

public class C4399 : IDisposable
{
	private const string AppId = "kid_wdsj";

	private const string GameUrl = "https://cdn.h5wan.4399sj.com/microterminal-h5-frame?game_id=500352";

	private const string BizId = "2201001794";

	private readonly CookieContainer _cookieContainer = new CookieContainer();

	private readonly HttpWrapper _login;

	private readonly MgbSdk _mgbSdk = new MgbSdk("x19");

	private readonly HttpWrapper _service = new HttpWrapper("https://microgame.5054399.net");

	public C4399()
	{
		_login = new HttpWrapper("https://ptlogin.4399.com", null, new HttpClientHandler
		{
			CookieContainer = _cookieContainer,
			UseCookies = true,
			AllowAutoRedirect = true
		});
	}

	public void Dispose()
	{
		_login.Dispose();
		_service.Dispose();
		_mgbSdk.Dispose();
		_cookieContainer.GetAllCookies().Clear();
		GC.SuppressFinalize(this);
	}

	public async Task<string> LoginWithPasswordAsync(string username, string password, string? sessionId = null, string? captcha = null)
	{
		QueryBuilder parameters = BuildLoginParameters().Add("username", username).Add("password", password);
		if (sessionId == null && captcha == null && (await (await _login.PostAsync("/ptlogin/loginFrame.do?v=1", parameters.BuildQueryString(), "application/x-www-form-urlencoded")).Content.ReadAsStringAsync()).Contains("账号异常，请输入验证码"))
		{
			throw new VerifyException("Captcha required");
		}
		if (sessionId != null && captcha != null)
		{
			parameters.Add("sessionId", sessionId).Add("inputCaptcha", captcha);
		}
		if (!(await _login.PostAsync("/ptlogin/login.do?v=1", parameters.BuildQueryString(), "application/x-www-form-urlencoded")).IsSuccessStatusCode)
		{
			throw new Exception("Login failed.");
		}
		return await GenerateSAuthAsync();
	}

	private async Task<string> GenerateSAuthAsync()
	{
		long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		string text = new QueryBuilder().Add("appId", "kid_wdsj").Add("gameUrl", "https://cdn.h5wan.4399sj.com/microterminal-h5-frame?game_id=500352").Add("isCrossDomain", "1")
			.Add("nick", "null")
			.Add("onLineStart", "false")
			.Add("ptLogin", "true")
			.Add("rand_time", "$randTime")
			.Add("retUrl", BuildRedirectUrl(timestamp))
			.Add("show", "1")
			.BuildQueryString();
		HttpResponseMessage httpResponseMessage = await _login.GetAsync("/ptlogin/checkKidLoginUserCookie.do?" + text);
		if (httpResponseMessage.RequestMessage == null || httpResponseMessage.RequestMessage.RequestUri == null)
		{
			throw new Exception("Login to Pc499 failed");
		}
		string text2;
		int num = (text2 = httpResponseMessage.RequestMessage.RequestUri.ToString()).LastIndexOf('?') + 1;
		string queryStr = text2.Substring(num, text2.Length - num);
		QueryBuilder queryBuilder = await GetUniAuthAsync(queryStr);
		return _mgbSdk.GenerateSAuth(queryBuilder.Get("username"), queryBuilder.Get("uid"), queryBuilder.Get("token"), queryBuilder.Get("time"), "4399pc");
	}

	private async Task<QueryBuilder> GetUniAuthAsync(string queryStr)
	{
		long value = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		string callback = $"jQuery1830{StringGenerator.GenerateRandomString(16, includeNumbers: true, includeUppercase: false, includeLowercase: false)}_{value}";
		string text = new QueryBuilder().Add("callback", callback).Add("queryStr", queryStr).BuildQueryString();
		HttpResponseMessage httpResponseMessage = await _service.GetAsync("/v2/service/sdk/info?" + text);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new Exception("Failed to get UniAuth data from 4399.");
		}
		return new QueryBuilder((JsonSerializer.Deserialize<C4399UniAuth>(TrimCallbackWrapper(await httpResponseMessage.Content.ReadAsStringAsync(), callback)) ?? throw new Exception("Failed to parse UniAuth JSON.")).Data.SdkLoginData);
	}

	private static string TrimCallbackWrapper(string content, string callback)
	{
		string text = callback + "(";
		if (!content.StartsWith(text))
		{
			throw new FormatException("Unexpected callback format from 4399 response.");
		}
		int length = text.Length;
		return content.Substring(length, content.Length - 1 - length);
	}

	private static QueryBuilder BuildLoginParameters()
	{
		return new QueryBuilder().Add("appId", "kid_wdsj").Add("autoLogin", "on").Add("bizId", "2201001794")
			.Add("css", "https://microgame.5054399.net/v2/resource/cssSdk/default/login.css")
			.Add("displayMode", "popup")
			.Add("externalLogin", "qq")
			.Add("gameId", "wd")
			.Add("iframeId", "popup_login_frame")
			.Add("includeFcmInfo", "false")
			.Add("layout", "vertical")
			.Add("layoutSelfAdapting", "true")
			.Add("level", "8")
			.Add("loginFrom", "uframe")
			.Add("mainDivId", "popup_login_div")
			.Add("postLoginHandler", "default")
			.Add("redirectUrl", "")
			.Add("regLevel", "8")
			.Add("sec", "1")
			.Add("sessionId", "")
			.Add("userNameLabel", "4399用户名")
			.Add("userNameTip", "请输入4399用户名")
			.Add("welcomeTip", "欢迎回到4399");
	}

	private static string BuildRedirectUrl(long timestamp)
	{
		return $"https://ptlogin.4399.com/resource/ucenter.html?action=login&appId={"kid_wdsj"}&loginLevel=8&regLevel=8&bizId={"2201001794"}&externalLogin=qq&qrLogin=true&layout=vertical&level=101&css=https://microgame.5054399.net/v2/resource/cssSdk/default/login.css&v=2018_11_26_16&postLoginHandler=redirect&checkLoginUserCookie=true&redirectUrl=http%3A%2F%2Fcdn.h5wan.4399sj.com%2Fmicroterminal-h5-frame%3Fgame_id%3D500352%26rand_time%3D{timestamp}";
	}
}
