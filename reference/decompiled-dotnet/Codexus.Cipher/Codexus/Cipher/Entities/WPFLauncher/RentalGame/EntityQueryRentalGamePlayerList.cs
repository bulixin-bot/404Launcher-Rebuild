using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.RentalGame;

public class EntityQueryRentalGamePlayerList
{
	[JsonPropertyName("server_id")]
	public string ServerId { get; set; } = string.Empty;

	[JsonPropertyName("offset")]
	public int Offset { get; set; }

	[JsonPropertyName("length")]
	public int Length { get; set; }
}
