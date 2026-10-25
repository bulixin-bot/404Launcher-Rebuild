using System.Text.Json.Serialization;

namespace Codexus.OpenSDK.Entities.X19;

public class X19AuthenticationVersion
{
	[JsonPropertyName("version")]
	public required string Version { get; set; }

	[JsonPropertyName("launcher_md5")]
	public string LauncherMd5 { get; set; } = string.Empty;

	[JsonPropertyName("updater_md5")]
	public string UpdaterMd5 { get; set; } = string.Empty;
}
