using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.NetGame;

public class EntityQueryNetGameItem
{
	[JsonPropertyName("title_image_url")]
	public string TitleImageUrl { get; set; } = string.Empty;
}
