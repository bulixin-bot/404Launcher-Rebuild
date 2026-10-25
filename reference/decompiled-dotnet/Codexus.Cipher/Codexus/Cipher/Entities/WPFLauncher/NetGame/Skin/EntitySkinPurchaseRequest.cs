using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.NetGame.Skin;

public class EntitySkinPurchaseRequest
{
	[JsonPropertyName("batch_count")]
	public required int BatchCount { get; set; }

	[JsonPropertyName("buy_path")]
	public required string BuyPath { get; set; }

	[JsonPropertyName("diamond")]
	public required int Diamond { get; set; }

	[JsonPropertyName("entity_id")]
	public required int EntityId { get; set; }

	[JsonPropertyName("item_id")]
	public required string ItemId { get; set; }

	[JsonPropertyName("item_level")]
	public required int ItemLevel { get; set; }

	[JsonPropertyName("last_play_time")]
	public required int LastPlayTime { get; set; }

	[JsonPropertyName("purchase_time")]
	public required int PurchaseTime { get; set; }

	[JsonPropertyName("total_play_time")]
	public required int TotalPlayTime { get; set; }

	[JsonPropertyName("user_id")]
	public required string UserId { get; set; }
}
