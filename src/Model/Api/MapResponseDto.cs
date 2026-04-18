public class MapResponseDto
{
	public int width { get; set; }
	public int height { get; set; }
	public string[][] terrain { get; set; }
	public Pois[] pois { get; set; }
	public Discovery[] discoveries { get; set; }
}
