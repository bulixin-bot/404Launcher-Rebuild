using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay.WhoAmi;

public class Code1
{
	[JsonPropertyName("code")]
	public string Code { get; set; }

	[JsonPropertyName("names")]
	public Names Names { get; set; }
}
