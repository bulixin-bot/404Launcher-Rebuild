using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Codexus.OpenSDK.Entities.Yggdrasil;

public class ModList
{
	[JsonPropertyName("mods")]
	public List<Mod> Mods { get; set; } = new List<Mod>();
}
