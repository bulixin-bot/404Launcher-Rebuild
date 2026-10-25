using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.MPay;

public class EntityDeviceResponse
{
	[JsonPropertyName("device")]
	public EntityDevice EntityDevice { get; set; } = new EntityDevice();
}
