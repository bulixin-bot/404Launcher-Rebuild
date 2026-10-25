using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.NetGame;

public class EntityQuerySearchByGameRequest
{
	[JsonPropertyName("mc_version_id")]
	public required int McVersionId { get; set; }

	[JsonPropertyName("game_type")]
	public required int GameType { get; set; }
}
