using System.Text.Json;

public class GameRace
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string description { get; set; } = "";
	public JsonElement? bonuses { get; set; }
}
