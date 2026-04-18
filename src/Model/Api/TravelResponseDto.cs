public class TravelResponseDto
{
	public string direction { get; set; }
	public From from { get; set; }
	public Destination destination { get; set; }
	public string terrain { get; set; }
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}
