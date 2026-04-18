public class TurnInQuestRewards
{
	public int xp { get; set; }
	public int gold { get; set; }
	public TurnInQuestRewardItem[] items { get; set; }
	public bool leveledUp { get; set; }
	public int newLevel { get; set; }
	public Newspell[] newSpells { get; set; }
}
