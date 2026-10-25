using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay.WhoAmi;

public class Names
{
	[JsonPropertyName("en")]
	public string En { get; set; }
}
