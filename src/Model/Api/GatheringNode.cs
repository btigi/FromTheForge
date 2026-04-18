public class GatheringNode
{
	public string poiId { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string nodeId { get; set; }
	public string skill { get; set; }
	public int minLevel { get; set; }
	public int xpReward { get; set; }
	public int cooldownMinutes { get; set; }
	public bool ready { get; set; }
	public DateTime? availableAt { get; set; }
}
