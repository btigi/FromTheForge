public class TravelCancelResponseDto
{
	public bool cancelled { get; set; }
	public Position position { get; set; }
	public int cellsTraversed { get; set; }
	public Discovery[] discoveries { get; set; }
	public Xp xp { get; set; }
}
