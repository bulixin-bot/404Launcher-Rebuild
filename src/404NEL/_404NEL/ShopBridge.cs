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

	private static readonly JsonSerializerOptions Observe_0020Finalize_0020Transform_002EWorker = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true
	};

	public static async Task<ShopCategory[]> GetCategoriesAsync(string entityId, string accessToken, string serverId)
	{
		return (from item in Acquire_0020Dispatch_0020Dispatch_002ESchedule(await PostAsync("/shop-item-type/query/search-by-gc-item", new
			{
				gc_item_id = serverId,
				offset = 0,
				length = 100
			}, entityId, accessToken), "entities").EnumerateArray()
			select new ShopCategory(Normalize_0020Synchronize_0020Dispatch_002EObserve(item, "entity_id"), Normalize_0020Synchronize_0020Dispatch_002EObserve(item, "name"), Resolve_0020Normalize_0020Finalize_002ETransform(item, "mc_item_count")) into item
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
		ShopItem[] array = (from item in Acquire_0020Dispatch_0020Dispatch_002ESchedule(jsonElement, "entities").EnumerateArray().Select(Dispatch_0020Worker_0020Acquire_002EFinalize)
			where !string.IsNullOrWhiteSpace(item.Id)
			select item).ToArray();
		return new ShopPage(array, Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement, "total", array.Length));
	}

	public static async Task<ShopCurrency> GetCurrencyAsync(string entityId, string accessToken)
	{
		JsonElement jsonElement = Process_0020Normalize_0020Finalize_002EMaterialize(await PostAsync("/user-currency/query", null, entityId, accessToken), "entity");
		int num = Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement, "pay_diamond");
		int num2 = Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement, "free_diamond");
		return new ShopCurrency(num, num2, num + num2);
	}

	public static async Task<ShopQuote> GetQuoteAsync(string entityId, string accessToken, string itemId)
	{
		return Synchronize_0020Observe_0020Materialize_002EFinalize(Process_0020Normalize_0020Finalize_002EMaterialize(await PostAsync("/mc-item-details/get-info", new
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
		JsonElement jsonElement = Process_0020Normalize_0020Finalize_002EMaterialize(await PostAsync("/mc-item-buy", new
		{
			entity_id = 0,
			item_id = itemId,
			item_num = 1,
			shop_type = 1,
			receiver_id = string.Empty
		}, entityId, accessToken), "entity");
		string orderId = Normalize_0020Synchronize_0020Dispatch_002EObserve(jsonElement, "entity_id");
		int buyType = Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement, "buy_type");
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
			if (Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement2, "code", -1) == 0)
			{
				return new ShopPurchaseResult(Success: true, orderId, Normalize_0020Synchronize_0020Dispatch_002EObserve(jsonElement2, "message", "购买成功"));
			}
		}
		return new ShopPurchaseResult(Success: false, orderId, "订单已提交，但暂未确认到账，请稍后在官方账户中核对。");
	}

	private static ShopItem Dispatch_0020Worker_0020Acquire_002EFinalize(JsonElement Normalize_0020Transform_0020Observe_002EMaterialize)
	{
		return new ShopItem(Normalize_0020Synchronize_0020Dispatch_002EObserve(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000132.Finalize_0020Worker_0020Materialize_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000133.Dispatch_0020Validate_0020Worker_002EFinalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000134.Dispatch_0020Worker_0020Finalize_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000135.Transform_0020Worker_0020Dispatch_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000136.Initialize_0020Transform_0020Synchronize_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000137.Process_0020Synchronize_0020Transform_002EEvaluate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000138.Transform_0020Validate_0020Schedule_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000139.Dispatch_0020Evaluate_0020Evaluate_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013A.Initialize_0020Transform_0020Dispatch_002EResolve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013B.Materialize_0020Finalize_0020Schedule_002EAcquire, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013C.Validate_0020Dispatch_0020Materialize_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013D.Normalize_0020Evaluate_0020Acquire_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013E.Validate_0020Materialize_0020Validate_002EDispatch, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Normalize_0020Transform_0020Observe_002EMaterialize, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200013F.Synchronize_0020Initialize_0020Resolve_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)));
	}

	private static ShopQuote Synchronize_0020Observe_0020Materialize_002EFinalize(JsonElement Worker_0020Initialize_0020Synchronize_002EAcquire, string Validate_0020Observe_0020Finalize_002ESynchronize)
	{
		return new ShopQuote(Normalize_0020Synchronize_0020Dispatch_002EObserve(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000140.Acquire_0020Evaluate_0020Validate_002EWorker, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Validate_0020Observe_0020Finalize_002ESynchronize), Normalize_0020Synchronize_0020Dispatch_002EObserve(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000141.Transform_0020Initialize_0020Schedule_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000142.Process_0020Transform_0020Evaluate_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000143.Materialize_0020Worker_0020Validate_002EProcess, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000144.Validate_0020Resolve_0020Resolve_002ESchedule, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Normalize_0020Synchronize_0020Dispatch_002EObserve(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000145.Materialize_0020Finalize_0020Normalize_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate), Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000146.Process_0020Evaluate_0020Dispatch_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000147.Resolve_0020Normalize_0020Observe_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000148.Observe_0020Dispatch_0020Observe_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_02000149.Transform_0020Initialize_0020Process_002EValidate, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)), Resolve_0020Normalize_0020Finalize_002ETransform(Worker_0020Initialize_0020Synchronize_002EAcquire, Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200014A.Schedule_0020Dispatch_0020Dispatch_002ENormalize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)));
	}

	private static async Task<JsonElement> PostAsync(string path, object? payload, string entityId, string accessToken)
	{
		JsonElement jsonElement = await PostRawAsync(path, payload, entityId, accessToken);
		int num = Resolve_0020Normalize_0020Finalize_002ETransform(jsonElement, "code", -1);
		if (num != 0)
		{
			string value = Normalize_0020Synchronize_0020Dispatch_002EObserve(jsonElement, "message");
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
		string body = ((payload == null) ? string.Empty : JsonSerializer.Serialize(payload, Observe_0020Finalize_0020Transform_002EWorker));
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

	private static JsonElement Acquire_0020Dispatch_0020Dispatch_002ESchedule(JsonElement Initialize_0020Normalize_0020Materialize_002EMaterialize, string Evaluate_0020Worker_0020Process_002ETransform)
	{
		if (Transform_0020Resolve_0020Dispatch_002EInitialize(Initialize_0020Normalize_0020Materialize_002EMaterialize, Evaluate_0020Worker_0020Process_002ETransform, out var Worker_0020Schedule_0020Dispatch_002ETransform) && Worker_0020Schedule_0020Dispatch_002ETransform.ValueKind == JsonValueKind.Array)
		{
			return Worker_0020Schedule_0020Dispatch_002ETransform;
		}
		return JsonDocument.Parse(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200014B.Materialize_0020Dispatch_0020Dispatch_002EInitialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate)).RootElement.Clone();
	}

	private static JsonElement Process_0020Normalize_0020Finalize_002EMaterialize(JsonElement Normalize_0020Acquire_0020Observe_002EWorker, string Schedule_0020Evaluate_0020Validate_002EDispatch)
	{
		if (Transform_0020Resolve_0020Dispatch_002EInitialize(Normalize_0020Acquire_0020Observe_002EWorker, Schedule_0020Evaluate_0020Validate_002EDispatch, out var Worker_0020Schedule_0020Dispatch_002ETransform) && Worker_0020Schedule_0020Dispatch_002ETransform.ValueKind == JsonValueKind.Object)
		{
			return Worker_0020Schedule_0020Dispatch_002ETransform;
		}
		throw new InvalidOperationException(Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200014C.Validate_0020Transform_0020Transform_002EObserve, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate) + Schedule_0020Evaluate_0020Validate_002EDispatch + Initialize_0020Finalize_0020Finalize_002ETransform.Schedule_0020Acquire_0020Transform_002ESynchronize(Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200014D.Acquire_0020Resolve_0020Acquire_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005D.Resolve_0020Synchronize_0020Resolve_002EMaterialize, Validate_0020Evaluate_0020Worker_002EDispatch.RecoveredType_0200005C.Dispatch_0020Acquire_0020Transform_002EEvaluate));
	}

	private static bool Transform_0020Resolve_0020Dispatch_002EInitialize(JsonElement Observe_0020Observe_0020Finalize_002EInitialize, string Validate_0020Transform_0020Schedule_002ESchedule, out JsonElement Worker_0020Schedule_0020Dispatch_002ETransform)
	{
		if (Observe_0020Observe_0020Finalize_002EInitialize.ValueKind == JsonValueKind.Object)
		{
			foreach (JsonProperty item in Observe_0020Observe_0020Finalize_002EInitialize.EnumerateObject())
			{
				if (item.Name.Equals(Validate_0020Transform_0020Schedule_002ESchedule, StringComparison.OrdinalIgnoreCase))
				{
					Worker_0020Schedule_0020Dispatch_002ETransform = item.Value;
					return true;
				}
			}
		}
		Worker_0020Schedule_0020Dispatch_002ETransform = default;
		return false;
	}

	private static string Normalize_0020Synchronize_0020Dispatch_002EObserve(JsonElement Initialize_0020Normalize_0020Normalize_002ESchedule, string Dispatch_0020Schedule_0020Evaluate_002ESynchronize, string Schedule_0020Evaluate_0020Resolve_002ESchedule = "")
	{
		if (!Transform_0020Resolve_0020Dispatch_002EInitialize(Initialize_0020Normalize_0020Normalize_002ESchedule, Dispatch_0020Schedule_0020Evaluate_002ESynchronize, out var Worker_0020Schedule_0020Dispatch_002ETransform))
		{
			return Schedule_0020Evaluate_0020Resolve_002ESchedule;
		}
		return Worker_0020Schedule_0020Dispatch_002ETransform.ValueKind switch
		{
			JsonValueKind.String => Worker_0020Schedule_0020Dispatch_002ETransform.GetString() ?? Schedule_0020Evaluate_0020Resolve_002ESchedule, 
			JsonValueKind.Number => Worker_0020Schedule_0020Dispatch_002ETransform.GetRawText(), 
			_ => Schedule_0020Evaluate_0020Resolve_002ESchedule, 
		};
	}

	private static int Resolve_0020Normalize_0020Finalize_002ETransform(JsonElement Synchronize_0020Resolve_0020Materialize_002EDispatch, string Process_0020Materialize_0020Evaluate_002EEvaluate, int Finalize_0020Worker_0020Worker_002EObserve = 0)
	{
		if (!Transform_0020Resolve_0020Dispatch_002EInitialize(Synchronize_0020Resolve_0020Materialize_002EDispatch, Process_0020Materialize_0020Evaluate_002EEvaluate, out var Worker_0020Schedule_0020Dispatch_002ETransform))
		{
			return Finalize_0020Worker_0020Worker_002EObserve;
		}
		if (Worker_0020Schedule_0020Dispatch_002ETransform.ValueKind == JsonValueKind.Number && Worker_0020Schedule_0020Dispatch_002ETransform.TryGetInt32(out var value))
		{
			return value;
		}
		if (Worker_0020Schedule_0020Dispatch_002ETransform.ValueKind == JsonValueKind.String && int.TryParse(Worker_0020Schedule_0020Dispatch_002ETransform.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
		{
			return value;
		}
		return Finalize_0020Worker_0020Worker_002EObserve;
	}
}
