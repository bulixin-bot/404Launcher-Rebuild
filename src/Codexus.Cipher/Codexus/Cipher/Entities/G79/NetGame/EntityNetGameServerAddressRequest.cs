using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.G79.NetGame;

public class EntityNetGameServerAddressRequest
{
	[JsonPropertyName("item_id")]
	public required string ItemId { get; set; }
}
