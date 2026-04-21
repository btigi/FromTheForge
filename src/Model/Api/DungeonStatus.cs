namespace FromTheForge.Model;

public class DungeonStatus
{
	public bool inDungeon { get; set; }
	public string dungeonId { get; set; } = "";
	public string poiId { get; set; } = "";
	public string poiName { get; set; } = "";
	public int dungeonLevel { get; set; }
	public int currentRoom { get; set; }
	public int totalRooms { get; set; }
	public bool completed { get; set; }
	public DungeonRoom[] rooms { get; set; } = [];

	public string error { get; set; } = "";
	public string message { get; set; } = "";
	public int statusCode { get; set; }
}
