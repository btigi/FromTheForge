public class CombatStatus
{
	public bool inCombat { get; set; }
	public int turn { get; set; }
	public string source { get; set; }
	public Monster monster { get; set; }
	public Player player { get; set; }
	public string[] log { get; set; }
}
