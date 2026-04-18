using System.Text;
using System.Text.Json;

const string email = "";
const string password = "";

var jsonString = "";

var options = new JsonSerializerOptions
{
	PropertyNamingPolicy = new CamelCaseNamingPolicy(),
	WriteIndented = true
};

var jsonReadOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


Console.WriteLine(@"  ______                     _   _            ______                   
 |  ____|                   | | | |          |  ____|                  
 | |__ _ __ ___  _ __ ___   | |_| |__   ___  | |__ ___  _ __ __ _  ___ 
 |  __| '__/ _ \| '_ ` _ \  | __| '_ \ / _ \ |  __/ _ \| '__/ _` |/ _ \
 | |  | | | (_) | | | | | | | |_| | | |  __/ | | | (_) | | | (_| |  __/
 |_|  |_|  \___/|_| |_| |_|  \__|_| |_|\___| |_|  \___/|_|  \__, |\___|
                                                             __/ |     
                                                            |___/   ");

var token = string.Empty;

var commands = new Dictionary<string, (string Help, Func<Task> Run)>(StringComparer.OrdinalIgnoreCase);

commands["r"] = ("Register account (built-in email/password)", async () =>
{
	var register = new RegisterDto();
	register.Email = email;
	register.Password = password;
	jsonString = JsonSerializer.Serialize(register, options);
	var registerResponse = await Post<RegisterResponseDto>(jsonString, Constants.Register);
	Console.WriteLine(registerResponse);
}
);

commands["l"] = ("Login; stores bearer token", async () =>
{
	var login = new LoginDto();
	login.Email = email;
	login.Password = password;
	jsonString = JsonSerializer.Serialize(login, options);
	var loginResponse = await Post<LoginResponseDto>(jsonString, Constants.Login);
	if (!string.IsNullOrEmpty(loginResponse.Error))
	{
		Console.WriteLine(loginResponse.Error);
		return;
	}
	token = loginResponse.AccessToken;
	Console.WriteLine($"Logged in as {loginResponse.User.Email}");
}
);

commands["me"] = ("Get authenticated profile", async () =>
{
	var meResponse = await Get<MeResponseDto>(Constants.Me, token);
	Console.WriteLine(meResponse);
}
);

commands["create"] = ("Create character (prompts name, race, class, stats)", async () =>
{
	Console.WriteLine("Name");
	var name = Console.ReadLine();
	Console.WriteLine("Race");
	Console.WriteLine("human, elf, dwarf, halfling, orc");
	var race = Console.ReadLine();
	Console.WriteLine("Class");
	Console.WriteLine("warrior, mage, rogue, cleric, ranger");
	var @class = Console.ReadLine();
	Console.WriteLine("STR");
	var str = Console.ReadLine();
	Console.WriteLine("DEX");
	var dex = Console.ReadLine();
	Console.WriteLine("CON");
	var con = Console.ReadLine();
	Console.WriteLine("INT");
	var intl = Console.ReadLine();
	Console.WriteLine("WIS");
	var wis = Console.ReadLine();
	Console.WriteLine("CHA");
	var cha = Console.ReadLine();
	var createCharacter = new CreateCharacterDto();
	createCharacter.Name = name;
	createCharacter.RaceId = race;
	createCharacter.ClassId = @class;
	createCharacter.Strength = Convert.ToInt32(str);
	createCharacter.Dexterity = Convert.ToInt32(dex);
	createCharacter.Constitution = Convert.ToInt32(con);
	createCharacter.Intelligence = Convert.ToInt32(intl);
	createCharacter.Wisdom = Convert.ToInt32(wis);
	createCharacter.Charisma = Convert.ToInt32(cha);
	jsonString = JsonSerializer.Serialize(createCharacter, options);
	var createCharacterResponse = await Post<CreateCharacterResponseDto>(jsonString, Constants.CreateCharacter, token);
	Console.WriteLine(createCharacterResponse);
}
);

commands["get"] = ("Get my character", async () =>
{
	var getCharacterResponse = await Get<Character>(Constants.GetCharacter, token);
	Console.WriteLine(getCharacterResponse);
}
);

commands["allocate"] = ("Placeholder: allocate stat point on level up", async () =>
{
	Console.WriteLine("allocate a stat point (gained on level up)");
}
);

commands["travel"] = ("Move one step (north/east/south/west)", async () =>
{
	Console.WriteLine("Direction");
	Console.WriteLine("north, east, south, west");
	var direction = Console.ReadLine();
	var travel = new TravelDto();
	travel.Direction = direction;
	jsonString = JsonSerializer.Serialize(travel, options);
	await Post<TravelResponseDto>(jsonString, Constants.Travel, token);
}
);

commands["go"] = ("Pathfind to coordinates", async () =>
{
	Console.WriteLine("X coordinate");
	var x = Console.ReadLine();
	Console.WriteLine("Y coordinate");
	var y = Console.ReadLine();
	var go = new GoDto();
	go.x = Convert.ToInt32(x);
	go.y = Convert.ToInt32(y);
	jsonString = JsonSerializer.Serialize(go, options);
	var goResponse = await Post<GoResponseDto>(jsonString, Constants.Go, token);
	Console.WriteLine(goResponse);
}
);

commands["travelstatus"] = ("Get travel status", async () =>
{
	var travelStatusResponse = await Get<TravelStatusResponseDto>(Constants.TravelStatus, token);
	Console.WriteLine(travelStatusResponse);
}
);

commands["travelcancel"] = ("Cancel active travel", async () =>
{
	var travelCancelResponse = await Post<TravelCancelResponseDto>(string.Empty, Constants.TravelCancel, token);
	Console.WriteLine(travelCancelResponse);
}
);

commands["map"] = ("Fetch world map; print terrain grid + size", async () =>
{
	var mapResponse = await Get<MapResponseDto>(Constants.Map, token);
	if (mapResponse?.terrain != null)
	{
		foreach (var row in mapResponse.terrain)
		{
			if (row != null)
			{
				foreach (var cell in row)
					Console.Write(TerrainGlyph.FromTerrain(cell));
			}
			Console.WriteLine();
		}
	}
	else
		Console.WriteLine("(no terrain grid)");
	if (mapResponse != null)
		Console.WriteLine($"size {mapResponse.width}x{mapResponse.height}");
}
);

commands["regions"] = ("List map regions", async () =>
{
	var regionsResponse = await Get<Region[]>(Constants.Regions, token);
	Console.WriteLine(regionsResponse);
}
);

commands["mapdetail"] = ("Cell detail at x,y", async () =>
{
	Console.WriteLine("X coordinate");
	var xMapDetail = Console.ReadLine();
	Console.WriteLine("Y coordinate");
	var yMapDetail = Console.ReadLine();
	var mapDetailResponse = await Get<MapDetailResponseDto>($"{Constants.MapDetail}/{xMapDetail}/{yMapDetail}", token);
	Console.WriteLine(mapDetailResponse);
}
);

commands["combatstatus"] = ("Get combat status", async () =>
{
	var combatStatusResponse = await Get<CombatStatus>(Constants.CombatStatus, token);
	Console.WriteLine(combatStatusResponse);
}
);

commands["combataction"] = ("POST combat action (attack, cast, use_item, flee)", async () =>
{
	Console.WriteLine("action");
	Console.WriteLine("  attack, cast, use_item, flee");
	var combatAction = Console.ReadLine();
	jsonString = JsonSerializer.Serialize(combatAction, options);
	var combatActionResponse = await Post<CombatActionResult>(jsonString, Constants.CombatAction, token);
	Console.WriteLine(combatActionResponse);
}
);

commands["inventory"] = ("Get inventory", async () =>
{
	var inventoryResponse = await Get<Inventory>(Constants.Inventory, token);
	Console.WriteLine(inventoryResponse);
}
);

commands["pickup"] = ("Pick up item (itemid, quantity)", async () =>
{
	Console.WriteLine("itemid");
	var itemid = Console.ReadLine();
	Console.WriteLine("quantity");
	var quantity = Console.ReadLine();
	var pickup = new Pickup();
	pickup.itemId = itemid;
	pickup.quantity = Convert.ToInt32(quantity);
	jsonString = JsonSerializer.Serialize(pickup, options);
	var pickupResponse = await Post<PickupResponse>(jsonString, Constants.Pickup, token);
	Console.WriteLine(pickupResponse);
}
);

commands["drop"] = ("Drop item", async () =>
{
	Console.WriteLine("itemid");
	var itemidDropped = Console.ReadLine();
	Console.WriteLine("quantity");
	var quantityDropped = Console.ReadLine();
	var drop = new Drop();
	drop.itemId = itemidDropped;
	drop.quantity = Convert.ToInt32(quantityDropped);
	jsonString = JsonSerializer.Serialize(drop, options);
	var droppedResponse = await Post<DropResponse>(jsonString, Constants.Drop, token);
	Console.WriteLine(droppedResponse);
}
);

commands["equip"] = ("Equip item to slot", async () =>
{
	Console.WriteLine("itemid");
	var itemidEquip = Console.ReadLine();
	Console.WriteLine("slot");
	Console.WriteLine("weapon, armor, helmet, shield, leggings, boots, gloves, ring1, ring2, amulet");
	var slot = Console.ReadLine();
	var equip = new Equip();
	equip.itemId = itemidEquip;
	equip.slot = slot;
	jsonString = JsonSerializer.Serialize(equip, options);
	var equipResponse = await Post<EquipResponse>(jsonString, Constants.Equip, token);
	Console.WriteLine(equipResponse);
}
);

commands["unequip"] = ("Unequip slot", async () =>
{
	Console.WriteLine("slot");
	Console.WriteLine("weapon, armor, helmet, shield, leggings, boots, gloves, ring1, ring2, amulet");
	var unequipslot = Console.ReadLine();
	var unequip = new Unequip();
	unequip.slot = unequipslot;
	jsonString = JsonSerializer.Serialize(unequip, options);
	var unequipResponse = await Post<UnequipResponse>(jsonString, Constants.Unequip, token);
	Console.WriteLine(unequipResponse);
}
);

commands["use"] = ("Use consumable", async () =>
{
	Console.WriteLine("itemid");
	var useitem = Console.ReadLine();
	var use = new Use();
	use.itemId = useitem;
	jsonString = JsonSerializer.Serialize(use, options);
	var useResponse = await Post<UseResponse>(jsonString, Constants.Use, token);
	Console.WriteLine(useResponse);
}
);

commands["spellbook"] = ("Get spellbook", async () =>
{
	var spellbookResponse = await Get<SpellbookResponse>(Constants.Spells, token);
	Console.WriteLine(spellbookResponse);
}
);

commands["spelllearn"] = ("Learn spell (spellId)", async () =>
{
	Console.WriteLine("spellId");
	var learnSpellId = Console.ReadLine();
	var learnSpell = new LearnSpell();
	learnSpell.spellId = learnSpellId;
	jsonString = JsonSerializer.Serialize(learnSpell, options);
	var learnSpellResponse = await Post<LearnSpellResponse>(jsonString, Constants.SpellsLearn, token);
	Console.WriteLine(learnSpellResponse);
}
);

commands["spellcast"] = ("Cast spell out of combat (spellId)", async () =>
{
	Console.WriteLine("spellId");
	var castSpellId = Console.ReadLine();
	var castSpell = new CastSpell();
	castSpell.spellId = castSpellId;
	jsonString = JsonSerializer.Serialize(castSpell, options);
	var castSpellResponse = await Post<CastSpellResponse>(jsonString, Constants.SpellsCast, token);
	Console.WriteLine(castSpellResponse);
}
);

commands["restcamp"] = ("Start camping (duration seconds 10-600)", async () =>
{
	Console.WriteLine("duration seconds (10-600)");
	var campDuration = Console.ReadLine();
	var startCamp = new StartCampRequest();
	startCamp.duration = Convert.ToInt32(campDuration);
	jsonString = JsonSerializer.Serialize(startCamp, options);
	var startCampResponse = await Post<StartCampResponse>(jsonString, Constants.RestCamp, token);
	Console.WriteLine(startCampResponse);
}
);

commands["restinn"] = ("Rest at inn", async () =>
{
	var innRestResponse = await Post<InnRestResponse>(string.Empty, Constants.RestInn, token);
	Console.WriteLine(innRestResponse);
}
);

commands["reststatus"] = ("Get rest status", async () =>
{
	var restStatusResponse = await Get<RestStatus>(Constants.RestStatusPath, token);
	Console.WriteLine(restStatusResponse);
}
);

commands["reststop"] = ("Stop resting", async () =>
{
	var stopRestResponse = await Post<StopRestResponse>(string.Empty, Constants.RestStop, token);
	Console.WriteLine(stopRestResponse);
}
);

commands["shop"] = ("Shop inventory (townId)", async () =>
{
	Console.WriteLine("townId");
	var shopTownId = Console.ReadLine();
	var shopResponse = await Get<ShopResponse>($"{Constants.Shops}/{shopTownId}", token);
	Console.WriteLine(shopResponse);
}
);

commands["shopbuy"] = ("Buy from shop", async () =>
{
	Console.WriteLine("townId");
	var buyTownId = Console.ReadLine();
	Console.WriteLine("itemId");
	var buyItemId = Console.ReadLine();
	Console.WriteLine("quantity");
	var buyQtyLine = Console.ReadLine();
	var buyQty = string.IsNullOrWhiteSpace(buyQtyLine) ? 1 : Convert.ToInt32(buyQtyLine);
	var buyItem = new BuyItem();
	buyItem.itemId = buyItemId;
	buyItem.quantity = buyQty;
	jsonString = JsonSerializer.Serialize(buyItem, options);
	var buyResponse = await Post<BuyItemResponse>(jsonString, $"{Constants.Shops}/{buyTownId}/buy", token);
	Console.WriteLine(buyResponse);
}
);

commands["shopsell"] = ("Sell to shop", async () =>
{
	Console.WriteLine("townId");
	var sellTownId = Console.ReadLine();
	Console.WriteLine("itemId");
	var sellItemId = Console.ReadLine();
	Console.WriteLine("quantity");
	var sellQtyLine = Console.ReadLine();
	var sellQty = string.IsNullOrWhiteSpace(sellQtyLine) ? 1 : Convert.ToInt32(sellQtyLine);
	var sellItem = new SellItem();
	sellItem.itemId = sellItemId;
	sellItem.quantity = sellQty;
	jsonString = JsonSerializer.Serialize(sellItem, options);
	var sellResponse = await Post<SellItemResponse>(jsonString, $"{Constants.Shops}/{sellTownId}/sell", token);
	Console.WriteLine(sellResponse);
}
);

commands["questavailable"] = ("Available quests at current town", async () =>
{
	var questAvailableResponse = await Get<AvailableQuestsResponse>(Constants.QuestAvailable, token);
	Console.WriteLine(questAvailableResponse);
}
);

commands["questactive"] = ("Active quests", async () =>
{
	var questActiveResponse = await Get<ActiveQuestsResponse>(Constants.QuestActive, token);
	Console.WriteLine(questActiveResponse);
}
);

commands["questaccept"] = ("Accept quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var acceptQuestId = Console.ReadLine();
	var acceptQuest = new AcceptQuest();
	acceptQuest.questId = acceptQuestId;
	jsonString = JsonSerializer.Serialize(acceptQuest, options);
	var acceptQuestResponse = await Post<AcceptQuestResponse>(jsonString, Constants.QuestAccept, token);
	Console.WriteLine(acceptQuestResponse);
}
);

commands["questturnin"] = ("Turn in quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var turnInQuestId = Console.ReadLine();
	var turnInQuest = new TurnInQuest();
	turnInQuest.questId = turnInQuestId;
	jsonString = JsonSerializer.Serialize(turnInQuest, options);
	var turnInQuestResponse = await Post<TurnInQuestResponse>(jsonString, Constants.QuestTurnIn, token);
	Console.WriteLine(turnInQuestResponse);
}
);

commands["questabandon"] = ("Abandon quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var abandonQuestId = Console.ReadLine();
	var abandonQuest = new AbandonQuest();
	abandonQuest.questId = abandonQuestId;
	jsonString = JsonSerializer.Serialize(abandonQuest, options);
	var abandonQuestResponse = await Post<AbandonQuestResponse>(jsonString, Constants.QuestAbandon, token);
	Console.WriteLine(abandonQuestResponse);
}
);

commands["dungeonstatus"] = ("Dungeon status", async () =>
{
	var dungeonStatusResponse = await Get<DungeonStatus>(Constants.DungeonStatusPath, token);
	Console.WriteLine(dungeonStatusResponse);
}
);

commands["dungeonenter"] = ("Enter dungeon (poiId)", async () =>
{
	Console.WriteLine("poiId");
	var dungeonPoiId = Console.ReadLine();
	var enterDungeon = new EnterDungeon();
	enterDungeon.poiId = dungeonPoiId;
	jsonString = JsonSerializer.Serialize(enterDungeon, options);
	var enterDungeonResponse = await Post<DungeonStatus>(jsonString, Constants.DungeonEnter, token);
	Console.WriteLine(enterDungeonResponse);
}
);

commands["dungeonadvance"] = ("Advance to next dungeon room", async () =>
{
	var dungeonAdvanceResponse = await Post<DungeonAdvanceResponse>(string.Empty, Constants.DungeonAdvance, token);
	Console.WriteLine(dungeonAdvanceResponse);
}
);

commands["dungeonleave"] = ("Leave dungeon", async () =>
{
	var dungeonLeaveResponse = await Post<DungeonLeaveResponse>(string.Empty, Constants.DungeonLeave, token);
	Console.WriteLine(dungeonLeaveResponse);
}
);

commands["gatheringskills"] = ("Gathering skill levels", async () =>
{
	var gatheringSkillsResponse = await Get<GatheringSkillsResponse>(Constants.GatheringSkills, token);
	Console.WriteLine(gatheringSkillsResponse);
}
);

commands["gatheringnodes"] = ("Nearby gathering nodes", async () =>
{
	var gatheringNodesResponse = await Get<GatheringNodesResponse>(Constants.GatheringNodes, token);
	Console.WriteLine(gatheringNodesResponse);
}
);

commands["gatheringharvest"] = ("Harvest node (nodeId)", async () =>
{
	Console.WriteLine("nodeId");
	var harvestNodeId = Console.ReadLine();
	var harvest = new Harvest();
	harvest.nodeId = harvestNodeId;
	jsonString = JsonSerializer.Serialize(harvest, options);
	var harvestResponse = await Post<HarvestResponse>(jsonString, Constants.GatheringHarvest, token);
	Console.WriteLine(harvestResponse);
}
);

commands["craftingskills"] = ("Crafting skill levels", async () =>
{
	var craftingSkillsResponse = await Get<CraftingSkillsResponse>(Constants.CraftingSkills, token);
	Console.WriteLine(craftingSkillsResponse);
}
);

commands["craftingrecipes"] = ("Crafting recipes (optional skill filter)", async () =>
{
	Console.WriteLine("skill filter (blacksmithing, alchemy, woodworking, blank for all)");
	var craftingSkillFilter = Console.ReadLine();
	var recipesUrl = string.IsNullOrWhiteSpace(craftingSkillFilter) ? Constants.CraftingRecipes : $"{Constants.CraftingRecipes}?skill={craftingSkillFilter}";
	var craftingRecipesResponse = await Get<CraftingRecipesResponse>(recipesUrl, token);
	Console.WriteLine(craftingRecipesResponse);
}
);

commands["craftingstations"] = ("Nearby crafting stations", async () =>
{
	var craftingStationsResponse = await Get<CraftingStationsResponse>(Constants.CraftingStations, token);
	Console.WriteLine(craftingStationsResponse);
}
);

commands["craftingcraft"] = ("Craft item (recipeId)", async () =>
{
	Console.WriteLine("recipeId");
	var recipeId = Console.ReadLine();
	var craft = new Craft();
	craft.recipeId = recipeId;
	jsonString = JsonSerializer.Serialize(craft, options);
	var craftResponse = await Post<CraftResponse>(jsonString, Constants.CraftingCraft, token);
	Console.WriteLine(craftResponse);
}
);

commands["gameraces"] = ("Game data: all races", async () =>
{
	var gameRacesResponse = await Get<GameRace[]>(Constants.GameRaces, token);
	foreach (var c in gameRacesResponse.OrderBy(o => o.name).ToList())
	{
		Console.WriteLine(c.id);
		Console.WriteLine($"  {c.name}");
		Console.WriteLine($"  {c.description}");
		//Console.WriteLine($"  {c.bonuses}"); // always null
	}
}
);

commands["gameclasses"] = ("Game data: all classes", async () =>
{
	var gameClassesResponse = await Get<GameClass[]>(Constants.GameClasses, token);
	foreach (var c in gameClassesResponse.OrderBy(o => o.name).ToList())
	{
		Console.WriteLine(c.id);
		Console.WriteLine($"  {c.name}");
		Console.WriteLine($"  {c.description}");
		//Console.WriteLine($"  {c.hitDie}"); // always 0
		//Console.WriteLine($"  {c.primaryStat}"); // always null
	}
}
);

commands["gameitems"] = ("Game data: all items", async () =>
{
	var gameItemsResponse = await Get<GameItemDef[]>(Constants.GameItems, token);
	Console.WriteLine(gameItemsResponse);
}
);

commands["gamespells"] = ("Game data: all spells", async () =>
{
	var gameSpellsResponse = await Get<GameSpellDef[]>(Constants.GameSpells, token);
	Console.WriteLine(gameSpellsResponse);
}
);

commands["help"] = ("Show this command list", () =>
{
	foreach (var kv in commands.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
		Console.WriteLine($"  {kv.Key,-22} {kv.Value.Help}");
	Console.WriteLine("  q                      Quit");
	return Task.CompletedTask;
}
);

Console.Write(">");
var action = Console.ReadLine();
while (action != "q")
{
	if (string.IsNullOrWhiteSpace(action))
	{
		action = Console.ReadLine();
		continue;
	}
	if (!commands.TryGetValue(action.Trim(), out var entry))
		Console.WriteLine($"Unknown command '{action}'. Type help for commands, q to quit.");
	else
		await entry.Run();
	Console.Write(">");
	action = Console.ReadLine();
}

Console.WriteLine("Goodbye");
Console.ReadKey();



async Task<T> Post<T>(string content, string endpoint, string token = "")
{
	try
	{
		using var client = new HttpClient { BaseAddress = new Uri("https://forgebound.io") };
		var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
		if (!string.IsNullOrEmpty(token))
		{
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		}
		var response = await client.PostAsync(endpoint, httpContent);
		var body = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<T>(body, jsonReadOptions);
	}
	catch (Exception ex)
	{
		return default;
	}
}

async Task<T> Get<T>(string endpoint, string token = "")
{
	try
	{
		using var client = new HttpClient { BaseAddress = new Uri("https://forgebound.io") };
		if (!string.IsNullOrEmpty(token))
			client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		var response = await client.GetAsync(endpoint);
		var body = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<T>(body, jsonReadOptions);
	}
	catch (Exception ex)
	{
		return default;
	}
}

file static class TerrainGlyph
{
	public static char FromTerrain(string t)
	{
		if (string.IsNullOrWhiteSpace(t))
			return '_';
		return t.Trim().ToLowerInvariant() switch
		{
			"ocean" => 'o',
			"forest" => 'f',
			"plains" => 'p',
			"coast" => 'c',
			"swamp" => 's',
			"mountain" => 'm',
			"desert" => 'd',
			_ => '_'
		};
	}
}

public class CamelCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name)
	{
		if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
			return name;

		return char.ToLowerInvariant(name[0]) + name[1..];
	}
}