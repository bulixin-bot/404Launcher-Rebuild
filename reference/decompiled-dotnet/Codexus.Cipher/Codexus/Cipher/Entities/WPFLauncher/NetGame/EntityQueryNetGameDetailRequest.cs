using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.NetGame;

public class EntityQueryNetGameDetailRequest
{
	[JsonPropertyName("item_id")]
	public required string ItemId { get; set; }
}
