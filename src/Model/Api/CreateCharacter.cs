namespace FromTheForge.Model;

public class CreateCharacter
{
	public string Name { get; set; } = "";
	public string RaceId { get; set; } = "";
	public string ClassId { get; set; } = "";
	public int Strength { get; set; }
	public int Dexterity { get; set; }
	public int Constitution { get; set; }
	public int Intelligence { get; set; }
	public int Wisdom { get; set; }
	public int Charisma { get; set; }
}
