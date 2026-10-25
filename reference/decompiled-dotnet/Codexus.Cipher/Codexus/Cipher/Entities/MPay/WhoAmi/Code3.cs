using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay.WhoAmi;

public class Code3
{
	[JsonPropertyName("iso_code")]
	public string IsoCode { get; set; }

	[JsonPropertyName("names")]
	public Names Names { get; set; }
}
