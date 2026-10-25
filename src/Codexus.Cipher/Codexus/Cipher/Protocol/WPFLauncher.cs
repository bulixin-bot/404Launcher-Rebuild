using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Codexus.Cipher.Entities;
using Codexus.Cipher.Entities.MPay;
using Codexus.Cipher.Entities.WPFLauncher;
using Codexus.Cipher.Entities.WPFLauncher.Minecraft;
using Codexus.Cipher.Entities.WPFLauncher.Minecraft.Mods;
using Codexus.Cipher.Entities.WPFLauncher.NetGame;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Skin;
using Codexus.Cipher.Entities.WPFLauncher.NetGame.Texture;
using Codexus.Cipher.Entities.WPFLauncher.RentalGame;
using Codexus.Cipher.Utils;
using Codexus.Cipher.Utils.Cipher;
using Codexus.Cipher.Utils.Http;
using Serilog;

namespace Codexus.Cipher.Protocol;

public class WPFLauncher : IDisposable
{
	private static readonly HttpWrapper Http = new HttpWrapper();

	private readonly HttpWrapper _client;

	private readonly HttpWrapper _core;

	private readonly HttpWrapper _game;

	private readonly HttpWrapper _gateway;

	private readonly HttpWrapper _rental;

	private readonly MgbSdk _sdk = new MgbSdk("x19");

	private static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
	};

	public MPay MPay { get; }

	public WPFLauncher()
	{
		MPay = new MPay("aecfrxodyqaaaajp-g-x19", GetLatestVersionAsync().GetAwaiter().GetResult());
		string userAgent = "WPFLauncher/" + MPay.GameVersion;
		_client = new HttpWrapper("https://x19mclobt.nie.netease.com", (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.UserAgent(userAgent);
		});
		_core = new HttpWrapper("https://x19obtcore.nie.netease.com:8443", (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.UserAgent(userAgent);
		});
		_game = new HttpWrapper("https://x19apigatewayobt.nie.netease.com", (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.UserAgent(userAgent);
		});
		_gateway = new HttpWrapper("https://x19apigatewayobt.nie.netease.com", (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.UserAgent(userAgent);
		});
		_rental = new HttpWrapper("https://x19mclobt.nie.netease.com", (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.UserAgent(userAgent);
		});
	}

	public void Dispose()
	{
		Http.Dispose();
		_core.Dispose();
		_game.Dispose();
		MPay.Dispose();
		_gateway.Dispose();
		_client.Dispose();
		_rental.Dispose();
		GC.SuppressFinalize(this);
	}

	public static string GetUserAgent()
	{
		return "WPFLauncher/" + GetLatestVersionAsync().GetAwaiter().GetResult();
	}

	private static async Task<Dictionary<string, EntityPatchVersion>> GetPatchVersionsAsync()
	{
		string text = await (await Http.GetAsync("https://x19.update.netease.com/pl/x19_java_patchlist")).Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<Dictionary<string, EntityPatchVersion>>("{" + text[..text.LastIndexOf(',')] + "}");
	}

	public static async Task<string> GetLatestVersionAsync()
	{
		return (await GetPatchVersionsAsync()).Keys.Last();
	}

	public async Task<EntityMPayUserResponse> LoginWithEmailAsync(string email, string password)
	{
		return await MPay.LoginWithEmailAsync(email, password);
	}

	public static EntityX19CookieRequest GenerateCookie(EntityMPayUserResponse user, EntityDevice device)
	{
		return new EntityX19CookieRequest
		{
			Json = JsonSerializer.Serialize(new EntityX19Cookie
			{
				SdkUid = user.User.Id,
				SessionId = user.User.Token,
				Udid = Guid.NewGuid().ToString("N").ToUpper(),
				DeviceId = device.Id
			}, DefaultOptions)
		};
	}

	public (EntityAuthenticationOtp, string) LoginWithCookie(string cookie)
	{
		return LoginWithCookieAsync(cookie).GetAwaiter().GetResult();
	}

	public (EntityAuthenticationOtp, string) LoginWithCookie(EntityX19CookieRequest cookie)
	{
		return LoginWithCookieAsync(cookie).GetAwaiter().GetResult();
	}

	private async Task<(EntityAuthenticationOtp, string)> LoginWithCookieAsync(string cookie)
	{
		EntityX19CookieRequest cookie2;
		try
		{
			cookie2 = JsonSerializer.Deserialize<EntityX19CookieRequest>(cookie);
		}
		catch (Exception)
		{
			cookie2 = new EntityX19CookieRequest
			{
				Json = cookie
			};
		}
		return await LoginWithCookieAsync(cookie2);
	}

	private async Task<(EntityAuthenticationOtp, string)> LoginWithCookieAsync(EntityX19CookieRequest cookie)
	{
		EntityX19Cookie entity = JsonSerializer.Deserialize<EntityX19Cookie>(cookie.Json);
		if (entity.LoginChannel != "netease")
		{
			await _sdk.AuthSession(cookie.Json);
		}
		Log.Debug("Your Cookie: {Cookie}", cookie.Json);
		Log.Information("Login with Cookie...");
		EntityAuthenticationOtp user = await AuthenticationOtpAsync(cookie, await LoginOtpAsync(cookie));
		await InterConn.LoginStart(user.EntityId, user.Token);
		_ = (Func<Task>)(() => Task.CompletedTask);
		return (user, entity.LoginChannel);
	}

	private async Task<EntityLoginOtp> LoginOtpAsync(EntityX19CookieRequest cookieRequest)
	{
		string text = await (await _core.PostAsync("/login-otp", JsonSerializer.Serialize(cookieRequest, DefaultOptions))).Content.ReadAsStringAsync();
		Entity<JsonElement?> entity = JsonSerializer.Deserialize<Entity<JsonElement?>>(text);
		if (entity == null)
		{
			throw new Exception("Failed to deserialize: " + text);
		}
		if (entity.Code != 0 || !entity.Data.HasValue)
		{
			throw new Exception("Failed to deserialize: " + entity.Message);
		}
		return JsonSerializer.Deserialize<EntityLoginOtp>(entity.Data.Value.GetRawText());
	}

	private async Task<EntityAuthenticationOtp> AuthenticationOtpAsync(EntityX19CookieRequest cookieRequest, EntityLoginOtp otp)
	{
		EntityX19Cookie entityX19Cookie = JsonSerializer.Deserialize<EntityX19Cookie>(cookieRequest.Json);
		string text = StringGenerator.GenerateHexString(4).ToUpper();
		EntityAuthenticationDetail value = new EntityAuthenticationDetail
		{
			Udid = "0000000000000000" + text,
			AppVersion = MPay.GameVersion,
			PayChannel = entityX19Cookie.AppChannel,
			Disk = text
		};
		string s = JsonSerializer.Serialize(new EntityAuthenticationData
		{
			SaData = JsonSerializer.Serialize(value, DefaultOptions),
			AuthJson = cookieRequest.Json,
			Version = new EntityAuthenticationVersion
			{
				Version = MPay.GameVersion
			},
			Aid = otp.Aid.ToString(),
			OtpToken = otp.OtpToken,
			LockTime = 0
		}, DefaultOptions);
		byte[] body = HttpUtil.HttpEncrypt(Encoding.UTF8.GetBytes(s));
		Entity<EntityAuthenticationOtp> entity = JsonSerializer.Deserialize<Entity<EntityAuthenticationOtp>>(HttpUtil.HttpDecrypt(await (await _core.PostAsync("/authentication-otp", body)).Content.ReadAsByteArrayAsync()) ?? throw new Exception("Cannot decrypt data"));
		if (entity.Code == 0)
		{
			return entity.Data;
		}
		throw new Exception(entity.Message);
	}

	public Entities<EntityNetGameItem> GetAvailableNetGames(string userId, string userToken, int offset, int length)
	{
		return GetAvailableNetGamesAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityNetGameItem>> GetAvailableNetGamesAsync(string userId, string userToken, int offset, int length)
	{
		string body = JsonSerializer.Serialize(new EntityNetGameRequest
		{
			AvailableMcVersions = Array.Empty<string>(),
			ItemType = 1,
			Length = length,
			Offset = offset,
			MasterTypeId = "2",
			SecondaryTypeId = ""
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityNetGameItem>>(await (await _game.PostAsync("/item/query/available", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityQueryNetGameItem> QueryNetGameItemByIds(string userId, string userToken, string[] ids)
	{
		return QueryNetGameItemByIdsAsync(userId, userToken, ids).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityQueryNetGameItem>> QueryNetGameItemByIdsAsync(string userId, string userToken, string[] ids)
	{
		string body = JsonSerializer.Serialize(new EntityQueryNetGameRequest
		{
			EntityIds = ids
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityQueryNetGameItem>>(await (await _game.PostAsync("/item/query/search-by-ids", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityQueryNetGameDetailItem> QueryNetGameDetailById(string userId, string userToken, string gameId)
	{
		return QueryNetGameDetailByIdAsync(userId, userToken, gameId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityQueryNetGameDetailItem>> QueryNetGameDetailByIdAsync(string userId, string userToken, string gameId)
	{
		string body = JsonSerializer.Serialize(new EntityQueryNetGameDetailRequest
		{
			ItemId = gameId
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityQueryNetGameDetailItem>>(await (await _game.PostAsync("/item-details/get_v2", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityGameCharacter> QueryNetGameCharacters(string userId, string userToken, string gameId)
	{
		return QueryNetGameCharactersAsync(userId, userToken, gameId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityGameCharacter>> QueryNetGameCharactersAsync(string userId, string userToken, string gameId)
	{
		string body = JsonSerializer.Serialize(new EntityQueryGameCharacters
		{
			GameId = gameId,
			UserId = userId
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityGameCharacter>>(await (await _game.PostAsync("/game-character/query/user-game-characters", body, (HttpWrapper.HttpWrapperBuilder builder) =>
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
		string body = JsonSerializer.Serialize(new
		{
			item_id = gameId
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityNetGameServerAddress>>(await (await _game.PostAsync("/item-address/get", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public async Task<Entity<EntityQuerySearchByGameResponse>> GetGameCoreModListAsync(string userId, string userToken, EnumGameVersion gameVersion, bool isRental)
	{
		string body = JsonSerializer.Serialize(new EntityQuerySearchByGameRequest
		{
			McVersionId = (int)gameVersion,
			GameType = (isRental ? 8 : 2)
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityQuerySearchByGameResponse>>(await (await _game.PostAsync("/game-auth-item-list/query/search-by-game", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public async Task<Entities<EntityComponentDownloadInfoResponse>> GetGameCoreModDetailsListAsync(string userId, string userToken, List<ulong> gameModList)
	{
		string body = JsonSerializer.Serialize(new EntitySearchByIdsQuery
		{
			ItemIdList = gameModList
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityComponentDownloadInfoResponse>>(await (await _game.PostAsync("/user-item-download-v2/get-list", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntitySkin> GetFreeSkinList(string userId, string userToken, int offset, int length = 20)
	{
		return GetFreeSkinListAsync(userId, userToken, offset, length).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntitySkin>> GetFreeSkinListAsync(string userId, string userToken, int offset, int length = 20)
	{
		string body = JsonSerializer.Serialize(new EntityFreeSkinListRequest
		{
			IsHas = true,
			ItemType = 2,
			Length = length,
			MasterTypeId = 10,
			Offset = offset,
			PriceType = 3,
			SecondaryTypeId = 31
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntitySkin>>(await (await _gateway.PostAsync("/item/query/available", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntitySkin> QueryFreeSkinByName(string userId, string userToken, string name)
	{
		return QueryFreeSkinByNameAsync(userId, userToken, name).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntitySkin>> QueryFreeSkinByNameAsync(string userId, string userToken, string name)
	{
		string body = JsonSerializer.Serialize(new EntityQuerySkinByNameRequest
		{
			IsHas = true,
			IsSync = 0,
			ItemType = 2,
			Keyword = name,
			Length = 20,
			MasterTypeId = 10,
			Offset = 0,
			PriceType = 3,
			SecondaryTypeId = "31",
			SortType = 1,
			Year = 0
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntitySkin>>(await (await _gateway.PostAsync("/item/query/search-by-keyword", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntitySkin> GetSkinDetails(string userId, string userToken, Entities<EntitySkin> skinList)
	{
		return GetSkinDetailsAsync(userId, userToken, skinList).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntitySkin>> GetSkinDetailsAsync(string userId, string userToken, Entities<EntitySkin> skinList)
	{
		List<string> entityIds = skinList.Data.Select((EntitySkin e) => e.EntityId).ToList();
		string body = JsonSerializer.Serialize(new EntitySkinDetailsRequest
		{
			ChannelId = 11,
			EntityIds = entityIds,
			IsHas = true,
			WithPrice = true,
			WithTitleImage = true
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntitySkin>>(await (await _gateway.PostAsync("/item/query/search-by-ids", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse PurchaseSkin(string userId, string userToken, string entityId)
	{
		return PurchaseSkinAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> PurchaseSkinAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new EntitySkinPurchaseRequest
		{
			BatchCount = 1,
			BuyPath = "PC_H5_COMPONENT_DETAIL",
			Diamond = 0,
			EntityId = 0,
			ItemId = entityId,
			ItemLevel = 0,
			LastPlayTime = 0,
			PurchaseTime = 0,
			TotalPlayTime = 0,
			UserId = userId
		}, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _gateway.PostAsync("/user-item-purchase", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public EntityResponse SetSkin(string userId, string userToken, string entityId)
	{
		return SetSkinAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<EntityResponse> SetSkinAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new
		{
			skin_settings = new List<EntitySkinSettings>
			{
				new EntitySkinSettings
				{
					ClientType = "java",
					GameType = 9,
					SkinId = entityId,
					SkinMode = 0,
					SkinType = 31
				},
				new EntitySkinSettings
				{
					ClientType = "java",
					GameType = 8,
					SkinId = entityId,
					SkinMode = 0,
					SkinType = 31
				},
				new EntitySkinSettings
				{
					ClientType = "java",
					GameType = 2,
					SkinId = entityId,
					SkinMode = 0,
					SkinType = 31
				},
				new EntitySkinSettings
				{
					ClientType = "java",
					GameType = 10,
					SkinId = entityId,
					SkinMode = 0,
					SkinType = 31
				},
				new EntitySkinSettings
				{
					ClientType = "java",
					GameType = 7,
					SkinId = entityId,
					SkinMode = 0,
					SkinType = 31
				}
			}
		}, DefaultOptions);
		return JsonSerializer.Deserialize<EntityResponse>(await (await _gateway.PostAsync("/user-game-skin-multi", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public List<EntityUserGameTexture> GetSkinListInGame(string userId, string userToken, EntityUserGameTextureRequest entity)
	{
		return GetSkinListInGameAsync(userId, userToken, entity).GetAwaiter().GetResult();
	}

	private async Task<List<EntityUserGameTexture>> GetSkinListInGameAsync(string userId, string userToken, EntityUserGameTextureRequest entity)
	{
		string body = JsonSerializer.Serialize(entity, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityUserGameTexture>>(await (await _game.PostAsync("/user-game-skin/query/search-by-type", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync()).Data.ToList();
	}

	public void CreateCharacter(string userId, string userToken, string gameId, string roleName)
	{
		CreateCharacterAsync(userId, userToken, gameId, roleName).GetAwaiter().GetResult();
	}

	private async Task CreateCharacterAsync(string userId, string userToken, string gameId, string roleName)
	{
		if (!(await _game.PostAsync("/game-character", JsonSerializer.Serialize(new EntityCreateCharacter
		{
			GameId = gameId,
			UserId = userId,
			Name = roleName
		}, DefaultOptions), (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).IsSuccessStatusCode)
		{
			throw new Exception("Failed to create character");
		}
	}

	public async Task<EntityAuthenticationUpdate?> AuthenticationUpdateAsync(string userId, string userToken)
	{
		string entity = JsonSerializer.Serialize(new EntityAuthenticationUpdate
		{
			EntityId = userId,
			IsRegister = true
		}, DefaultOptions);
		byte[] body = HttpUtil.HttpEncrypt(Encoding.UTF8.GetBytes(entity));
		HttpResponseMessage response = await _core.PostAsync("/authentication/update", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, entity, userId, userToken));
		});
		byte[] array = HttpUtil.HttpDecrypt(await response.Content.ReadAsByteArrayAsync());
		if (response.IsSuccessStatusCode)
		{
			try
			{
				return JsonSerializer.Deserialize<Entity<EntityAuthenticationUpdate>>(array).Data;
			}
			catch (Exception ex)
			{
				Log.Error("Exception while deserializing, reason: {Message}", ex.Message);
			}
		}
		Log.Error("Failed to update authentication, response: {Json}", array);
		return null;
	}

	public Entities<EntityNetGameItem>? QueryNetGameWithKeyword(string userId, string userToken, string keyword)
	{
		return QueryNetGameWithKeywordAsync(userId, userToken, keyword).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityNetGameItem>?> QueryNetGameWithKeywordAsync(string userId, string userToken, string keyword)
	{
		HttpResponseMessage response = await _game.PostAsync("/item/query/search-by-keyword", JsonSerializer.Serialize(new EntityNetGameKeyword
		{
			Keyword = keyword
		}, DefaultOptions), (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		});
		string text = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode)
		{
			Log.Error("Failed to query net game with keyword, response: {Json}", text);
			return null;
		}
		return JsonSerializer.Deserialize<Entities<EntityNetGameItem>>(text);
	}

	public Entity<EntityCoreLibResponse> GetMinecraftClientLibs(string userId, string userToken, EnumGameVersion? gameVersion = null)
	{
		return GetMinecraftClientLibsAsync(userId, userToken, gameVersion).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityCoreLibResponse>> GetMinecraftClientLibsAsync(string userId, string userToken, EnumGameVersion? gameVersion = null)
	{
		gameVersion.GetValueOrDefault();
		if (!gameVersion.HasValue)
		{
			EnumGameVersion value = EnumGameVersion.NONE;
			gameVersion = value;
		}
		string body = JsonSerializer.Serialize(new EntityMcDownloadVersion
		{
			McVersion = (int)gameVersion.Value
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityCoreLibResponse>>(await (await _client.PostAsync("/game-patch-info", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityComponentDownloadInfoResponse> GetNetGameComponentDownloadList(string userId, string userToken, string gameId)
	{
		return GetNetGameComponentDownloadListAsync(userId, userToken, gameId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityComponentDownloadInfoResponse>> GetNetGameComponentDownloadListAsync(string userId, string userToken, string gameId)
	{
		string body = JsonSerializer.Serialize(new EntitySearchByItemIdQuery
		{
			ItemId = gameId,
			Length = 0,
			Offset = 0
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityComponentDownloadInfoResponse>>(await (await _client.PostAsync("/user-item-download-v2", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entities<EntityRentalGame> GetRentalGameList(string userId, string userToken, int offset)
	{
		return GetRentalGameListAsync(userId, userToken, offset).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityRentalGame>> GetRentalGameListAsync(string userId, string userToken, int offset)
	{
		string body = JsonSerializer.Serialize(new EntityQueryRentalGame
		{
			Offset = offset,
			SortType = 0
		}, DefaultOptions);
		HttpResponseMessage httpResponseMessage = await _rental.PostAsync("/rental-server/query/available-public-server", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		});
		return JsonSerializer.Deserialize<Entities<EntityRentalGame>>(options: new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			Converters = { (JsonConverter)new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
		}, json: await httpResponseMessage.Content.ReadAsStringAsync());
	}

	public Entities<EntityRentalGamePlayerList> GetRentalGameRolesList(string userId, string userToken, string entityId)
	{
		return GetRentalGameRolesListAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityRentalGamePlayerList>> GetRentalGameRolesListAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new EntityQueryRentalGamePlayerList
		{
			ServerId = entityId,
			Offset = 0,
			Length = 10
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entities<EntityRentalGamePlayerList>>(await (await _rental.PostAsync("/rental-server-player/query/search-by-user-server", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityRentalGamePlayerList> AddRentalGameRole(string userId, string userToken, string serverId, string roleName)
	{
		return AddRentalGameRoleAsync(userId, userToken, serverId, roleName).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityRentalGamePlayerList>> AddRentalGameRoleAsync(string userId, string userToken, string serverId, string roleName)
	{
		string body = JsonSerializer.Serialize(new EntityAddRentalGameRole
		{
			ServerId = serverId,
			UserId = userId,
			Name = roleName,
			CreateTs = 555555,
			IsOnline = false,
			Status = 0
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityRentalGamePlayerList>>(await (await _rental.PostAsync("/rental-server-player", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityRentalGamePlayerList> DeleteRentalGameRole(string userId, string userToken, string entityId)
	{
		return DeleteRentalGameRoleAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityRentalGamePlayerList>> DeleteRentalGameRoleAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new EntityDeleteRentalGameRole
		{
			EntityId = entityId
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityRentalGamePlayerList>>(await (await _rental.PostAsync("/rental-server-player/delete", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityRentalGameServerAddress> GetRentalGameServerAddress(string userId, string userToken, string entityId, string? pwd = null)
	{
		return GetRentalGameServerAddressAsync(userId, userToken, entityId, pwd).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityRentalGameServerAddress>> GetRentalGameServerAddressAsync(string userId, string userToken, string entityId, string? pwd = null)
	{
		string body = JsonSerializer.Serialize(new EntityQueryRentalGameServerAddress
		{
			ServerId = entityId,
			Password = (pwd ?? "none")
		}, DefaultOptions);
		return JsonSerializer.Deserialize<Entity<EntityRentalGameServerAddress>>(await (await _rental.PostAsync("/rental-server-world-enter/get", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync());
	}

	public Entity<EntityRentalGameDetails> GetRentalGameDetails(string userId, string userToken, string entityId)
	{
		return GetRentalGameDetailsAsync(userId, userToken, entityId).GetAwaiter().GetResult();
	}

	private async Task<Entity<EntityRentalGameDetails>> GetRentalGameDetailsAsync(string userId, string userToken, string entityId)
	{
		string body = JsonSerializer.Serialize(new EntityQueryRentalGameDetail
		{
			ServerId = entityId
		}, DefaultOptions);
		string json = await (await _rental.PostAsync("/rental-server-details/get", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		})).Content.ReadAsStringAsync();
		JsonSerializerOptions options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			Converters = { (JsonConverter)new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
		};
		return JsonSerializer.Deserialize<Entity<EntityRentalGameDetails>>(json, options);
	}

	public Entities<EntityRentalGame> SearchRentalGameByName(string userId, string userToken, string worldId)
	{
		return SearchRentalGameByNameAsync(userId, userToken, worldId).GetAwaiter().GetResult();
	}

	private async Task<Entities<EntityRentalGame>> SearchRentalGameByNameAsync(string userId, string userToken, string worldId)
	{
		string body = JsonSerializer.Serialize(new EntityQueryRentalGameById
		{
			Offset = 0uL,
			SortType = EnumSortType.General,
			WorldNameKey = new List<string>(1) { worldId }
		}, DefaultOptions);
		HttpResponseMessage httpResponseMessage = await _rental.PostAsync("/rental-server/query/available-public-server", body, (HttpWrapper.HttpWrapperBuilder builder) =>
		{
			builder.AddHeader(TokenUtil.ComputeHttpRequestToken(builder.Url, builder.Body, userId, userToken));
		});
		return JsonSerializer.Deserialize<Entities<EntityRentalGame>>(options: new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			Converters = { (JsonConverter)new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
		}, json: await httpResponseMessage.Content.ReadAsStringAsync());
	}
}
