using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Codexus.OpenSDK;

namespace _404NEL;

public static class ShopBridge
{
	private const string CategoryPath = "/shop-item-type/query/search-by-gc-item";

	private const string ItemPath = "/shop-item/query/search-by-gc-item-type";

	private const string CurrencyPath = "/user-currency/query";

	private const string PricePath = "/mc-item-details/get-info";

	private const string BuyPath = "/mc-item-buy";

	private const string BuyResultPath = "/buy-item-result";

	private static readonly JsonSerializerOptions Acquire_0020Acquire_0020Transform_002EDispatch = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public static async Task<ShopCategory[]> GetCategoriesAsync(string entityId, string accessToken, string serverId)
	{
		return (from item in Validate_0020Schedule_0020Acquire_002ESynchronize(await PostAsync("/shop-item-type/query/search-by-gc-item", new
			{
				gc_item_id = serverId,
				offset = 0,
				length = 100
			}, entityId, accessToken), "entities").EnumerateArray()
			select new ShopCategory(Evaluate_0020Worker_0020Acquire_002EWorker(item, "entity_id"), Evaluate_0020Worker_0020Acquire_002EWorker(item, "name"), Process_0020Finalize_0020Schedule_002EWorker(item, "mc_item_count")) into item
			where !string.IsNullOrWhiteSpace(item.Id)
			select item).ToArray();
	}

	public static async Task<ShopPage> GetItemsAsync(string entityId, string accessToken, string serverId, string categoryId, int offset, int length)
	{
		JsonElement jsonElement = await PostAsync("/shop-item/query/search-by-gc-item-type", new
		{
			gc_item_id = serverId,
			shop_item_type_id = categoryId,
			offset = Math.Max(0, offset),
			length = Math.Clamp(length, 1, 100)
		}, entityId, accessToken);
		ShopItem[] array = (from item in Validate_0020Schedule_0020Acquire_002ESynchronize(jsonElement, "entities").EnumerateArray().Select(Materialize_0020Worker_0020Transform_002EObserve)
			where !string.IsNullOrWhiteSpace(item.Id)
			select item).ToArray();
		return new ShopPage(array, Process_0020Finalize_0020Schedule_002EWorker(jsonElement, "total", array.Length));
	}

	public static async Task<ShopCurrency> GetCurrencyAsync(string entityId, string accessToken)
	{
		JsonElement jsonElement = Worker_0020Materialize_0020Schedule_002EInitialize(await PostAsync("/user-currency/query", null, entityId, accessToken), "entity");
		int num = Process_0020Finalize_0020Schedule_002EWorker(jsonElement, "pay_diamond");
		int num2 = Process_0020Finalize_0020Schedule_002EWorker(jsonElement, "free_diamond");
		return new ShopCurrency(num, num2, num + num2);
	}

	public static async Task<ShopQuote> GetQuoteAsync(string entityId, string accessToken, string itemId)
	{
		return Materialize_0020Validate_0020Transform_002EInitialize(Worker_0020Materialize_0020Schedule_002EInitialize(await PostAsync("/mc-item-details/get-info", new
		{
			item_id = itemId
		}, entityId, accessToken), "entity"), itemId);
	}

	public static async Task<ShopPurchaseResult> PurchaseAsync(string entityId, string accessToken, string itemId, int expectedDiamonds, int expectedPoints)
	{
		ShopQuote shopQuote = await GetQuoteAsync(entityId, accessToken, itemId);
		if (shopQuote.FinalDiamonds != expectedDiamonds || shopQuote.FinalPoints != expectedPoints)
		{
			throw new InvalidOperationException("商品价格已发生变化，请重新确认后购买。");
		}
		JsonElement jsonElement = Worker_0020Materialize_0020Schedule_002EInitialize(await PostAsync("/mc-item-buy", new
		{
			entity_id = 0,
			item_id = itemId,
			item_num = 1,
			shop_type = 1,
			receiver_id = string.Empty
		}, entityId, accessToken), "entity");
		string orderId = Evaluate_0020Worker_0020Acquire_002EWorker(jsonElement, "entity_id");
		int buyType = Process_0020Finalize_0020Schedule_002EWorker(jsonElement, "buy_type");
		if (string.IsNullOrWhiteSpace(orderId))
		{
			throw new InvalidOperationException("服务器未返回有效订单号。");
		}
		for (int attempt = 0; attempt < 5; attempt++)
		{
			if (attempt > 0)
			{
				await Task.Delay(TimeSpan.FromSeconds(2L));
			}
			JsonElement jsonElement2 = await PostRawAsync("/buy-item-result", new
			{
				orderid = orderId,
				buy_type = buyType
			}, entityId, accessToken);
			if (Process_0020Finalize_0020Schedule_002EWorker(jsonElement2, "code", -1) == 0)
			{
				return new ShopPurchaseResult(Success: true, orderId, Evaluate_0020Worker_0020Acquire_002EWorker(jsonElement2, "message", "购买成功"));
			}
		}
		return new ShopPurchaseResult(Success: false, orderId, "订单已提交，但暂未确认到账，请稍后在官方账户中核对。");
	}

	private static ShopItem Materialize_0020Worker_0020Transform_002EObserve(JsonElement Normalize_0020Process_0020Initialize_002EValidate)
	{
		return new ShopItem(Evaluate_0020Worker_0020Acquire_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Observe_0020Worker_0020Normalize_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Finalize_0020Worker_0020Resolve_002EWorker, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Initialize_0020Synchronize_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Finalize_0020Resolve_002EValidate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Materialize_0020Materialize_002ENormalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Dispatch_0020Dispatch_0020Initialize_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Transform_0020Finalize_002EWorker, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Dispatch_0020Transform_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Schedule_0020Synchronize_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Synchronize_0020Process_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Schedule_0020Initialize_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Schedule_0020Transform_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Dispatch_0020Process_0020Schedule_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Normalize_0020Process_0020Initialize_002EValidate, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Worker_0020Worker_0020Schedule_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)));
	}

	private static ShopQuote Materialize_0020Validate_0020Transform_002EInitialize(JsonElement Dispatch_0020Dispatch_0020Materialize_002EInitialize, string Normalize_0020Process_0020Schedule_002EObserve)
	{
		return new ShopQuote(Evaluate_0020Worker_0020Acquire_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Acquire_0020Schedule_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Normalize_0020Process_0020Schedule_002EObserve), Evaluate_0020Worker_0020Acquire_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Synchronize_0020Validate_002EMaterialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Transform_0020Resolve_0020Observe_002ETransform, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Validate_0020Schedule_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Initialize_0020Dispatch_0020Initialize_002EEvaluate, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Evaluate_0020Worker_0020Acquire_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Schedule_0020Observe_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform), Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Materialize_0020Worker_0020Worker_002EProcess, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Acquire_0020Worker_0020Evaluate_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Observe_0020Worker_0020Transform_002ESynchronize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Process_0020Validate_002EFinalize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)), Process_0020Finalize_0020Schedule_002EWorker(Dispatch_0020Dispatch_0020Materialize_002EInitialize, Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Process_0020Transform_0020Finalize_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)));
	}

	private static async Task<JsonElement> PostAsync(string path, object? payload, string entityId, string accessToken)
	{
		JsonElement jsonElement = await PostRawAsync(path, payload, entityId, accessToken);
		int num = Process_0020Finalize_0020Schedule_002EWorker(jsonElement, "code", -1);
		if (num != 0)
		{
			string value = Evaluate_0020Worker_0020Acquire_002EWorker(jsonElement, "message");
			throw new InvalidOperationException(string.IsNullOrWhiteSpace(value) ? $"网易接口返回错误：{num}" : $"网易接口返回错误：{value} ({num})");
		}
		return jsonElement;
	}

	private static async Task<JsonElement> PostRawAsync(string path, object? payload, string entityId, string accessToken)
	{
		if (string.IsNullOrWhiteSpace(entityId) || string.IsNullOrWhiteSpace(accessToken))
		{
			throw new InvalidOperationException("账号登录状态无效。");
		}
		string body = ((payload == null) ? string.Empty : JsonSerializer.Serialize(payload, Acquire_0020Acquire_0020Transform_002EDispatch));
		using HttpResponseMessage response = await X19.ApiPostAsync(path, body, entityId, accessToken);
		string json = await response.Content.ReadAsStringAsync();
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException($"网易接口请求失败：HTTP {(int)response.StatusCode}");
		}
		try
		{
			using JsonDocument jsonDocument = JsonDocument.Parse(json);
			return jsonDocument.RootElement.Clone();
		}
		catch (JsonException innerException)
		{
			throw new InvalidOperationException("网易接口返回了无法解析的数据。", innerException);
		}
	}

	private static JsonElement Validate_0020Schedule_0020Acquire_002ESynchronize(JsonElement Materialize_0020Worker_0020Observe_002EAcquire, string Worker_0020Worker_0020Initialize_002EFinalize)
	{
		if (Transform_0020Initialize_0020Observe_002EAcquire(Materialize_0020Worker_0020Observe_002EAcquire, Worker_0020Worker_0020Initialize_002EFinalize, out var Transform_0020Evaluate_0020Resolve_002EValidate) && Transform_0020Evaluate_0020Resolve_002EValidate.ValueKind == JsonValueKind.Array)
		{
			return Transform_0020Evaluate_0020Resolve_002EValidate;
		}
		return JsonDocument.Parse(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Schedule_0020Schedule_0020Evaluate_002EDispatch, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform)).RootElement.Clone();
	}

	private static JsonElement Worker_0020Materialize_0020Schedule_002EInitialize(JsonElement Observe_0020Dispatch_0020Normalize_002EResolve, string Resolve_0020Transform_0020Validate_002EFinalize)
	{
		if (Transform_0020Initialize_0020Observe_002EAcquire(Observe_0020Dispatch_0020Normalize_002EResolve, Resolve_0020Transform_0020Validate_002EFinalize, out var Transform_0020Evaluate_0020Resolve_002EValidate) && Transform_0020Evaluate_0020Resolve_002EValidate.ValueKind == JsonValueKind.Object)
		{
			return Transform_0020Evaluate_0020Resolve_002EValidate;
		}
		throw new InvalidOperationException(Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Resolve_0020Evaluate_0020Initialize_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform) + Resolve_0020Transform_0020Validate_002EFinalize + Worker_0020Normalize_0020Dispatch_002EMaterialize.Materialize_0020Finalize_0020Synchronize_002EAcquire(Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Normalize_0020Process_0020Evaluate_002ESchedule, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Validate_0020Transform_0020Observe_002EInitialize, Schedule_0020Acquire_0020Initialize_002EResolve._003C_003Ec.Evaluate_0020Resolve_0020Evaluate_002ETransform));
	}

	private static bool Transform_0020Initialize_0020Observe_002EAcquire(JsonElement Schedule_0020Initialize_0020Initialize_002EMaterialize, string Materialize_0020Initialize_0020Worker_002EWorker, out JsonElement Transform_0020Evaluate_0020Resolve_002EValidate)
	{
		if (Schedule_0020Initialize_0020Initialize_002EMaterialize.ValueKind == JsonValueKind.Object)
		{
			foreach (JsonProperty item in Schedule_0020Initialize_0020Initialize_002EMaterialize.EnumerateObject())
			{
				if (item.Name.Equals(Materialize_0020Initialize_0020Worker_002EWorker, StringComparison.OrdinalIgnoreCase))
				{
					Transform_0020Evaluate_0020Resolve_002EValidate = item.Value;
					return true;
				}
			}
		}
		Transform_0020Evaluate_0020Resolve_002EValidate = default;
		return false;
	}

	private static string Evaluate_0020Worker_0020Acquire_002EWorker(JsonElement Synchronize_0020Resolve_0020Normalize_002EProcess, string Schedule_0020Transform_0020Evaluate_002EInitialize, string Materialize_0020Synchronize_0020Acquire_002EAcquire = "")
	{
		if (!Transform_0020Initialize_0020Observe_002EAcquire(Synchronize_0020Resolve_0020Normalize_002EProcess, Schedule_0020Transform_0020Evaluate_002EInitialize, out var Transform_0020Evaluate_0020Resolve_002EValidate))
		{
			return Materialize_0020Synchronize_0020Acquire_002EAcquire;
		}
		return Transform_0020Evaluate_0020Resolve_002EValidate.ValueKind switch
		{
			JsonValueKind.String => Transform_0020Evaluate_0020Resolve_002EValidate.GetString() ?? Materialize_0020Synchronize_0020Acquire_002EAcquire, 
			JsonValueKind.Number => Transform_0020Evaluate_0020Resolve_002EValidate.GetRawText(), 
			_ => Materialize_0020Synchronize_0020Acquire_002EAcquire, 
		};
	}

	private static int Process_0020Finalize_0020Schedule_002EWorker(JsonElement Worker_0020Resolve_0020Initialize_002EDispatch, string Normalize_0020Validate_0020Finalize_002EEvaluate, int Dispatch_0020Resolve_0020Materialize_002EFinalize = 0)
	{
		if (!Transform_0020Initialize_0020Observe_002EAcquire(Worker_0020Resolve_0020Initialize_002EDispatch, Normalize_0020Validate_0020Finalize_002EEvaluate, out var Transform_0020Evaluate_0020Resolve_002EValidate))
		{
			return Dispatch_0020Resolve_0020Materialize_002EFinalize;
		}
		if (Transform_0020Evaluate_0020Resolve_002EValidate.ValueKind == JsonValueKind.Number && Transform_0020Evaluate_0020Resolve_002EValidate.TryGetInt32(out var value))
		{
			return value;
		}
		if (Transform_0020Evaluate_0020Resolve_002EValidate.ValueKind == JsonValueKind.String && int.TryParse(Transform_0020Evaluate_0020Resolve_002EValidate.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
		{
			return value;
		}
		return Dispatch_0020Resolve_0020Materialize_002EFinalize;
	}
}
