namespace FromTheForge.Model;

public class ActiveQuest
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string description { get; set; } = "";
	public string giverTown { get; set; } = "";
	public string status { get; set; } = "";
	public QuestObjective[] objectives { get; set; } = [];
	public QuestRewards rewards { get; set; } = new QuestRewards();
	public DateTime acceptedAt { get; set; }
	public DateTime? completedAt { get; set; }
}
