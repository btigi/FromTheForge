namespace FromTheForge.Model;

public class Inventory
{
	public Equipment equipment { get; set; } = new Equipment();
	public Backpack[] backpack { get; set; } = [];
	public float carryWeight { get; set; }
	public float carryCapacity { get; set; }
}
