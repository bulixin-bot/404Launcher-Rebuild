using System;
using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.G79;

public class EntityQueryUserDetail
{
	[JsonPropertyName("version")]
	public required Version Version { get; set; }
}
