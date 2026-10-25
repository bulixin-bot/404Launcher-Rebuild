using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.G79;

public class EntitySetNickNameRequest
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("entity_id")]
	public required string EntityId { get; set; }
}
