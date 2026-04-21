namespace FromTheForge.Model;

public class TravelResponse
{
	public string direction { get; set; } = "";
	public From from { get; set; } = new From();
	public Destination destination { get; set; } = new Destination();
	public string terrain { get; set; } = "";
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }

	public string error { get; set; } = "";
	public string message { get; set; } = "";
	public int statusCode { get; set; }
}
