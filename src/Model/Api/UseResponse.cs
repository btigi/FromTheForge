namespace FromTheForge.Model;

public class UseResponse
{
	public string used { get; set; } = "";
	public string message { get; set; } = "";
	public Effect effect { get; set; } = new Effect();
	public string result { get; set; } = "";
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public string learned { get; set; } = "";
}
