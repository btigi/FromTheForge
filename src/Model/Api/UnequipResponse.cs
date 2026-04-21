namespace FromTheForge.Model;

public class UnequipResponse
{
	public Unequipped unequipped { get; set; } = new Unequipped();
	public string message { get; set; } = "";
}
