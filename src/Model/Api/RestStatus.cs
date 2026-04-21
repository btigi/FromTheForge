namespace FromTheForge.Model;

public class RestStatus
{
	public bool resting { get; set; }
	public string type { get; set; } = "";
	public DateTime startedAt { get; set; }
	public DateTime until { get; set; }
	public double elapsedSeconds { get; set; }
	public int totalDurationSeconds { get; set; }
	public int currentHp { get; set; }
	public int currentMana { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public RestRecoveryAmount recovered { get; set; } = new RestRecoveryAmount();
}
