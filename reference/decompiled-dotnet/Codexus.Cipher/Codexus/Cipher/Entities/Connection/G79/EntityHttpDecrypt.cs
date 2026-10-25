using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Connection.G79;

public class EntityHttpDecrypt
{
	[JsonPropertyName("body")]
	public required string Body { get; set; }
}
