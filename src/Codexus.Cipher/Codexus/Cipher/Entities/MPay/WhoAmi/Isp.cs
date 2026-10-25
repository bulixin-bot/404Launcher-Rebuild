using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay.WhoAmi;

public class Isp
{
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("names")]
	public Names Names { get; set; }
}
