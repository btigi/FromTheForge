public class AvailableQuest
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string description { get; set; } = "";
	public int levelMin { get; set; }
	public int levelMax { get; set; }
	public QuestObjective[] objectives { get; set; }
	public QuestRewards rewards { get; set; }
}
