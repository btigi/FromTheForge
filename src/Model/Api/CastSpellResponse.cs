namespace FromTheForge.Model;

public class CastSpellResponse
{
	public string cast { get; set; } = "";
	public int manaCost { get; set; }
	public Effect effect { get; set; } = new Effect();
	public string result { get; set; } = "";

	public string error { get; set; } = "";
	public string message { get; set; } = "";
	public int statusCode { get; set; }
}
