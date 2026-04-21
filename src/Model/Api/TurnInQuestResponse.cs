namespace FromTheForge.Model;

public class TurnInQuestResponse
{
	public string message { get; set; } = "";
	public TurnInQuestRewards rewards { get; set; } = new TurnInQuestRewards();
}
