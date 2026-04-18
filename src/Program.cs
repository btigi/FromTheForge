using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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


var token = string.Empty;

var action = Console.ReadLine();
while (action != "q")
{
	switch (action)
	{
		case "r":
			var register = new RegisterDto();
			register.Email = email;
			register.Password = password;

			jsonString = JsonSerializer.Serialize(register, options);

			var registerResponse = await Post<RegisterResponseDto>(jsonString, Register);
			Console.WriteLine(registerResponse);
			break;
		case "l":
			var login = new LoginDto();
			login.Email = email;
			login.Password = password;

			jsonString = JsonSerializer.Serialize(login, options);

			var loginResponse = await Post<LoginResponseDto>(jsonString, Login);
			token = loginResponse.AccessToken;
			Console.WriteLine(loginResponse);
			break;
		case "me":
			var meResponse = await Get<MeResponseDto>(Me, token);
			Console.WriteLine(meResponse);
			break;
		case "create":
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
			break;
		case "get":
			var getCharacterResponse = await Get<Character>(GetCharacter, token);
			Console.WriteLine(getCharacterResponse);
			break;
		case "allocate":
			Console.WriteLine("allocate a stat point (gained on level up)");
			break;
		case "travel":
			Console.WriteLine("Direction");
			Console.WriteLine("north, east, south, west");
			var direction = Console.ReadLine();
			var travel = new TravelDto();
			travel.Direction = direction;
			jsonString = JsonSerializer.Serialize(travel, options);
			var travelResponse = await Post<TravelResponseDto>(jsonString, Travel, token);
			break;
		case "go":
			Console.WriteLine("X coordinate");
			var x = Console.ReadLine();
			Console.WriteLine("Y coordinate");
			var y = Console.ReadLine();
			var go = new GoDto();
			go.x = Convert.ToInt32(x);
			go.y = Convert.ToInt32(y);
			jsonString = JsonSerializer.Serialize(go, options);
			var goResponse = await Post<GoResponseDto>(jsonString, Go, token);
			break;
		case "travelstatus":
			var travelStatusResponse = await Get<TravelStatusResponseDto>(TravelStatus, token);
			Console.WriteLine(travelStatusResponse);
			break;
		case "travelcancel":
			var travelCancelResponse = await Post<TravelCancelResponseDto>(string.Empty, TravelCancel, token);
			Console.WriteLine(travelCancelResponse);
			break;
		case "map":
		{
			static char MapTerrainChar(string t)
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

			var mapResponse = await Get<MapResponseDto>(Map, token);
			if (mapResponse?.terrain != null)
			{
				foreach (var row in mapResponse.terrain)
				{
					if (row != null)
					{
						foreach (var cell in row)
							Console.Write(MapTerrainChar(cell));
					}
					Console.WriteLine();
				}
			}
			else
				Console.WriteLine("(no terrain grid)");

			if (mapResponse != null)
				Console.WriteLine($"size {mapResponse.width}x{mapResponse.height}");
			break;
		}
		case "regions":
			var regionsResponse = await Get<Region[]>(Regions, token);
			Console.WriteLine(regionsResponse);
			break;
		case "mapdetail":
			Console.WriteLine("X coordinate");
			var xMapDetail = Console.ReadLine();
			Console.WriteLine("Y coordinate");
			var yMapDetail = Console.ReadLine();
			var mapDetailResponse = await Get<MapDetailResponseDto>($"{MapDetail}/{xMapDetail}/{yMapDetail}", token);
			Console.WriteLine(mapDetailResponse);
			break;
		case "combatstatus":
			var combatStatusResponse = await Get<CombatStatus>(CombatStatus, token);
			Console.WriteLine(combatStatusResponse);
			break;
		case "combataction":
			Console.WriteLine("action");
			Console.WriteLine("  attack, cast, use_item, flee");
			var combatAction = Console.ReadLine();
			jsonString = JsonSerializer.Serialize(combatAction, options);
			var combatActionResponse = await Post<CombatActionResult>(jsonString, CombatAction, token);
			Console.WriteLine(combatActionResponse);
			break;
		case "inventory":
			var inventoryResponse = await Get<Inventory>(Inventory, token);
			Console.WriteLine(inventoryResponse);
			break;
		case "pickup":
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
			break;
		case "drop":
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
			break;
		case "equip":
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
			break;
		case "unequip":
			Console.WriteLine("slot");
			Console.WriteLine("weapon, armor, helmet, shield, leggings, boots, gloves, ring1, ring2, amulet");
			var unequipslot = Console.ReadLine();
			var unequip = new Unequip();
			unequip.slot = unequipslot;
			jsonString = JsonSerializer.Serialize(unequip, options);
			var unequipResponse = await Post<UnequipResponse>(jsonString, Unequip, token);
			Console.WriteLine(unequipResponse);
			break;
		case "use":
			Console.WriteLine("itemid");
			var useitem = Console.ReadLine();
			var use = new Use();
			use.itemId = useitem;
			jsonString = JsonSerializer.Serialize(use, options);
			var useResponse = await Post<UseResponse>(jsonString, Use, token);
			Console.WriteLine(useResponse);
			break;
		case "spellbook":
			var spellbookResponse = await Get<SpellbookResponse>(Spells, token);
			Console.WriteLine(spellbookResponse);
			break;
		case "spelllearn":
			Console.WriteLine("spellId");
			var learnSpellId = Console.ReadLine();
			var learnSpell = new LearnSpell();
			learnSpell.spellId = learnSpellId;
			jsonString = JsonSerializer.Serialize(learnSpell, options);
			var learnSpellResponse = await Post<LearnSpellResponse>(jsonString, SpellsLearn, token);
			Console.WriteLine(learnSpellResponse);
			break;
		case "spellcast":
			Console.WriteLine("spellId");
			var castSpellId = Console.ReadLine();
			var castSpell = new CastSpell();
			castSpell.spellId = castSpellId;
			jsonString = JsonSerializer.Serialize(castSpell, options);
			var castSpellResponse = await Post<CastSpellResponse>(jsonString, SpellsCast, token);
			Console.WriteLine(castSpellResponse);
			break;
		case "restcamp":
			Console.WriteLine("duration seconds (10-600)");
			var campDuration = Console.ReadLine();
			var startCamp = new StartCampRequest();
			startCamp.duration = Convert.ToInt32(campDuration);
			jsonString = JsonSerializer.Serialize(startCamp, options);
			var startCampResponse = await Post<StartCampResponse>(jsonString, RestCamp, token);
			Console.WriteLine(startCampResponse);
			break;
		case "restinn":
			var innRestResponse = await Post<InnRestResponse>(string.Empty, RestInn, token);
			Console.WriteLine(innRestResponse);
			break;
		case "reststatus":
			var restStatusResponse = await Get<RestStatus>(RestStatusPath, token);
			Console.WriteLine(restStatusResponse);
			break;
		case "reststop":
			var stopRestResponse = await Post<StopRestResponse>(string.Empty, RestStop, token);
			Console.WriteLine(stopRestResponse);
			break;
		case "shop":
			Console.WriteLine("townId");
			var shopTownId = Console.ReadLine();
			var shopResponse = await Get<ShopResponse>($"{Shops}/{shopTownId}", token);
			Console.WriteLine(shopResponse);
			break;
		case "shopbuy":
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
			break;
		case "shopsell":
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
			break;
		case "questavailable":
			var questAvailableResponse = await Get<AvailableQuestsResponse>(QuestAvailable, token);
			Console.WriteLine(questAvailableResponse);
			break;
		case "questactive":
			var questActiveResponse = await Get<ActiveQuestsResponse>(QuestActive, token);
			Console.WriteLine(questActiveResponse);
			break;
		case "questaccept":
			Console.WriteLine("questId");
			var acceptQuestId = Console.ReadLine();
			var acceptQuest = new AcceptQuest();
			acceptQuest.questId = acceptQuestId;
			jsonString = JsonSerializer.Serialize(acceptQuest, options);
			var acceptQuestResponse = await Post<AcceptQuestResponse>(jsonString, QuestAccept, token);
			Console.WriteLine(acceptQuestResponse);
			break;
		case "questturnin":
			Console.WriteLine("questId");
			var turnInQuestId = Console.ReadLine();
			var turnInQuest = new TurnInQuest();
			turnInQuest.questId = turnInQuestId;
			jsonString = JsonSerializer.Serialize(turnInQuest, options);
			var turnInQuestResponse = await Post<TurnInQuestResponse>(jsonString, QuestTurnIn, token);
			Console.WriteLine(turnInQuestResponse);
			break;
		case "questabandon":
			Console.WriteLine("questId");
			var abandonQuestId = Console.ReadLine();
			var abandonQuest = new AbandonQuest();
			abandonQuest.questId = abandonQuestId;
			jsonString = JsonSerializer.Serialize(abandonQuest, options);
			var abandonQuestResponse = await Post<AbandonQuestResponse>(jsonString, QuestAbandon, token);
			Console.WriteLine(abandonQuestResponse);
			break;
		case "dungeonstatus":
			var dungeonStatusResponse = await Get<DungeonStatus>(DungeonStatusPath, token);
			Console.WriteLine(dungeonStatusResponse);
			break;
		case "dungeonenter":
			Console.WriteLine("poiId");
			var dungeonPoiId = Console.ReadLine();
			var enterDungeon = new EnterDungeon();
			enterDungeon.poiId = dungeonPoiId;
			jsonString = JsonSerializer.Serialize(enterDungeon, options);
			var enterDungeonResponse = await Post<DungeonStatus>(jsonString, DungeonEnter, token);
			Console.WriteLine(enterDungeonResponse);
			break;
		case "dungeonadvance":
			var dungeonAdvanceResponse = await Post<DungeonAdvanceResponse>(string.Empty, DungeonAdvance, token);
			Console.WriteLine(dungeonAdvanceResponse);
			break;
		case "dungeonleave":
			var dungeonLeaveResponse = await Post<DungeonLeaveResponse>(string.Empty, DungeonLeave, token);
			Console.WriteLine(dungeonLeaveResponse);
			break;
		case "gatheringskills":
			var gatheringSkillsResponse = await Get<GatheringSkillsResponse>(GatheringSkills, token);
			Console.WriteLine(gatheringSkillsResponse);
			break;
		case "gatheringnodes":
			var gatheringNodesResponse = await Get<GatheringNodesResponse>(GatheringNodes, token);
			Console.WriteLine(gatheringNodesResponse);
			break;
		case "gatheringharvest":
			Console.WriteLine("nodeId");
			var harvestNodeId = Console.ReadLine();
			var harvest = new Harvest();
			harvest.nodeId = harvestNodeId;
			jsonString = JsonSerializer.Serialize(harvest, options);
			var harvestResponse = await Post<HarvestResponse>(jsonString, GatheringHarvest, token);
			Console.WriteLine(harvestResponse);
			break;
		case "craftingskills":
			var craftingSkillsResponse = await Get<CraftingSkillsResponse>(CraftingSkills, token);
			Console.WriteLine(craftingSkillsResponse);
			break;
		case "craftingrecipes":
			Console.WriteLine("skill filter (blacksmithing, alchemy, woodworking, blank for all)");
			var craftingSkillFilter = Console.ReadLine();
			var recipesUrl = string.IsNullOrWhiteSpace(craftingSkillFilter) ? CraftingRecipes : $"{CraftingRecipes}?skill={craftingSkillFilter}";
			var craftingRecipesResponse = await Get<CraftingRecipesResponse>(recipesUrl, token);
			Console.WriteLine(craftingRecipesResponse);
			break;
		case "craftingstations":
			var craftingStationsResponse = await Get<CraftingStationsResponse>(CraftingStations, token);
			Console.WriteLine(craftingStationsResponse);
			break;
		case "craftingcraft":
			Console.WriteLine("recipeId");
			var recipeId = Console.ReadLine();
			var craft = new Craft();
			craft.recipeId = recipeId;
			jsonString = JsonSerializer.Serialize(craft, options);
			var craftResponse = await Post<CraftResponse>(jsonString, CraftingCraft, token);
			Console.WriteLine(craftResponse);
			break;
		case "gameraces":
			var gameRacesResponse = await Get<GameRace[]>(GameRaces, token);
			Console.WriteLine(gameRacesResponse);
			break;
		case "gameclasses":
			var gameClassesResponse = await Get<GameClass[]>(GameClasses, token);
			Console.WriteLine(gameClassesResponse);
			break;
		case "gameitems":
			var gameItemsResponse = await Get<GameItemDef[]>(GameItems, token);
			Console.WriteLine(gameItemsResponse);
			break;
		case "gamespells":
			var gameSpellsResponse = await Get<GameSpellDef[]>(GameSpells, token);
			Console.WriteLine(gameSpellsResponse);
			break;
	}
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



public class CamelCaseNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name)
	{
		if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
			return name;

		return char.ToLowerInvariant(name[0]) + name[1..];
	}
}


public class RegisterDto
{
	public string Email { get; set; }
	public string Password { get; set; }
}

public class RegisterResponseDto
{
	public string Id { get; set; }
	public string Email { get; set; }
	public string Message { get; set; }
}

public class LoginDto
{
	public string Email { get; set; }
	public string Password { get; set; }
}

public class LoginResponseDto
{
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }
	public int ExpiresIn { get; set; }
	public User User { get; set; }

	public string Error { get; set; }
	public string Message { get; set; }
	public int StatusCode { get; set; }
}

public class User
{
	public string Id { get; set; }
	public string Email { get; set; }
}

public class MeResponseDto
{
	public string Id { get; set; }
	public string Email { get; set; }
	public DateTime CreatedAt { get; set; }
}

public class CreateCharacterDto
{
	public string Name { get; set; }
	public string RaceId { get; set; }
	public string ClassId { get; set; }
	public int Strength { get; set; }
	public int Dexterity { get; set; }
	public int Constitution { get; set; }
	public int Intelligence { get; set; }
	public int Wisdom { get; set; }
	public int Charisma { get; set; }
}


public class CreateCharacterResponseDto
{
	public string Error { get; set; }
	public string Message { get; set; }

	public string id { get; set; }
	public string user_id { get; set; }
	public string name { get; set; }
	public string race_id { get; set; }
	public string class_id { get; set; }
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

public class Race
{
	public string id { get; set; }
	public string name { get; set; }
}

public class @Class
{
	public string id { get; set; }
	public string name { get; set; }
}



public class Character
{
	public string id { get; set; }
	public string name { get; set; }
	public Race race { get; set; }
	public @Class @class { get; set; }
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
	public Position position { get; set; }
	public bool inCombat { get; set; }
	public bool inDungeon { get; set; }
	public DateTime createdAt { get; set; }
	public Effectivestats effectiveStats { get; set; }
	public Equipment equipment { get; set; }
	public Spell[] spells { get; set; }
	public int carryCapacity { get; set; }
	public Travel travel { get; set; }
	public Discovery[] discoveries { get; set; }
	public Xp_Extra xp_extra { get; set; }
	public Encounter encounter { get; set; }
}

public class Position
{
	public int x { get; set; }
	public int y { get; set; }
}

public class Effectivestats
{
	public int strength { get; set; }
	public int dexterity { get; set; }
	public int constitution { get; set; }
	public int intelligence { get; set; }
	public int wisdom { get; set; }
	public int charisma { get; set; }
	public int maxHp { get; set; }
	public int ac { get; set; }
}

public class Equipment
{
	public Weapon weapon { get; set; }
	public Armor armor { get; set; }
	public Helmet helmet { get; set; }
	public Shield shield { get; set; }
	public Leggings leggings { get; set; }
	public Boots boots { get; set; }
	public Gloves gloves { get; set; }
	public Ring ring1 { get; set; }
	public Ring ring2 { get; set; }
	public Amulet amulet { get; set; }
}

public class Weapon
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Armor
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Helmet
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Shield
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Leggings
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Boots
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Gloves
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Ring
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Amulet
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
}

public class Travel
{
	public bool isTraveling { get; set; }
	public Position1 position { get; set; }
	public Estimatedcurrent estimatedCurrent { get; set; }
	public Destination destination { get; set; }
	public Path[] path { get; set; }
	public int progressIndex { get; set; }
	public int totalCells { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}

public class Position1
{
	public int x { get; set; }
	public int y { get; set; }
}

public class Estimatedcurrent
{
	public int x { get; set; }
	public int y { get; set; }
}

public class Destination
{
	public int x { get; set; }
	public int y { get; set; }
}

public class Path
{
	public int x { get; set; }
	public int y { get; set; }
}

public class Xp_Extra
{
	public int xpGained { get; set; }
	public int totalXp { get; set; }
	public Levelup levelUp { get; set; }
}

public class Levelup
{
	public int newLevel { get; set; }
	public int hpGained { get; set; }
	public int manaGained { get; set; }
	public Newspell[] newSpells { get; set; }
	public int statPointsGained { get; set; }
}

public class Newspell
{
	public string id { get; set; }
	public string name { get; set; }
	public int manaCost { get; set; }
}

public class Encounter
{
	public string monsterId { get; set; }
	public string monsterName { get; set; }
	public int monsterLevel { get; set; }
	public int monsterHp { get; set; }
	public int monsterMaxHp { get; set; }
	public string monsterType { get; set; }
	public string source { get; set; }
}

public class Spell
{
	public string id { get; set; }
	public string name { get; set; }
	public string school { get; set; }
	public int spellLevel { get; set; }
	public int manaCost { get; set; }
}

public class Discovery
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string category { get; set; }
	public int x { get; set; }
	public int y { get; set; }
	public string terrain { get; set; }
	public string description { get; set; }
	public int? level_min { get; set; }
	public int? level_max { get; set; }
	public DateTime discoveredAt { get; set; }
}

public class TravelResponseDto
{
	public string direction { get; set; }
	public From from { get; set; }
	public Destination destination { get; set; }
	public string terrain { get; set; }
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}

public class From
{
	public int x { get; set; }
	public int y { get; set; }
}

public class TravelDto
{
	public string Direction { get; set; }
}

public class GoDto
{
	public int x { get; set; }
	public int y { get; set; }
}


public class GoResponseDto
{
	public From from { get; set; }
	public Destination destination { get; set; }
	public Path[] path { get; set; }
	public int totalCells { get; set; }
	public int travelSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
}

public class TravelStatusResponseDto
{
	public bool isTraveling { get; set; }
	public Position position { get; set; }
	public Estimatedcurrent estimatedCurrent { get; set; }
	public Destination destination { get; set; }
	public Path[] path { get; set; }
	public int progressIndex { get; set; }
	public int totalCells { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime eta { get; set; }
	public Discovery[] discoveries { get; set; }
	public Xp xp { get; set; }
	public Encounter encounter { get; set; }
}

public class Xp
{
	public int xpGained { get; set; }
	public int totalXp { get; set; }
	public Levelup levelUp { get; set; }
}

public class TravelCancelResponseDto
{
	public bool cancelled { get; set; }
	public Position position { get; set; }
	public int cellsTraversed { get; set; }
	public Discovery[] discoveries { get; set; }
	public Xp xp { get; set; }
}


public class MapResponseDto
{
	public int width { get; set; }
	public int height { get; set; }
	public string[][] terrain { get; set; }
	public Pois[] pois { get; set; }
	public Discovery[] discoveries { get; set; }
}

public class Pois
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string category { get; set; }
	public int x { get; set; }
	public int y { get; set; }
	public string terrain { get; set; }
	public string description { get; set; }
	public int? level_min { get; set; }
	public int? level_max { get; set; }
	public DateTime discoveredAt { get; set; }
}

public class Region
{
	public string id { get; set; }
	public string name { get; set; }
	public string[] terrains { get; set; }
	public int centerX { get; set; }
	public int centerY { get; set; }
	public int radius { get; set; }
	public string color { get; set; }
	public string description { get; set; }
}

public class MapDetailResponseDto
{
	public int x { get; set; }
	public int y { get; set; }
	public string terrain { get; set; }
	public Poi poi { get; set; }
}

public class Poi
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string category { get; set; }
	public int x { get; set; }
	public int y { get; set; }
	public string terrain { get; set; }
	public string description { get; set; }
	public int? level_min { get; set; }
	public int? level_max { get; set; }
	public DateTime discoveredAt { get; set; }
}

public class CombatStatus
{
	public bool inCombat { get; set; }
	public int turn { get; set; }
	public string source { get; set; }
	public Monster monster { get; set; }
	public Player player { get; set; }
	public string[] log { get; set; }
}

public class Monster
{
	public string id { get; set; }
	public string name { get; set; }
	public int level { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int ac { get; set; }
	public string type { get; set; }
	public Effect[] effects { get; set; }
}

public class Effect
{
	public string type { get; set; }
	public int duration { get; set; }
	public int value { get; set; }
	public string source { get; set; }
}

public class Player
{
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public int ac { get; set; }
	public Effect[] effects { get; set; }
}

public class CombatAction
{
	public string action { get; set; }
	public string spellId { get; set; }
	public string itemId { get; set; }
}

public class CombatActionResult
{
	public string outcome { get; set; }
	public int turn { get; set; }
	public string[] log { get; set; }
	public Monster monster { get; set; }
	public Player player { get; set; }
	public int xp { get; set; }
	public int gold { get; set; }
	public Item[] items { get; set; }
	public Levelup levelUp { get; set; }
	public int goldLost { get; set; }
	public int xpLost { get; set; }
	public int hp { get; set; }
	public string message { get; set; }
}

public class Item
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class Inventory
{
	public Equipment equipment { get; set; }
	public Backpack[] backpack { get; set; }
	public float carryWeight { get; set; }
	public float carryCapacity { get; set; }
}

public class Backpack
{
	public string itemId { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
	public int quantity { get; set; }
	public float weight { get; set; }
	public int levelRequired { get; set; }
	public string classRestriction { get; set; }
}

public class Pickup
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class PickupResponse
{
	public string pickedUp { get; set; }
	public int quantity { get; set; }
	public string message { get; set; }
}

public class Drop
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class DropResponse
{
	public string dropped { get; set; }
	public int quantity { get; set; }
	public string message { get; set; }
}

public class Equip
{
	public string itemId { get; set; }
	public string slot { get; set; }
}

public class EquipResponse
{
	public Equipped equipped { get; set; }
	public Unequipped unequipped { get; set; }
	public string message { get; set; }
}

public class Equipped
{
	public string slot { get; set; }
	public Item item { get; set; }
}

public class Unequipped
{
	public string slot { get; set; }
	public Item item { get; set; }
}

public class Unequip
{
	public string slot { get; set; }
}

public class UnequipResponse
{
	public Unequipped unequipped { get; set; }
	public string message { get; set; }
}

public class Use
{
	public string itemId { get; set; }
}

public class UseResponse
{
	public string used { get; set; }
	public string message { get; set; }
	public Effect effect { get; set; }
	public string result { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public string learned { get; set; }
}

public class SpellbookResponse
{
	public int mana { get; set; }
	public int maxMana { get; set; }
	public KnownSpell[] spells { get; set; }
}

public class KnownSpell
{
	public string id { get; set; }
	public string name { get; set; }
	public string school { get; set; }
	public int spell_level { get; set; }
	public int mana_cost { get; set; }
}

public class LearnSpell
{
	public string spellId { get; set; }
}

public class LearnSpellResponse
{
	public string learned { get; set; }
	public string message { get; set; }
	public KnownSpell spell { get; set; }
}

public class CastSpell
{
	public string spellId { get; set; }
}

public class CastSpellResponse
{
	public string cast { get; set; }
	public int manaCost { get; set; }
	public Effect effect { get; set; }
	public string result { get; set; }
}

public class StartCampRequest
{
	public int? duration { get; set; }
}

public class StartCampResponse
{
	public bool resting { get; set; }
	public string type { get; set; }
	public int durationSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime until { get; set; }
	public Position position { get; set; }
	public RestRecoveryAmount willRecover { get; set; }
}

public class InnRestResponse
{
	public bool rested { get; set; }
	public string type { get; set; }
	public string location { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public int hpRestored { get; set; }
	public int manaRestored { get; set; }
}

public class RestStatus
{
	public bool resting { get; set; }
	public string type { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime until { get; set; }
	public double elapsedSeconds { get; set; }
	public int totalDurationSeconds { get; set; }
	public int currentHp { get; set; }
	public int currentMana { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public RestRecoveryAmount recovered { get; set; }
}

public class StopRestResponse
{
	public bool stopped { get; set; }
	public bool completed { get; set; }
	public double elapsedSeconds { get; set; }
	public RestRecoveryAmount recovered { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
}

public class RestRecoveryAmount
{
	public int hp { get; set; }
	public int mana { get; set; }
}

public class ShopResponse
{
	public string townId { get; set; }
	public string shop { get; set; }
	public string description { get; set; }
	public int restockMinutes { get; set; }
	public int gold { get; set; }
	public ShopStockItem[] items { get; set; }
}

public class ShopStockItem
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
	public int price { get; set; }
	public int sellPrice { get; set; }
	public int levelRequired { get; set; }
	public string classRestriction { get; set; }
	public string weight { get; set; }
	public int stock { get; set; }
	public int maxStock { get; set; }
}

public class BuyItem
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class BuyItemResponse
{
	public string bought { get; set; }
	public int quantity { get; set; }
	public int totalCost { get; set; }
	public int gold { get; set; }
	public string message { get; set; }
}

public class SellItem
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class SellItemResponse
{
	public string sold { get; set; }
	public int quantity { get; set; }
	public int unitPrice { get; set; }
	public int totalPrice { get; set; }
	public int gold { get; set; }
	public string message { get; set; }
}

public class QuestObjective
{
	public string type { get; set; }
	public string target { get; set; }
	public int quantity { get; set; }
	public string description { get; set; }
	public int current { get; set; }
	public int @required { get; set; }
}

public class QuestRewards
{
	public int xp { get; set; }
	public int gold { get; set; }
	public QuestRewardItem[] items { get; set; }
}

public class QuestRewardItem
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class AvailableQuestsResponse
{
	public string town { get; set; }
	public AvailableQuest[] quests { get; set; }
}

public class AvailableQuest
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public int levelMin { get; set; }
	public int levelMax { get; set; }
	public QuestObjective[] objectives { get; set; }
	public QuestRewards rewards { get; set; }
}

public class ActiveQuestsResponse
{
	public int activeCount { get; set; }
	public int maxActive { get; set; }
	public ActiveQuest[] quests { get; set; }
}

public class ActiveQuest
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string giverTown { get; set; }
	public string status { get; set; }
	public QuestObjective[] objectives { get; set; }
	public QuestRewards rewards { get; set; }
	public DateTime acceptedAt { get; set; }
	public DateTime completedAt { get; set; }
}

public class AcceptQuest
{
	public string questId { get; set; }
}

public class AcceptQuestResponse
{
	public string message { get; set; }
	public AcceptedQuest quest { get; set; }
}

public class AcceptedQuest
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string status { get; set; }
	public QuestObjective[] objectives { get; set; }
	public QuestRewards rewards { get; set; }
}

public class TurnInQuest
{
	public string questId { get; set; }
}

public class TurnInQuestResponse
{
	public string message { get; set; }
	public TurnInQuestRewards rewards { get; set; }
}

public class TurnInQuestRewards
{
	public int xp { get; set; }
	public int gold { get; set; }
	public TurnInQuestRewardItem[] items { get; set; }
	public bool leveledUp { get; set; }
	public int newLevel { get; set; }
	public Newspell[] newSpells { get; set; }
}

public class TurnInQuestRewardItem
{
	public string itemId { get; set; }
	public int quantity { get; set; }
	public string name { get; set; }
}

public class AbandonQuest
{
	public string questId { get; set; }
}

public class AbandonQuestResponse
{
	public string message { get; set; }
}

public class EnterDungeon
{
	public string poiId { get; set; }
}

public class DungeonRoom
{
	public int index { get; set; }
	public string type { get; set; }
	public bool cleared { get; set; }
	public string[] log { get; set; }
}

public class DungeonStatus
{
	public bool inDungeon { get; set; }
	public string dungeonId { get; set; }
	public string poiId { get; set; }
	public string poiName { get; set; }
	public int dungeonLevel { get; set; }
	public int currentRoom { get; set; }
	public int totalRooms { get; set; }
	public bool completed { get; set; }
	public DungeonRoom[] rooms { get; set; }
}

public class DungeonAdvanceMonster
{
	public string name { get; set; }
	public int level { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public string type { get; set; }
}

public class DungeonAdvanceResponse
{
	public int roomIndex { get; set; }
	public string roomType { get; set; }
	public string status { get; set; }
	public string message { get; set; }
	public DungeonAdvanceMonster monster { get; set; }
	public string trapType { get; set; }
	public bool avoided { get; set; }
	public int gold { get; set; }
	public QuestRewardItem[] loot { get; set; }
	public int hp { get; set; }
	public int maxHp { get; set; }
	public int mana { get; set; }
	public int maxMana { get; set; }
	public RestRecoveryAmount recovered { get; set; }
	public string[] log { get; set; }
}

public class DungeonLeaveResponse
{
	public bool left { get; set; }
	public string message { get; set; }
	public Position position { get; set; }
}

public class GatheringSkillsResponse
{
	public GatheringSkillsDetail skills { get; set; }
}

public class GatheringSkillsDetail
{
	public GatheringSkillEntry mining { get; set; }
	public GatheringSkillEntry herbalism { get; set; }
	public GatheringSkillEntry woodcutting { get; set; }
}

public class GatheringSkillEntry
{
	public int level { get; set; }
	public int xp { get; set; }
	public int? xpToNext { get; set; }
}

public class GatheringNodesResponse
{
	public Position position { get; set; }
	public GatheringNode[] nodes { get; set; }
}

public class GatheringNode
{
	public string poiId { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string nodeId { get; set; }
	public string skill { get; set; }
	public int minLevel { get; set; }
	public int xpReward { get; set; }
	public int cooldownMinutes { get; set; }
	public bool ready { get; set; }
	public DateTime? availableAt { get; set; }
}

public class Harvest
{
	public string nodeId { get; set; }
}

public class HarvestResponse
{
	public string harvested { get; set; }
	public string itemId { get; set; }
	public int quantity { get; set; }
	public int xpGained { get; set; }
	public string skill { get; set; }
	public int skillLevel { get; set; }
	public int skillXp { get; set; }
	public int cooldownMinutes { get; set; }
	public DateTime availableAt { get; set; }
	public HarvestLevelUp levelUp { get; set; }
}

public class HarvestLevelUp
{
	public int newLevel { get; set; }
	public string skill { get; set; }
}

public class CraftingSkillsResponse
{
	public CraftingSkillsDetail skills { get; set; }
}

public class CraftingSkillsDetail
{
	public CraftingSkillEntry blacksmithing { get; set; }
	public CraftingSkillEntry alchemy { get; set; }
	public CraftingSkillEntry woodworking { get; set; }
}

public class CraftingSkillEntry
{
	public int level { get; set; }
	public int xp { get; set; }
	public int? xpToNext { get; set; }
}

public class CraftingRecipesResponse
{
	public CraftingRecipe[] recipes { get; set; }
}

public class CraftingRecipe
{
	public string id { get; set; }
	public string name { get; set; }
	public string skill { get; set; }
	public int minLevel { get; set; }
	public int xpReward { get; set; }
	public CraftingIngredient[] ingredients { get; set; }
	public CraftingOutput output { get; set; }
	public bool meetsLevel { get; set; }
	public bool hasIngredients { get; set; }
	public bool canCraft { get; set; }
}

public class CraftingIngredient
{
	public string itemId { get; set; }
	public int quantity { get; set; }
	public int have { get; set; }
}

public class CraftingOutput
{
	public string itemId { get; set; }
	public int quantity { get; set; }
}

public class CraftingStationsResponse
{
	public Position position { get; set; }
	public CraftingStation[] stations { get; set; }
}

public class CraftingStation
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string skill { get; set; }
	public string description { get; set; }
}

public class Craft
{
	public string recipeId { get; set; }
}

public class CraftResponse
{
	public string crafted { get; set; }
	public string itemId { get; set; }
	public int quantity { get; set; }
	public int xpGained { get; set; }
	public string skill { get; set; }
	public int skillLevel { get; set; }
	public int skillXp { get; set; }
	public HarvestLevelUp levelUp { get; set; }
}

public class GameRace
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public JsonElement? bonuses { get; set; }
}

public class GameClass
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public int hitDie { get; set; }
	public string primaryStat { get; set; }
}

public class GameItemDef
{
	public string id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public string rarity { get; set; }
	public int? damage_min { get; set; }
	public int? damage_max { get; set; }
	public int armor { get; set; }
	public int value { get; set; }
	public double weight { get; set; }
	public int level_req { get; set; }
	public string class_req { get; set; }
	public JsonElement? statusEffect { get; set; }
	public double? statusChance { get; set; }
}

public class GameSpellDef
{
	public string id { get; set; }
	public string name { get; set; }
	[JsonPropertyName("class")]
	public string spellClass { get; set; }
	public int level_req { get; set; }
	public int mana_cost { get; set; }
	public string targetType { get; set; }
	public string effectType { get; set; }
	public double effectValue { get; set; }
	public string damageType { get; set; }
	public int cooldown { get; set; }
	public string description { get; set; }
	public JsonElement? statusEffect { get; set; }
}