using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.G79.RentalGame;

public class EntityRentalGameServerAddress
{
	[JsonPropertyName("mcserver_host")]
	public required string Host { get; set; }

	[JsonPropertyName("mcserver_port")]
	public required int Port { get; set; }
}
