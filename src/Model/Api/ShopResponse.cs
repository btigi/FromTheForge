namespace FromTheForge.Model;

public class ShopResponse
{
	public string townId { get; set; } = "";
	public string shop { get; set; } = "";
	public string description { get; set; } = "";
	public int restockMinutes { get; set; }
	public int gold { get; set; }
	public ShopStockItem[] items { get; set; } = [];

	public string error { get; set; } = "";
	public string message { get; set; } = "";
	public int statusCode { get; set; }
}
