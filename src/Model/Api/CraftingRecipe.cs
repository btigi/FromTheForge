namespace FromTheForge.Model;

public class CraftingRecipe
{
	public string id { get; set; } = "";
	public string name { get; set; } = "";
	public string skill { get; set; } = "";
	public int minLevel { get; set; }
	public int xpReward { get; set; }
	public CraftingIngredient[] ingredients { get; set; } = [];
	public CraftingOutput output { get; set; } = new CraftingOutput();
	public bool meetsLevel { get; set; }
	public bool hasIngredients { get; set; }
	public bool canCraft { get; set; }
}
