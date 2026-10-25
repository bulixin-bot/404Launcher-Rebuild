using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Pc4399;

public record Entity4399Account
{
	[JsonPropertyName("account")]
	public string Account { get; set; } = string.Empty;

	[JsonPropertyName("password")]
	public string Password { get; set; } = string.Empty;
}
