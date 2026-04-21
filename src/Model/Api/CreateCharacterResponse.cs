public class CreateCharacterResponse
{
	public string Error { get; set; } = "";
	public string[] Message { get; set; }

	public string id { get; set; } = "";
	public string user_id { get; set; } = "";
	public string name { get; set; } = "";
	public string race_id { get; set; } = "";
	public string class_id { get; set; } = "";
	public int level { get; set; }
	public int xp { get; set; }
	public int hp { get; set; }
	public int max_hp { get; set; }
	public int mana { get; set; }
	public int max_mana { get; set; }
	public int ac { get; set; }
	public int gold { get; set; }
	public int strength { get; set; }
	public int dexterity { get; set; }
	public int constitution { get; set; }
	public int intelligence { get; set; }
	public int wisdom { get; set; }
	public int charisma { get; set; }
	public int pos_x { get; set; }
	public int pos_y { get; set; }
	public Race race { get; set; }
	public @Class @class { get; set; }
}
