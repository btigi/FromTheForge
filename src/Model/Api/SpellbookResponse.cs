namespace FromTheForge.Model;

public class SpellbookResponse
{
	public int mana { get; set; }
	public int maxMana { get; set; }
	public KnownSpell[] spells { get; set; } = [];
}
