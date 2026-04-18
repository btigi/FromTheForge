public class TravelStatusResponseDto
{
	public bool isTraveling { get; set; }
	public Position position { get; set; }
	public Estimatedcurrent estimatedCurrent { get; set; }
	public Destination destination { get; set; }
	public Path[] path { get; set; }
	public int progressIndex { get; set; }
	public int totalCells { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
	public Discovery[] discoveries { get; set; }
	public Xp xp { get; set; }
	public Encounter encounter { get; set; }
}
