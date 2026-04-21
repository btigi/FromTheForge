namespace FromTheForge.Model;

public class DungeonRoom
{
	public int index { get; set; }
	public string type { get; set; } = "";
	public bool cleared { get; set; }
	public string[] log { get; set; } = [];
}
