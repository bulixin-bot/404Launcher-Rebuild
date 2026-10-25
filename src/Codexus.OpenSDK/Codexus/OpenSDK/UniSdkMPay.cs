using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.OpenSDK.Entities.MPay;
using Codexus.OpenSDK.Exceptions;
using Codexus.OpenSDK.Extensions;
using Codexus.OpenSDK.Generator;
using Codexus.OpenSDK.Http;

namespace Codexus.OpenSDK;

public class UniSdkMPay : IDisposable
{
	private readonly string _id;

	private readonly HttpWrapper _service = new HttpWrapper("https://service.mkey.163.com");

	public string ProjectId { get; }

	public string GameVersion { get; }

	public MPayDevice? Device { get; private set; }

	public UniSdkMPay(string project, string version)
	{
		ProjectId = project;
		GameVersion = version;
		_id = LoadOrCreateId();
	}

	public void Dispose()
	{
		_service.Dispose();
		GC.SuppressFinalize(this);
	}

	public async Task<MPayDevice> InitializeDeviceAsync()
	{
		if (Device != null)
		{
			return Device;
		}
		Device = await LoadOrCreateDeviceAsync();
		return Device;
	}

	private async Task<MPayDevice> LoadOrCreateDeviceAsync()
	{
		string deviceFileName = GetDeviceFileName();
		byte[] array = LoadFromCache(deviceFileName);
		if (array == null)
		{
			return await RegisterNewDeviceAsync(deviceFileName);
		}
		try
		{
			MPayDeviceWrapper mPayDeviceWrapper = JsonSerializer.Deserialize<MPayDeviceWrapper>(array);
			if (mPayDeviceWrapper?.Device != null)
			{
				return mPayDeviceWrapper.Device;
			}
		}
		catch (Exception)
		{
		}
		return await RegisterNewDeviceAsync(deviceFileName);
	}

	private async Task<MPayDevice> RegisterNewDeviceAsync(string fileName)
	{
		QueryBuilder queryBuilder = BuildDeviceRegistrationParams();
		HttpResponseMessage httpResponseMessage = await _service.PostAsync("/mpay/games/" + ProjectId + "/devices", queryBuilder.BuildQueryString(), "application/x-www-form-urlencoded");
		httpResponseMessage.EnsureSuccessStatusCode();
		string text = await httpResponseMessage.Content.ReadAsStringAsync();
		SaveToCache(fileName, text);
		return JsonSerializer.Deserialize<MPayDeviceWrapper>(text).Device;
	}

	public async Task<MPayUserWrapper?> LoginWithEmailAsync(string email, string password)
	{
		EnsureDeviceInitialized();
		string value = EncryptLoginParams(email, password);
		QueryBuilder queryBuilder = BuildBaseParams().Add("opt_fields", "nickname,avatar,realname_status,mobile_bind_status,mask_related_mobile,related_login_status").Add("params", value).Add("un", email.EncodeBase64());
		HttpResponseMessage response = await _service.PostAsync($"/mpay/games/{ProjectId}/devices/{Device.Id}/users", queryBuilder.BuildQueryString(), "application/x-www-form-urlencoded");
		string text = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode)
		{
			return HandleLoginError(text);
		}
		return JsonSerializer.Deserialize<MPayUserWrapper>(text);
	}

	private string EncryptLoginParams(string email, string password)
	{
		return JsonSerializer.Serialize(new MPayParameters
		{
			Username = email,
			Password = password.EncodeMd5(),
			UniqueId = _id
		}).AesEncrypt(Device.Key.DecodeHex()).EncodeHex();
	}

	private static MPayUserWrapper HandleLoginError(string errorJson)
	{
		MPayVerifyResponse? mPayVerifyResponse = JsonSerializer.Deserialize<MPayVerifyResponse>(errorJson);
		if (mPayVerifyResponse != null && mPayVerifyResponse.Code == 1351)
		{
			throw new VerifyException(errorJson);
		}
		throw new Exception("Email login failed: " + errorJson);
	}

	public async Task<bool> SendSmsCodeAsync(string phoneNumber)
	{
		EnsureDeviceInitialized();
		QueryBuilder queryBuilder = BuildBaseParams().Add("device_id", Device.Id).Add("mobile", phoneNumber);
		return (await _service.PostAsync("/mpay/api/users/login/mobile/get_sms", queryBuilder.BuildQueryString(), "application/x-www-form-urlencoded")).IsSuccessStatusCode;
	}

	public async Task<MPaySMSTicket?> VerifySmsCodeAsync(string phoneNumber, string code)
	{
		EnsureDeviceInitialized();
		QueryBuilder queryBuilder = BuildBaseParams().Add("device_id", Device.Id).Add("mobile", phoneNumber).Add("smscode", code)
			.Add("up_content", "");
		HttpResponseMessage response = await _service.PostAsync("/mpay/api/users/login/mobile/verify_sms", queryBuilder.BuildQueryString(), "application/x-www-form-urlencoded");
		string json = await response.Content.ReadAsStringAsync();
		return response.IsSuccessStatusCode ? JsonSerializer.Deserialize<MPaySMSTicket>(json) : null;
	}

	public async Task<MPayUserWrapper?> CompleteSmsLoginAsync(string phoneNumber, string ticket)
	{
		EnsureDeviceInitialized();
		string text = phoneNumber.EncodeBase64();
		QueryBuilder queryBuilder = BuildBaseParams().Add("device_id", Device.Id).Add("opt_fields", "nickname,avatar,realname_status,mobile_bind_status,mask_related_mobile,related_login_status").Add("ticket", ticket);
		HttpResponseMessage response = await _service.PostAsync("/mpay/api/users/login/mobile/finish?un=" + text, queryBuilder.BuildQueryString(), "application/x-www-form-urlencoded");
		string json = await response.Content.ReadAsStringAsync();
		return response.IsSuccessStatusCode ? JsonSerializer.Deserialize<MPayUserWrapper>(json) : null;
	}

	private QueryBuilder BuildDeviceRegistrationParams()
	{
		return BuildBaseParams().Add("unique_id", _id).Add("brand", "Microsoft").Add("device_model", "pc_mode")
			.Add("device_name", "PC-" + StringGenerator.GenerateRandomString(12))
			.Add("device_type", "Computer")
			.Add("init_urs_device", "0")
			.Add("mac", StringGenerator.GenerateRandomMacAddress())
			.Add("resolution", "1920x1080")
			.Add("system_name", "windows")
			.Add("system_version", "10.0.22621");
	}

	private QueryBuilder BuildBaseParams()
	{
		return new QueryBuilder().Add("app_channel", "netease").Add("app_mode", "2").Add("app_type", "games")
			.Add("arch", "win_x64")
			.Add("cv", "c4.2.0")
			.Add("mcount_app_key", "EEkEEXLymcNjM42yLY3Bn6AO15aGy4yq")
			.Add("mcount_transaction_id", "0")
			.Add("process_id", Environment.ProcessId.ToString())
			.Add("sv", "10.0.22621")
			.Add("updater_cv", "c1.0.0")
			.Add("game_id", ProjectId)
			.Add("gv", GameVersion);
	}

	private void EnsureDeviceInitialized()
	{
		if (Device == null)
		{
			throw new InvalidOperationException("Device not initialized. Call InitializeDeviceAsync() first.");
		}
	}

	private string LoadOrCreateId()
	{
		string uniqueIdFileName = GetUniqueIdFileName();
		byte[] array = LoadFromCache(uniqueIdFileName);
		if (array != null)
		{
			return Encoding.UTF8.GetString(array);
		}
		string text = Guid.NewGuid().ToString("N");
		SaveToCache(uniqueIdFileName, text);
		return text;
	}

	private string GetUniqueIdFileName()
	{
		return ProjectId + ".id";
	}

	private string GetDeviceFileName()
	{
		return ProjectId + ".device";
	}

	private static void SaveToCache(string fileName, string content)
	{
		try
		{
			File.WriteAllText(fileName, content);
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to save cache file " + fileName + ": " + ex.Message);
		}
	}

	private static byte[]? LoadFromCache(string fileName)
	{
		try
		{
			return (!File.Exists(fileName)) ? null : File.ReadAllBytes(fileName);
		}
		catch (Exception ex)
		{
			throw new Exception("Failed to load cache file " + fileName + ": " + ex.Message);
		}
	}
}
