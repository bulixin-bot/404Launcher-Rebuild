using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities;

public class EntityResponse
{
	[JsonPropertyName("code")]
	public int Code { get; set; }

	[JsonPropertyName("message")]
	public string Message { get; set; } = string.Empty;
}
