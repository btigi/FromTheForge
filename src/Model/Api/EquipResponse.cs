public class EquipResponse
{
	public Equipped equipped { get; set; } = new Equipped();
	public Unequipped unequipped { get; set; } = new Unequipped();
	public string message { get; set; } = "";
}
