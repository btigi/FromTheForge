public class DungeonAdvanceResponse
{
	public int roomIndex { get; set; }
	public string roomType { get; set; }
	public string status { get; set; }
	public string message { get; set; }
	public DungeonAdvanceMonster monster { get; set; }
	public string trapType { get; set; }
	public bool avoided { get; set; }
	public int gold { get; set; }
	public QuestRewardItem[] loot { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public RestRecoveryAmount recovered { get; set; }
	public string[] log { get; set; }

	public string error { get; set; }
	public int statusCode { get; set; }
}
