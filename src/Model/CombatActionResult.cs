public class CombatActionResult
{
	public string outcome { get; set; }
	public int turn { get; set; }
	public string[] log { get; set; }
	public Monster monster { get; set; }
	public Player player { get; set; }
	public int xp { get; set; }
	public int gold { get; set; }
	public Item[] items { get; set; }
	public Levelup levelUp { get; set; }
	public int goldLost { get; set; }
	public int xpLost { get; set; }
	public int hp { get; set; }
	public string message { get; set; }
}
