using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Connection.G79;

public class EntityZkpGetCheckNum
{
	[JsonPropertyName("body")]
	public required string Body { get; set; }
}
