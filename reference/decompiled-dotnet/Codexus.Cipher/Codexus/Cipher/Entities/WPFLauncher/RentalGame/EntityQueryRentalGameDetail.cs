using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.RentalGame;

public class EntityQueryRentalGameDetail
{
	[JsonPropertyName("server_id")]
	public string ServerId { get; set; } = string.Empty;
}
