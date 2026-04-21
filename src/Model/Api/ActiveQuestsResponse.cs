namespace FromTheForge.Model;

public class ActiveQuestsResponse
{
	public int activeCount { get; set; }
	public int maxActive { get; set; }
	public ActiveQuest[] quests { get; set; } = [];
}
