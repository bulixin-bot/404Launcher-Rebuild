using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.Minecraft;

public class EntityMcDownloadVersion
{
	[JsonPropertyName("mc_version")]
	public required int McVersion { get; set; }
}
