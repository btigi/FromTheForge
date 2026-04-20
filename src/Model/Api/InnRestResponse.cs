public class InnRestResponse
{
	public bool rested { get; set; }
	public string type { get; set; }
	public string location { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public int hpRestored { get; set; }
	public int manaRestored { get; set; }

	public string error { get; set; }
	public string message { get; set; }
	public int statusCode { get; set; }
}