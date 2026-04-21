namespace FromTheForge.Model;

public class CombatStatus
{
	public bool inCombat { get; set; }
	public int turn { get; set; }
	public string source { get; set; } = "";
	public Monster monster { get; set; } = new Monster();
	public Player player { get; set; } = new Player();
	public string[] log { get; set; } = [];
}
