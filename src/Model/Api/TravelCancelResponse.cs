public class TravelCancelResponse
{
	public bool cancelled { get; set; }
	public Position position { get; set; } = new Position();
	public int cellsTraversed { get; set; }
	public Discovery[] discoveries { get; set; } = [];
	public Xp xp { get; set; } = new Xp();
}
