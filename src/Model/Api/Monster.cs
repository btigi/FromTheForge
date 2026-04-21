namespace FromTheForge.Model;

public class Monster
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public int level { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int ac { get; set; }
	public string type { get; set; } = "";
	public Effect[] effects { get; set; } = [];
}
