public class LearnSpellResponse
{
	public string learned { get; set; } = "";
	public string message { get; set; } = "";
	public KnownSpell spell { get; set; } = new KnownSpell();
}
