using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher;

public class EntityX19CookieRequest
{
	[JsonPropertyName("sauth_json")]
	public required string Json { get; set; }
}
