using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.NetGame;

public class EntityQuerySearchByGameResponse
{
	[JsonPropertyName("mc_version_id")]
	public required int McVersionId { get; set; }

	[JsonPropertyName("game_type")]
	public required int GameType { get; set; }

	[JsonPropertyName("iid_list")]
	public required List<ulong> IidList { get; set; }
}
