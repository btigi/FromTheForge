public class AllocateResponse
{
	public string allocated { get; set; } = "";
	public int newValue { get; set; }
	public int unspentStatPoints { get; set; }
	public string message { get; set; } = "";
	public int maxHp { get; set; }
	public int hp { get; set; }
	public int maxMana { get; set; }
	public int mana { get; set; }
}