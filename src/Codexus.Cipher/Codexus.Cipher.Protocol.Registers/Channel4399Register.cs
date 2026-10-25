using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Codexus.Cipher.Entities.Pc4399;
using Codexus.Cipher.Utils;
using Codexus.Cipher.Utils.Exception;
using Codexus.Cipher.Utils.Http;
using Codexus.Development.SDK.Entities;

namespace Codexus.Cipher.Protocol.Registers;

public class Channel4399Register : IDisposable
{
	private readonly HttpWrapper _register = new HttpWrapper("https://ptlogin.4399.com", null, new HttpClientHandler
	{
		AllowAutoRedirect = true
	});

	public void Dispose()
	{
		_register.Dispose();
		GC.SuppressFinalize(this);
	}

	public async Task<Entity4399Account> RegisterAsync(Func<byte[], string> captchaFunc, Func<IdCard> idCardFunc)
	{
		string account = "cxus" + RandomUtil.GetRandomString(7);
		string password = RandomUtil.GetRandomString(8);
		string captchaId = RandomUtil.GenerateSessionId();
		string captcha = await CaptchaAsync(captchaFunc, captchaId);
		IdCard idCard = idCardFunc();
		string url = BuildRegisterUrl(captchaId, captcha, account, password, idCard.IdNumber, idCard.Name);
		HttpResponseMessage httpResponseMessage = await _register.GetAsync(url);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new Exception("Status Code:" + httpResponseMessage.StatusCode);
		}
		EnsureRegisterSuccess(await httpResponseMessage.Content.ReadAsStringAsync());
		return new Entity4399Account
		{
			Account = account,
			Password = password
		};
	}

	private async Task<string> CaptchaAsync(Func<byte[], string> captchaFunc, string captchaId)
	{
		HttpResponseMessage httpResponseMessage = await _register.GetAsync("/ptlogin/captcha.do?xx=1&captchaId=" + captchaId);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new CaptchaException("Status Code:" + httpResponseMessage.StatusCode);
		}
		byte[] array = await httpResponseMessage.Content.ReadAsByteArrayAsync();
		if (array.Length < 5)
		{
			throw new CaptchaException("Captcha image invalid");
		}
		return captchaFunc(array);
	}

	private static string BuildRegisterUrl(string captchaId, string captcha, string account, string password, string idCard, string name)
	{
		return "/ptlogin/register.do?" + new ParameterBuilder().Append("postLoginHandler", "default").Append("displayMode", "popup").Append("appId", "www_home")
			.Append("gameId", "")
			.Append("cid", "")
			.Append("externalLogin", "qq")
			.Append("aid", "")
			.Append("ref", "")
			.Append("css", "")
			.Append("redirectUrl", "")
			.Append("regMode", "reg_normal")
			.Append("sessionId", captchaId)
			.Append("regIdcard", "true")
			.Append("noEmail", "false")
			.Append("crossDomainIFrame", "")
			.Append("crossDomainUrl", "")
			.Append("mainDivId", "popup_reg_div")
			.Append("showRegInfo", "true")
			.Append("includeFcmInfo", "false")
			.Append("expandFcmInput", "true")
			.Append("fcmFakeValidate", "true")
			.Append("userNameLabel", "4399用户名")
			.Append("username", account)
			.Append("password", password)
			.Append("realname", name)
			.Append("idcard", idCard)
			.Append("email", RandomUtil.GetRandomString(10, "0123456789") + "@qq.com")
			.Append("reg_eula_agree", "on")
			.Append("inputCaptcha", captcha)
			.FormUrlEncode();
	}

	private static void EnsureRegisterSuccess(string content)
	{
		if (content.Contains("验证码错误"))
		{
			throw new Exception("Captcha Invalid");
		}
		if (content.Contains("用户名已被注册"))
		{
			throw new Exception("Account has been registered");
		}
		if (!content.Contains("请一定记住您注册的用户名和密码"))
		{
			throw new Exception("Unknown error");
		}
	}

	public static string GenerateRandomIdCard()
	{
		string text = "110108" + GetRandomDate("19700101", "20041231") + RandomUtil.GetRandomString(3, "0123456789");
		return text + GetIdCardLastCode(text);
	}

	public static string GenerateChineseName()
	{
		return RandomUtil.GetRandomString(1, "李王张刘陈杨赵黄周吴徐孙胡朱高林何郭马罗梁宋郑谢韩唐冯于董萧程曹袁邓许傅沈曾彭吕苏卢蒋蔡贾丁魏薛叶阎余潘杜戴夏钟汪田任姜范方石姚谭廖邹熊金陆郝孔白崔康毛邱秦江史顾侯邵孟龙万段漕钱汤尹黎易常武乔贺赖龚文") + GenerateChineseCharacter() + GenerateChineseCharacter();
	}

	private static char GenerateChineseCharacter()
	{
		return (char)Random.Shared.Next(19968, 40870);
	}

	private static string GetRandomDate(string startDate, string endDate)
	{
		DateTime dateTime = DateTime.ParseExact(startDate, "yyyyMMdd", CultureInfo.InvariantCulture);
		int days = (DateTime.ParseExact(endDate, "yyyyMMdd", CultureInfo.InvariantCulture) - dateTime).Days;
		return dateTime.AddDays(Random.Shared.Next(days)).ToString("yyyyMMdd");
	}

	private static string GetIdCardLastCode(string idCard)
	{
		int[] factors = new int[17]
		{
			7, 9, 10, 5, 8, 4, 2, 1, 6, 3,
			7, 9, 10, 5, 8, 4, 2
		};
		string[] array = new string[11]
		{
			"1", "0", "X", "9", "8", "7", "6", "5", "4", "3",
			"2"
		};
		int num = idCard.Take(17).Select((char c, int i) => (c - 48) * factors[i]).Sum();
		return array[num % 11];
	}
}
