public class GoResponseDto
{
	public From from { get; set; }
	public Destination destination { get; set; }
	public Path[] path { get; set; }
	public int totalCells { get; set; }
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}