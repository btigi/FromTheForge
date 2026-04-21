namespace FromTheForge.Model;

public class Travel
{
	public bool isTraveling { get; set; }
	public Position position { get; set; } = new Position();
	public Estimatedcurrent estimatedCurrent { get; set; } = new Estimatedcurrent();
	public Destination destination { get; set; } = new Destination();
	public Path[] path { get; set; } = [];
	public int progressIndex { get; set; }
	public int totalCells { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}
