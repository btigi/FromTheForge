using System.Text;
using System.Text.Json;

const string email = "";
const string password = "";

const string Register = "/api/auth/register";
const string Login = "/api/auth/login";
const string Me = "/api/auth/me";
const string CreateCharacter = "/api/characters";
const string GetCharacter = "/api/characters/me";
const string Travel = "/api/travel/move";
const string Go = "/api/travel/go";
const string TravelStatus = "/api/travel/status";
const string TravelCancel = "/api/travel/cancel";
const string Map = "/api/map";
const string Regions = "/api/map/regions";
const string MapDetail = "/api/map/cell";
const string CombatStatus = "/api/combat/status";
const string CombatAction = "/api/combat/action";
const string Inventory = "/api/inventory";
const string Pickup = "/api/inventory/pick-up";
const string Drop = "/api/inventory/drop";
const string Equip = "/api/inventory/equip";
const string Unequip = "/api/inventory/unequip";
const string Use = "/api/inventory/use";
const string Spells = "/api/spells";
const string SpellsLearn = "/api/spells/learn";
const string SpellsCast = "/api/spells/cast";
const string RestCamp = "/api/rest/camp";
const string RestInn = "/api/rest/inn";
const string RestStatusPath = "/api/rest/status";
const string RestStop = "/api/rest/stop";
const string Shops = "/api/shops";
const string QuestAvailable = "/api/quests/available";
const string QuestActive = "/api/quests/active";
const string QuestAccept = "/api/quests/accept";
const string QuestTurnIn = "/api/quests/turn-in";
const string QuestAbandon = "/api/quests/abandon";
const string DungeonStatusPath = "/api/dungeons/status";
const string DungeonEnter = "/api/dungeons/enter";
const string DungeonAdvance = "/api/dungeons/advance";
const string DungeonLeave = "/api/dungeons/leave";
const string GatheringSkills = "/api/gathering/skills";
const string GatheringNodes = "/api/gathering/nodes";
const string GatheringHarvest = "/api/gathering/harvest";
const string CraftingSkills = "/api/crafting/skills";
const string CraftingRecipes = "/api/crafting/recipes";
const string CraftingStations = "/api/crafting/stations";
const string CraftingCraft = "/api/crafting/craft";
const string GameRaces = "/api/game/races";
const string GameClasses = "/api/game/classes";
const string GameItems = "/api/game/items";
const string GameSpells = "/api/game/spells";

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
	var registerResponse = await Post<RegisterResponseDto>(jsonString, Register);
	Console.WriteLine(registerResponse);
});

commands["l"] = ("Login; stores bearer token", async () =>
{
	var login = new LoginDto();
	login.Email = email;
	login.Password = password;
	jsonString = JsonSerializer.Serialize(login, options);
	var loginResponse = await Post<LoginResponseDto>(jsonString, Login);
	token = loginResponse.AccessToken;
	Console.WriteLine(loginResponse);
});

commands["me"] = ("Get authenticated profile", async () =>
{
	var meResponse = await Get<MeResponseDto>(Me, token);
	Console.WriteLine(meResponse);
});

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
	var createCharacterResponse = await Post<CreateCharacterResponseDto>(jsonString, CreateCharacter, token);
	Console.WriteLine(createCharacterResponse);
});

commands["get"] = ("Get my character", async () =>
{
	var getCharacterResponse = await Get<Character>(GetCharacter, token);
	Console.WriteLine(getCharacterResponse);
});

commands["allocate"] = ("Placeholder: allocate stat point on level up", async () =>
{
	Console.WriteLine("allocate a stat point (gained on level up)");
});

commands["travel"] = ("Move one step (north/east/south/west)", async () =>
{
	Console.WriteLine("Direction");
	Console.WriteLine("north, east, south, west");
	var direction = Console.ReadLine();
	var travel = new TravelDto();
	travel.Direction = direction;
	jsonString = JsonSerializer.Serialize(travel, options);
	await Post<TravelResponseDto>(jsonString, Travel, token);
});

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
	var goResponse = await Post<GoResponseDto>(jsonString, Go, token);
	Console.WriteLine(goResponse);
});

commands["travelstatus"] = ("Get travel status", async () =>
{
	var travelStatusResponse = await Get<TravelStatusResponseDto>(TravelStatus, token);
	Console.WriteLine(travelStatusResponse);
});

commands["travelcancel"] = ("Cancel active travel", async () =>
{
	var travelCancelResponse = await Post<TravelCancelResponseDto>(string.Empty, TravelCancel, token);
	Console.WriteLine(travelCancelResponse);
});

commands["map"] = ("Fetch world map; print terrain grid + size", async () =>
{
	var mapResponse = await Get<MapResponseDto>(Map, token);
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
});

commands["regions"] = ("List map regions", async () =>
{
	var regionsResponse = await Get<Region[]>(Regions, token);
	Console.WriteLine(regionsResponse);
});

commands["mapdetail"] = ("Cell detail at x,y", async () =>
{
	Console.WriteLine("X coordinate");
	var xMapDetail = Console.ReadLine();
	Console.WriteLine("Y coordinate");
	var yMapDetail = Console.ReadLine();
	var mapDetailResponse = await Get<MapDetailResponseDto>($"{MapDetail}/{xMapDetail}/{yMapDetail}", token);
	Console.WriteLine(mapDetailResponse);
});

commands["combatstatus"] = ("Get combat status", async () =>
{
	var combatStatusResponse = await Get<CombatStatus>(CombatStatus, token);
	Console.WriteLine(combatStatusResponse);
});

commands["combataction"] = ("POST combat action (attack, cast, use_item, flee)", async () =>
{
	Console.WriteLine("action");
	Console.WriteLine("  attack, cast, use_item, flee");
	var combatAction = Console.ReadLine();
	jsonString = JsonSerializer.Serialize(combatAction, options);
	var combatActionResponse = await Post<CombatActionResult>(jsonString, CombatAction, token);
	Console.WriteLine(combatActionResponse);
});

commands["inventory"] = ("Get inventory", async () =>
{
	var inventoryResponse = await Get<Inventory>(Inventory, token);
	Console.WriteLine(inventoryResponse);
});

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
	var pickupResponse = await Post<PickupResponse>(jsonString, Pickup, token);
	Console.WriteLine(pickupResponse);
});

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
	var droppedResponse = await Post<DropResponse>(jsonString, Drop, token);
	Console.WriteLine(droppedResponse);
});

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
	var equipResponse = await Post<EquipResponse>(jsonString, Equip, token);
	Console.WriteLine(equipResponse);
});

commands["unequip"] = ("Unequip slot", async () =>
{
	Console.WriteLine("slot");
	Console.WriteLine("weapon, armor, helmet, shield, leggings, boots, gloves, ring1, ring2, amulet");
	var unequipslot = Console.ReadLine();
	var unequip = new Unequip();
	unequip.slot = unequipslot;
	jsonString = JsonSerializer.Serialize(unequip, options);
	var unequipResponse = await Post<UnequipResponse>(jsonString, Unequip, token);
	Console.WriteLine(unequipResponse);
});

commands["use"] = ("Use consumable", async () =>
{
	Console.WriteLine("itemid");
	var useitem = Console.ReadLine();
	var use = new Use();
	use.itemId = useitem;
	jsonString = JsonSerializer.Serialize(use, options);
	var useResponse = await Post<UseResponse>(jsonString, Use, token);
	Console.WriteLine(useResponse);
});

commands["spellbook"] = ("Get spellbook", async () =>
{
	var spellbookResponse = await Get<SpellbookResponse>(Spells, token);
	Console.WriteLine(spellbookResponse);
});

commands["spelllearn"] = ("Learn spell (spellId)", async () =>
{
	Console.WriteLine("spellId");
	var learnSpellId = Console.ReadLine();
	var learnSpell = new LearnSpell();
	learnSpell.spellId = learnSpellId;
	jsonString = JsonSerializer.Serialize(learnSpell, options);
	var learnSpellResponse = await Post<LearnSpellResponse>(jsonString, SpellsLearn, token);
	Console.WriteLine(learnSpellResponse);
});

commands["spellcast"] = ("Cast spell out of combat (spellId)", async () =>
{
	Console.WriteLine("spellId");
	var castSpellId = Console.ReadLine();
	var castSpell = new CastSpell();
	castSpell.spellId = castSpellId;
	jsonString = JsonSerializer.Serialize(castSpell, options);
	var castSpellResponse = await Post<CastSpellResponse>(jsonString, SpellsCast, token);
	Console.WriteLine(castSpellResponse);
});

commands["restcamp"] = ("Start camping (duration seconds 10-600)", async () =>
{
	Console.WriteLine("duration seconds (10-600)");
	var campDuration = Console.ReadLine();
	var startCamp = new StartCampRequest();
	startCamp.duration = Convert.ToInt32(campDuration);
	jsonString = JsonSerializer.Serialize(startCamp, options);
	var startCampResponse = await Post<StartCampResponse>(jsonString, RestCamp, token);
	Console.WriteLine(startCampResponse);
});

commands["restinn"] = ("Rest at inn", async () =>
{
	var innRestResponse = await Post<InnRestResponse>(string.Empty, RestInn, token);
	Console.WriteLine(innRestResponse);
});

commands["reststatus"] = ("Get rest status", async () =>
{
	var restStatusResponse = await Get<RestStatus>(RestStatusPath, token);
	Console.WriteLine(restStatusResponse);
});

commands["reststop"] = ("Stop resting", async () =>
{
	var stopRestResponse = await Post<StopRestResponse>(string.Empty, RestStop, token);
	Console.WriteLine(stopRestResponse);
});

commands["shop"] = ("Shop inventory (townId)", async () =>
{
	Console.WriteLine("townId");
	var shopTownId = Console.ReadLine();
	var shopResponse = await Get<ShopResponse>($"{Shops}/{shopTownId}", token);
	Console.WriteLine(shopResponse);
});

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
	var buyResponse = await Post<BuyItemResponse>(jsonString, $"{Shops}/{buyTownId}/buy", token);
	Console.WriteLine(buyResponse);
});

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
	var sellResponse = await Post<SellItemResponse>(jsonString, $"{Shops}/{sellTownId}/sell", token);
	Console.WriteLine(sellResponse);
});

commands["questavailable"] = ("Available quests at current town", async () =>
{
	var questAvailableResponse = await Get<AvailableQuestsResponse>(QuestAvailable, token);
	Console.WriteLine(questAvailableResponse);
});

commands["questactive"] = ("Active quests", async () =>
{
	var questActiveResponse = await Get<ActiveQuestsResponse>(QuestActive, token);
	Console.WriteLine(questActiveResponse);
});

commands["questaccept"] = ("Accept quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var acceptQuestId = Console.ReadLine();
	var acceptQuest = new AcceptQuest();
	acceptQuest.questId = acceptQuestId;
	jsonString = JsonSerializer.Serialize(acceptQuest, options);
	var acceptQuestResponse = await Post<AcceptQuestResponse>(jsonString, QuestAccept, token);
	Console.WriteLine(acceptQuestResponse);
});

commands["questturnin"] = ("Turn in quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var turnInQuestId = Console.ReadLine();
	var turnInQuest = new TurnInQuest();
	turnInQuest.questId = turnInQuestId;
	jsonString = JsonSerializer.Serialize(turnInQuest, options);
	var turnInQuestResponse = await Post<TurnInQuestResponse>(jsonString, QuestTurnIn, token);
	Console.WriteLine(turnInQuestResponse);
});

commands["questabandon"] = ("Abandon quest (questId)", async () =>
{
	Console.WriteLine("questId");
	var abandonQuestId = Console.ReadLine();
	var abandonQuest = new AbandonQuest();
	abandonQuest.questId = abandonQuestId;
	jsonString = JsonSerializer.Serialize(abandonQuest, options);
	var abandonQuestResponse = await Post<AbandonQuestResponse>(jsonString, QuestAbandon, token);
	Console.WriteLine(abandonQuestResponse);
});

commands["dungeonstatus"] = ("Dungeon status", async () =>
{
	var dungeonStatusResponse = await Get<DungeonStatus>(DungeonStatusPath, token);
	Console.WriteLine(dungeonStatusResponse);
});

commands["dungeonenter"] = ("Enter dungeon (poiId)", async () =>
{
	Console.WriteLine("poiId");
	var dungeonPoiId = Console.ReadLine();
	var enterDungeon = new EnterDungeon();
	enterDungeon.poiId = dungeonPoiId;
	jsonString = JsonSerializer.Serialize(enterDungeon, options);
	var enterDungeonResponse = await Post<DungeonStatus>(jsonString, DungeonEnter, token);
	Console.WriteLine(enterDungeonResponse);
});

commands["dungeonadvance"] = ("Advance to next dungeon room", async () =>
{
	var dungeonAdvanceResponse = await Post<DungeonAdvanceResponse>(string.Empty, DungeonAdvance, token);
	Console.WriteLine(dungeonAdvanceResponse);
});

commands["dungeonleave"] = ("Leave dungeon", async () =>
{
	var dungeonLeaveResponse = await Post<DungeonLeaveResponse>(string.Empty, DungeonLeave, token);
	Console.WriteLine(dungeonLeaveResponse);
});

commands["gatheringskills"] = ("Gathering skill levels", async () =>
{
	var gatheringSkillsResponse = await Get<GatheringSkillsResponse>(GatheringSkills, token);
	Console.WriteLine(gatheringSkillsResponse);
});

commands["gatheringnodes"] = ("Nearby gathering nodes", async () =>
{
	var gatheringNodesResponse = await Get<GatheringNodesResponse>(GatheringNodes, token);
	Console.WriteLine(gatheringNodesResponse);
});

commands["gatheringharvest"] = ("Harvest node (nodeId)", async () =>
{
	Console.WriteLine("nodeId");
	var harvestNodeId = Console.ReadLine();
	var harvest = new Harvest();
	harvest.nodeId = harvestNodeId;
	jsonString = JsonSerializer.Serialize(harvest, options);
	var harvestResponse = await Post<HarvestResponse>(jsonString, GatheringHarvest, token);
	Console.WriteLine(harvestResponse);
});

commands["craftingskills"] = ("Crafting skill levels", async () =>
{
	var craftingSkillsResponse = await Get<CraftingSkillsResponse>(CraftingSkills, token);
	Console.WriteLine(craftingSkillsResponse);
});

commands["craftingrecipes"] = ("Crafting recipes (optional skill filter)", async () =>
{
	Console.WriteLine("skill filter (blacksmithing, alchemy, woodworking, blank for all)");
	var craftingSkillFilter = Console.ReadLine();
	var recipesUrl = string.IsNullOrWhiteSpace(craftingSkillFilter) ? CraftingRecipes : $"{CraftingRecipes}?skill={craftingSkillFilter}";
	var craftingRecipesResponse = await Get<CraftingRecipesResponse>(recipesUrl, token);
	Console.WriteLine(craftingRecipesResponse);
});

commands["craftingstations"] = ("Nearby crafting stations", async () =>
{
	var craftingStationsResponse = await Get<CraftingStationsResponse>(CraftingStations, token);
	Console.WriteLine(craftingStationsResponse);
});

commands["craftingcraft"] = ("Craft item (recipeId)", async () =>
{
	Console.WriteLine("recipeId");
	var recipeId = Console.ReadLine();
	var craft = new Craft();
	craft.recipeId = recipeId;
	jsonString = JsonSerializer.Serialize(craft, options);
	var craftResponse = await Post<CraftResponse>(jsonString, CraftingCraft, token);
	Console.WriteLine(craftResponse);
});

commands["gameraces"] = ("Game data: all races", async () =>
{
	var gameRacesResponse = await Get<GameRace[]>(GameRaces, token);
	Console.WriteLine(gameRacesResponse);
});

commands["gameclasses"] = ("Game data: all classes", async () =>
{
	var gameClassesResponse = await Get<GameClass[]>(GameClasses, token);
	Console.WriteLine(gameClassesResponse);
});

commands["gameitems"] = ("Game data: all items", async () =>
{
	var gameItemsResponse = await Get<GameItemDef[]>(GameItems, token);
	Console.WriteLine(gameItemsResponse);
});

commands["gamespells"] = ("Game data: all spells", async () =>
{
	var gameSpellsResponse = await Get<GameSpellDef[]>(GameSpells, token);
	Console.WriteLine(gameSpellsResponse);
});

commands["help"] = ("Show this command list", () =>
{
	foreach (var kv in commands.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
		Console.WriteLine($"  {kv.Key,-22} {kv.Value.Help}");
	Console.WriteLine("  q                      Quit");
	return Task.CompletedTask;
});

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
