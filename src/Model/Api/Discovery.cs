namespace FromTheForge.Model;

public class Discovery
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string type { get; set; } = "";
	public string category { get; set; } = "";
	public int x { get; set; }
	public int y { get; set; }
	public string terrain { get; set; } = "";
	public string description { get; set; } = "";
	public int? level_min { get; set; }
	public int? level_max { get; set; }
	public DateTime discoveredAt { get; set; }
}
