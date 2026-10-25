using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.RentalGame;

public class EntityDeleteRentalGameRole
{
	[JsonPropertyName("entity_id")]
	public string EntityId { get; set; } = string.Empty;
}
