using System.Text.Json;

namespace FromTheForge.Model;

public class GameItemDef
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string type { get; set; } = "";
	public string rarity { get; set; } = "";
	public int? damage_min { get; set; }
	public int? damage_max { get; set; }
	public int armor { get; set; }
	public int value { get; set; }
	public double weight { get; set; }
	public int level_req { get; set; }
	public string class_req { get; set; } = "";
	public JsonElement? statusEffect { get; set; }
	public double? statusChance { get; set; }
}
