namespace FromTheForge.Model;

public class Spell
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string school { get; set; } = "";
	public int spellLevel { get; set; }
	public int manaCost { get; set; }
}
