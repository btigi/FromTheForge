public class HarvestResponse
{
	public string harvested { get; set; }
	public string itemId { get; set; }
	public int quantity { get; set; }
	public int xpGained { get; set; }
	public string skill { get; set; }
	public int skillLevel { get; set; }
	public int skillXp { get; set; }
	public int cooldownMinutes { get; set; }
	public DateTime availableAt { get; set; }
	public HarvestLevelUp levelUp { get; set; }

	public string error { get; set; }
	public string message { get; set; }
	public int statusCode { get; set; }
}
