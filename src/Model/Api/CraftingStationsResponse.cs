namespace FromTheForge.Model;

public class CraftingStationsResponse
{
	public Position position { get; set; } = new Position();
	public CraftingStation[] stations { get; set; } = [];
}
