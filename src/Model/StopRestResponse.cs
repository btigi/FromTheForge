public class StopRestResponse
{
	public bool stopped { get; set; }
	public bool completed { get; set; }
	public double elapsedSeconds { get; set; }
	public RestRecoveryAmount recovered { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
}
