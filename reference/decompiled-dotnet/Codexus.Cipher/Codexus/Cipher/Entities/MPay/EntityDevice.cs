using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay;

public class EntityDevice
{
	[JsonPropertyName("id")]
	public string Id { get; set; } = string.Empty;

	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
}
