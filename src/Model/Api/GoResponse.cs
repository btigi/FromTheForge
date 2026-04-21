public class GoResponse
{
	public From from { get; set; } = new From();
	public Destination destination { get; set; } = new Destination();
	public Path[] path { get; set; } = [];
	public int totalCells { get; set; }
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }

	public string error { get; set; } = "";
	public string message { get; set; } = "";
	public int statusCode { get; set; }
}