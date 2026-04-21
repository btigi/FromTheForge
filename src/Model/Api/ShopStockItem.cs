namespace FromTheForge.Model;

public class ShopStockItem
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string type { get; set; } = "";
	public string rarity { get; set; } = "";
	public int price { get; set; }
	public int sellPrice { get; set; }
	public int levelRequired { get; set; }
	public string classRestriction { get; set; } = "";
	public float weight { get; set; }	
	public int stock { get; set; }
	public int maxStock { get; set; }
}
