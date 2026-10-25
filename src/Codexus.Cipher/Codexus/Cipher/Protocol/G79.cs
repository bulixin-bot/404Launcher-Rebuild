using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.Connection.G79;
using Codexus.Cipher.Entities.G79;
using Codexus.Cipher.Entities.G79.NetGame;
using Codexus.Cipher.Entities.G79.RentalGame;
using Codexus.Cipher.Entities.WPFLauncher;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;
using Codexus.Development.SDK.Utils;
using Serilog;

namespace Codexus.Cipher.Protocol;

public class G79 : IDisposable
{
	private readonly HttpWrapper _client = new HttpWrapper("https://g79mclobt.minecraft.cn", null, new HttpClientHandler
	{
		AutomaticDecompression = DecompressionMethods.GZip,
		ServerCertificateCustomValidationCallback = delegate
		{
			return true;
		},
		UseProxy = false
	});

	private readonly HttpWrapper _core = new HttpWrapper("https://g79obtcore.nie.netease.com:8443", (HttpWrapper.HttpWrapperBuilder builder) =>
	{
		builder.UserAgent("okhttp/3.12.12");
	}, new HttpClientHandler
	{
		AutomaticDecompression = DecompressionMethods.GZip
	}, new Version(2, 0));

	private readonly MgbSdk _mgbSdk = new MgbSdk("x19");

	public void Dispose()
	{
		_core.Dispose();
		_client.Dispose();
		GC.SuppressFinalize(this);
	}

	public Codexus.Cipher.Entities.G79.EntityAuthenticationOtp AuthenticationOtp(string cookieRequest, string nexusToken)
	{
		return AuthenticationOtpAsync(cookieRequest, nexusToken).GetAwaiter().GetResult();
	}

	private static string ExtractCookie(string cookie)
	{
		try
		{
			return JsonSerializer.Deserialize<EntityX19CookieRequest>(cookie).Json;
		}
		catch (Exception)
		{
			return cookie;
		}
	}

	private async Task<Codexus.Cipher.Entities.G79.EntityAuthenticationOtp> AuthenticationOtpAsync(string cookieRequest, string nexusToken)
	{
		string text = ExtractCookie(cookieRequest);
		Log.Information("Try extracting cookie...");
		if (cookieRequest.Contains("4399com"))
		{
			_mgbSdk.AuthSession(text).GetAwaiter().GetResult();
		}
		Log.Information("Encrypting cookie...");
		using WebNexusApi api = new WebNexusApi(nexusToken);
		string json = ((!text.StartsWith('{')) ? (await api.PeHttpEncryptAsync(text)) : (await api.PeAccountConvert(text)));
		string body = JsonSerializer.Deserialize<EntityHttpEncrypt>(json).Body;
		string text2 = await (await _core.PostAsync("/pe-authentication", body)).Content.ReadAsStringAsync();
		Log.Information("Decrypting response...");
		Entity<JsonElement?> entity = JsonSerializer.Deserialize<Entity<JsonElement?>>(JsonSerializer.Deserialize<EntityHttpDecrypt>(api.PeHttpDecryptAsync(text2).GetAwaiter().GetResult()).Body);
		if (entity == null)
		{
			throw new Exception("Failed to deserialize: " + text2);
		}
		if (entity.Code != 0 || !entity.Data.HasValue)
		{
			throw new Exception("Failed to deserialize: " + entity.Message);
		}
		return JsonSerializer.Deserialize<Codexus.Cipher.Entities.G79.EntityAuthenticationOtp>(entity.Data.Value.GetRawText());
	}

	public Entity<EntityUserDetails> GetUserDetail(string userId, string userToken)
	{
		return GetUserDetailAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityUserDetails>> GetUserDetailAsync(string userId, string userToken)
	{
		string body = JsonSerializer.Serialize(new EntityQueryUserDetail
		{
			Version = new Version(2, 0)
		});
		string text = await (await _core.PostAsync("/pe-user-detail/get", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<Entity<EntityUserDetails>>(text) ?? throw new Exception("Failed to deserialize: " + text);
	}

	public Codexus.Cipher.Entities.G79.Entities<EntityNetGame> GetAvailableNetGames(string userId, string userToken)
	{
		return GetAvailableNetGamesAsync(userId, userToken).GetAwaiter().GetResult();
	}

	private async Task<Codexus.Cipher.Entities.G79.Entities<EntityNetGame>> GetAvailableNetGamesAsync(string userId, string userToken)
	{
		string body = JsonSerializer.Serialize(new EntityNetGameRequest
		{
			Version = "2.12",
			ChannelId = 5
		});
		return JsonSerializer.Deserialize<Codexus.Cipher.Entities.G79.Entities<EntityNetGame>>(await (await _client.PostAsync("/pe-game/query/get-list-v4", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityNetGameServerAddress> GetNetGameServerAddress(string userId, string userToken, string gameId)
	{
		return GetNetGameServerAddressAsync(userId, userToken, gameId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityNetGameServerAddress>> GetNetGameServerAddressAsync(string userId, string userToken, string gameId)
	{
		string body = JsonSerializer.Serialize(new EntityNetGameServerAddressRequest
		{
			ItemId = gameId
		});
		return JsonSerializer.Deserialize<Entity<EntityNetGameServerAddress>>(await (await _client.PostAsync("/pe-game/query/get-server-address", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public string GetAvailableRentalGames(string userId, string userToken, int offset)
	{
		return GetAvailableRentalGamesAsync(userId, userToken, offset).GetAwaiter().GetResult();
	}

	private async Task<string> GetAvailableRentalGamesAsync(string userId, string userToken, int offset)
	{
		string body = JsonSerializer.Serialize(new EntityRentalGameRequest
		{
			SortType = 0,
			OrderType = 0,
			Offset = offset
		});
		return await (await _client.PostAsync("/rental-server/query/available-by-sort-type", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync();
	}

	public Entity<EntityRentalGameServerAddress> GetRentalGameServerAddress(string userId, string userToken, string gameId, string password = "")
	{
		return GetRentalGameServerAddressAsync(userId, userToken, gameId, password).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityRentalGameServerAddress>> GetRentalGameServerAddressAsync(string userId, string userToken, string gameId, string password = "")
	{
		string body = JsonSerializer.Serialize(new EntityRentalGameServerAddressRequest
		{
			ServerId = gameId,
			Password = password
		});
		return JsonSerializer.Deserialize<Entity<EntityRentalGameServerAddress>>(await (await _client.PostAsync("/rental-server-world-enter/get", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntitySetNickName> SetNickName(string userId, string userToken, string nickName)
	{
		return SetNickNameAsync(userId, userToken, nickName).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntitySetNickName>> SetNickNameAsync(string userId, string userToken, string nickName)
	{
		string body = JsonSerializer.Serialize(new EntitySetNickNameRequest
		{
			Name = nickName,
			EntityId = userId
		});
		return JsonSerializer.Deserialize<Entity<EntitySetNickName>>(await (await _client.PostAsync("/nickname-setting", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}
}
