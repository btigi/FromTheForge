public class AcceptedQuest
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string status { get; set; }
	public QuestObjective[] objectives { get; set; }
	public QuestRewards rewards { get; set; }
}
