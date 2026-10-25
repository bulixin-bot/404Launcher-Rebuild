using System.Text.Json;

namespace Codexus.OpenSDK.Entities.Yggdrasil;

public class GameProfile
{
	public required string GameId { get; set; }

	public required string GameVersion { get; set; }

	public required string BootstrapMd5 { get; set; }

	public required string DatFileMd5 { get; set; }

	public required ModList Mods { get; set; }

	public required UserProfile User { get; set; }

	public string GetModInfo()
	{
		return JsonSerializer.Serialize(Mods);
	}
}
