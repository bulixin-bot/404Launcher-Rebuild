using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Connection.G79;

public class EntityHttpEncrypt
{
	[JsonPropertyName("body")]
	public required string Body { get; set; }
}
