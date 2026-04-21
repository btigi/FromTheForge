using System.Text.Json;
using System.Text.Json.Serialization;

public class GameSpellDef
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	[JsonPropertyName("class")]
	public string spellClass { get; set; } = "";
	public int level_req { get; set; }
	public int mana_cost { get; set; }
	public string targetType { get; set; } = "";
	public string effectType { get; set; } = "";
	public double effectValue { get; set; }
	public string damageType { get; set; } = "";
	public int cooldown { get; set; }
	public string description { get; set; } = "";
	public JsonElement? statusEffect { get; set; }
}