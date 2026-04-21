namespace FromTheForge.Model;

public class MapResponse
{
	public int width { get; set; }
	public int height { get; set; }
	public required string[][] terrain { get; set; }
	public Poi[] pois { get; set; } = [];
	public Discovery[] discoveries { get; set; } = [];
}
