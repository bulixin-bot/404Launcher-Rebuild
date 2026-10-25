using System.Text.Json.Serialization;
using Codexus.Cipher.Entities.Converter;

namespace Codexus.Cipher.Entities.G79;

public class Entities<T> : EntityResponse
{
	[JsonPropertyName("details")]
	public string Details { get; set; } = string.Empty;

	[JsonPropertyName("entity")]
	public T? Data { get; set; }

	[JsonPropertyName("total")]
	[JsonConverter(typeof(NetEaseStringConverter))]
	public int Total { get; set; }
}
