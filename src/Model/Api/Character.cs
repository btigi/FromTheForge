namespace FromTheForge.Model;

public class Character
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public Race race { get; set; } = new Race();
	public @Class @class { get; set; } = new @Class();
	public int level { get; set; }
	public int xp { get; set; }
	public int xpToNextLevel { get; set; }
	public int unspentStatPoints { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int ac { get; set; }
	public int strength { get; set; }
	public int dexterity { get; set; }
	public int constitution { get; set; }
	public int intelligence { get; set; }
	public int wisdom { get; set; }
	public int charisma { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public int gold { get; set; }
	public Position position { get; set; } = new Position();
	public bool inCombat { get; set; }
	public bool inDungeon { get; set; }
	public DateTime createdAt { get; set; }
	public Effectivestats effectiveStats { get; set; } = new Effectivestats();
	public Equipment equipment { get; set; } = new Equipment();
	public Spell[] spells { get; set; } = [];
	public int carryCapacity { get; set; }
	public Travel travel { get; set; } = new Travel();
	public Discovery[] discoveries { get; set; } = [];
	public Xp_Extra xp_extra { get; set; } = new Xp_Extra();
	public Encounter encounter { get; set; } = new Encounter();
}
