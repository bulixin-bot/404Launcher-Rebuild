using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.G79;

public class EntityUserDetails
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
