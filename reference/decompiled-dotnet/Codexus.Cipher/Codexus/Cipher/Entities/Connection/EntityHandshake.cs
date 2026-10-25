using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Connection;

public class EntityHandshake
{
	[JsonPropertyName("handshakeBody")]
	public required string HandshakeBody { get; set; }
}
