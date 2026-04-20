using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

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


var buildDate = GetBuildDate(Assembly.GetExecutingAssembly());
var buildDateString = buildDate?.ToString("yyyy/MM/dd@HH:mm") ?? "unknown";
Console.WriteLine($"Build {buildDateString}");

var token = string.Empty;

var commands = new Dictionary<string, (string Help, Func<Task> Run)>(StringComparer.OrdinalIgnoreCase);

commands["register"] = ("Register account", async () =>
{
	var registerEmail = Environment.GetEnvironmentVariable("FromTheForge_Username")?.Trim();
	if (string.IsNullOrWhiteSpace(registerEmail))
	{
		Console.WriteLine("Email");
		registerEmail = Console.ReadLine();
	}
	var registerPassword = Environment.GetEnvironmentVariable("FromTheForge_Password")?.Trim();
	if (string.IsNullOrWhiteSpace(registerPassword))
	{
		Console.WriteLine("Password");
		registerPassword = Console.ReadLine();
	}

	var register = new RegisterDto();
	register.Email = registerEmail;
	register.Password = registerPassword;
	jsonString = JsonSerializer.Serialize(register, options);
	var registerResponse = await Post<RegisterResponseDto>(jsonString, Constants.Register);
	Console.WriteLine(registerResponse);
}
);

commands["login"] = ("Login", async () =>
{
	var loginEmail = Environment.GetEnvironmentVariable("FromTheForge_Username")?.Trim();
	if (string.IsNullOrWhiteSpace(loginEmail))
	{
		Console.WriteLine("Email");
		loginEmail = Console.ReadLine();
	}
	var loginPassword = Environment.GetEnvironmentVariable("FromTheForge_Password")?.Trim();
	if (string.IsNullOrWhiteSpace(loginPassword))
	{
		Console.WriteLine("Password");
		loginPassword = Console.ReadLine();
	}
	var login = new LoginDto();
	login.Email = loginEmail;
	login.Password = loginPassword;
	jsonString = JsonSerializer.Serialize(login, options);
	var loginResponse = await Post<LoginResponseDto>(jsonString, Constants.Login);
	if (!string.IsNullOrEmpty(loginResponse.Error))
	{
		Console.WriteLine(loginResponse.Error);
		return;
	}
	token = loginResponse.AccessToken;
	Console.WriteLine($"Logged in as {new string('*', loginResponse.User.Email.Length)}");
}
);

commands["me"] = ("Get authenticated profile", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var meResponse = await Get<MeResponseDto>(Constants.Me, token);
	Console.WriteLine($"Id: {meResponse.Id}");
	Console.WriteLine($"Email: {meResponse.Email}");
}
);

commands["create"] = ("Create character", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("Name");
	var name = Console.ReadLine();
	Console.WriteLine("Race");
	var race = ReadValidatedInput(["human", "elf", "dwarf", "halfling", "orc"]);
	Console.WriteLine("Class");
	var @class = ReadValidatedInput(["warrior", "mage", "rogue", "cleric", "ranger"]);
	Console.WriteLine("STR");
	var str = ReadValidatedIntInput(8, 15);
	Console.WriteLine("DEX");
	var dex = ReadValidatedIntInput(8, 15);
	Console.WriteLine("CON");
	var con = ReadValidatedIntInput(8, 15);
	Console.WriteLine("INT");
	var intl = ReadValidatedIntInput(8, 15);
	Console.WriteLine("WIS");
	var wis = ReadValidatedIntInput(8, 15);
	Console.WriteLine("CHA");
	var cha = ReadValidatedIntInput(8, 15);
	//TODO: point buy validation?
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
	if (String.IsNullOrEmpty(createCharacterResponse.Error))
	{
		Console.WriteLine($"Created character");
		Console.WriteLine($"  {createCharacterResponse.name} {createCharacterResponse.race_id} {createCharacterResponse.class_id} [{createCharacterResponse.pos_x},{createCharacterResponse.pos_y}]");
		Console.WriteLine($"  {createCharacterResponse.hp}/{createCharacterResponse.max_hp}HP {createCharacterResponse.mana}/{createCharacterResponse.max_mana}mana");
		Console.WriteLine($"  {createCharacterResponse.ac} {createCharacterResponse.gold}");
		Console.WriteLine($"  STR {createCharacterResponse.strength} DEX {createCharacterResponse.dexterity} CON {createCharacterResponse.constitution} INT {createCharacterResponse.intelligence} WIS {createCharacterResponse.wisdom} CHA {createCharacterResponse.charisma}");
	}
	else
	{
		Console.WriteLine($"Error {createCharacterResponse.Error}");
		foreach (var message in createCharacterResponse.Message)
		{
			Console.WriteLine(message);
		}
		return;
	}
}
);

commands["get"] = ("Get my character", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var character = await Get<Character>(Constants.GetCharacter, token);
	if (character == null)
	{
		Console.WriteLine("(No character / request failed)");
		return;
	}

	Console.WriteLine($"{character.name} {character.race.name}, {character.@class.name}");
	Console.WriteLine($"Level: {character.level} XP: {character.xp}/{character.xpToNextLevel}");
	Console.WriteLine($"{character.hp}/{character.maxHp}HP, {character.mana}/{character.maxMana}mana, {character.ac}AC, {character.gold}gold");
	Console.WriteLine($"Base:  STR {character.strength} DEX {character.dexterity} CON {character.constitution} INT {character.intelligence} WIS {character.wisdom} CHA {character.charisma}");
	Console.WriteLine($"Effective:  STR {character.effectiveStats.strength} DEX {character.effectiveStats.dexterity} CON {character.effectiveStats.constitution} INT {character.effectiveStats.intelligence} WIS {character.effectiveStats.wisdom} CHA {character.effectiveStats.charisma}");
	Console.WriteLine($"In Dungeon: {character.inDungeon}");
	Console.WriteLine($"In Combat: {character.inCombat}");
	var travelDestination = String.Empty;
	if (character.travel.isTraveling)
	{
		travelDestination = $"[{character.travel.destination.x},{character.travel.destination.y}] ETA: {character.travel.eta}";
	}
	Console.WriteLine($"Location: [{character.position.x},{character.position.y}]");
	Console.WriteLine($"Travelling: {character.travel.isTraveling} {travelDestination}");
	if (character.unspentStatPoints > 0)
	{
		Console.WriteLine($"** {character.unspentStatPoints} unspent stat points **");
	}
}
);

commands["allocate"] = ("Placeholder: allocate stat point on level up", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("Stat");
	var stat = ReadValidatedInput(["strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma"]);
	var allocate = new Allocate();
	allocate.stat = stat;
	jsonString = JsonSerializer.Serialize(allocate, options);
	var allocateResponse = await Post<AllocateResponse>(jsonString, Constants.Allocate, token);
	Console.WriteLine(allocateResponse.message);
}
);

commands["move"] = ("Move one step (north/east/south/west)", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("Direction");
	var direction = ReadValidatedInput(["north", "east", "south", "west"]);
	var travel = new TravelDto();
	travel.Direction = direction;
	jsonString = JsonSerializer.Serialize(travel, options);
	var travelResponse = await Post<TravelResponseDto>(jsonString, Constants.Travel, token);
	if (String.IsNullOrEmpty(travelResponse.error))
	{
		Console.WriteLine($"Terrain {travelResponse.terrain}");
		Console.WriteLine($"Moving from [{travelResponse.from.x},{travelResponse.from.y}] to [{travelResponse.destination.x},{travelResponse.destination.y}]");
		Console.WriteLine($"Started at {travelResponse.startedAt}, eta {travelResponse.eta} {travelResponse.travelSeconds} seconds");
	}
	else
	{
		Console.WriteLine($"Error {travelResponse.error}");
		Console.WriteLine($"{travelResponse.message}");
	}
}
);

commands["go"] = ("Pathfind to coordinates", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("X coordinate");
	var x = ReadValidatedIntInput(0, 100);
	Console.WriteLine("Y coordinate");
	var y = ReadValidatedIntInput(0, 100);
	var go = new GoDto();
	go.x = Convert.ToInt32(x);
	go.y = Convert.ToInt32(y);
	jsonString = JsonSerializer.Serialize(go, options);
	var goResponse = await Post<GoResponseDto>(jsonString, Constants.Go, token);
	if (String.IsNullOrEmpty(goResponse.error))
	{
		Console.WriteLine($"Started at {goResponse.startedAt}, eta {goResponse.eta}");
		Console.WriteLine($"Moving {goResponse.totalCells} cells in {goResponse.travelSeconds} seconds");
	}
	else
	{
		Console.WriteLine($"Error {goResponse.error}");
		Console.WriteLine($"{goResponse.message}");
	}
}
);

commands["travelstatus"] = ("Get travel status", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var travelStatusResponse = await Get<TravelStatusResponseDto>(Constants.TravelStatus, token);
	if (travelStatusResponse.isTraveling)
	{
		Console.WriteLine($"From [{travelStatusResponse.position.x},{travelStatusResponse.position.y}] to [{travelStatusResponse.destination.x},{travelStatusResponse.destination.y}]");
		Console.WriteLine($"Current [{travelStatusResponse.estimatedCurrent.x},{travelStatusResponse.estimatedCurrent.y}]");
		Console.WriteLine($"Progress {travelStatusResponse.progressIndex} out of {travelStatusResponse.totalCells} cells");
		Console.WriteLine($"{travelStatusResponse.startedAt} {travelStatusResponse.eta}");
		if (travelStatusResponse.discoveries != null)
		{
			foreach (var discovery in travelStatusResponse.discoveries)
			{
				Console.WriteLine($"{discovery.name} ({discovery.id}) {discovery.terrain} {discovery.type} {discovery.category} [{discovery.x},{discovery.y}]");
				Console.WriteLine($"{discovery.description}");
				Console.WriteLine($"{discovery.level_min}-{discovery.level_max}");
				Console.WriteLine($"{discovery.discoveredAt}");
			}
		}
		if (travelStatusResponse.xp != null)
		{
			Console.WriteLine($"{travelStatusResponse.xp.xpGained} {travelStatusResponse.xp.totalXp}");
			if (travelStatusResponse.xp.levelUp != null)
			{
				Console.WriteLine($"{travelStatusResponse.xp.levelUp.newLevel}");
				Console.WriteLine($"{travelStatusResponse.xp.levelUp.hpGained}");
				Console.WriteLine($"{travelStatusResponse.xp.levelUp.manaGained}");
				Console.WriteLine($"{travelStatusResponse.xp.levelUp.statPointsGained}");
				if (travelStatusResponse.xp.levelUp.newSpells != null)
				{
					foreach (var spell in travelStatusResponse.xp.levelUp.newSpells)
					{
						Console.WriteLine($"{spell.name} ({spell.id}) {spell.manaCost}");
					}
				}
			}
		}
		if (travelStatusResponse.encounter != null)
		{
			Console.WriteLine($"Encounter:");
			Console.WriteLine($"  {travelStatusResponse.encounter.monsterName} ({travelStatusResponse.encounter.monsterId})");
			Console.WriteLine($"  {travelStatusResponse.encounter.monsterLevel} {travelStatusResponse.encounter.monsterHp}/{travelStatusResponse.encounter.monsterMaxHp}HP");
			Console.WriteLine($"  {travelStatusResponse.encounter.monsterType} {travelStatusResponse.encounter.source}");
		}
	}
	else
	{
		Console.WriteLine("Not travelling");
	}
}
);

commands["travelcancel"] = ("Cancel active travel", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var travelCancelResponse = await Post<TravelCancelResponseDto>(string.Empty, Constants.TravelCancel, token);
	if (travelCancelResponse.cancelled)
	{
		Console.WriteLine("Travel cancelled");
	}
	Console.WriteLine(travelCancelResponse.cellsTraversed);
	Console.WriteLine($"[{travelCancelResponse.position.x},{travelCancelResponse.position.y}]");
	//Console.WriteLine(travelCancelResponse.xp);
	if (travelCancelResponse.discoveries != null)
	{
		foreach (var discovery in travelCancelResponse.discoveries)
		{
			Console.WriteLine($"{discovery.name} [{discovery.x},{discovery.y}]");
			Console.WriteLine($"  {discovery.description}");
			Console.WriteLine($"  {discovery.category} {discovery.type} {discovery.level_min}-{discovery.level_max}");
		}
	}
}
);

commands["map"] = ("Get world map", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}

	Console.WriteLine("Map command");
	var submap = ReadValidatedInput(["map", "pois", "discoveries"]);

	var mapResponse = await Get<MapResponseDto>(Constants.Map, token);

	if (submap == "map")
	{
		if (mapResponse?.terrain != null)
		{
			foreach (var row in mapResponse.terrain)
			{
				if (row != null)
				{
					var existingColour = Console.ForegroundColor;
					foreach (var cell in row)
					{
						var terrainInfo = TerrainGlyph.FromTerrain(cell);
						Console.ForegroundColor = terrainInfo.Item2;
						Console.Write(terrainInfo.Item1);
					}
					Console.ForegroundColor = existingColour;
				}
				Console.WriteLine();
			}
		}
	}

	if (submap == "pois")
	{
		if (mapResponse.pois != null)
		{
			foreach (var poi in mapResponse.pois)
			{
				Console.WriteLine($"{poi.name} [{poi.x},{poi.y}]");
				Console.WriteLine($"  {poi.description}");
				Console.WriteLine($"  {poi.category} {poi.type} {poi.level_min}-{poi.level_max}");
			}
		}
		else
		{
			Console.WriteLine("No pois.");
		}
	}

	if (submap == "discoveries")
	{
		if (mapResponse.discoveries != null)
		{
			foreach (var discovery in mapResponse.discoveries)
			{
				Console.WriteLine($"{discovery.name} [{discovery.x},{discovery.y}]");
				Console.WriteLine($"  {discovery.description}");
				Console.WriteLine($"  {discovery.category} {discovery.type} {discovery.level_min}-{discovery.level_max}");
			}
		}
		else
		{
			Console.WriteLine("No discoveries.");
		}
	}
}
);

commands["regions"] = ("List map regions", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var regionsResponse = await Get<Region[]>(Constants.Regions, token);
	foreach (var region in regionsResponse)
	{
		Console.WriteLine($"{region.name} [{region.centerX},{region.centerY}]");
		Console.WriteLine($"  {region.description}");
	}
}
);

commands["mapdetail"] = ("Cell detail at x,y", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("X coordinate");
	var xMapDetail = Console.ReadLine();
	Console.WriteLine("Y coordinate");
	var yMapDetail = Console.ReadLine();
	var mapDetailResponse = await Get<MapDetailResponseDto>($"{Constants.MapDetail}/{xMapDetail}/{yMapDetail}", token);
	Console.WriteLine(mapDetailResponse.terrain);
	if (mapDetailResponse.poi != null)
	{
		Console.WriteLine($"{mapDetailResponse.poi.name} [{mapDetailResponse.poi.x},{mapDetailResponse.poi.y}]");
		Console.WriteLine($"  {mapDetailResponse.poi.description}");
		Console.WriteLine($"  {mapDetailResponse.poi.category} {mapDetailResponse.poi.type} {mapDetailResponse.poi.level_min}-{mapDetailResponse.poi.level_max}");
	}
}
);

commands["combatstatus"] = ("Get combat status", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var combatStatusResponse = await Get<CombatStatus>(Constants.CombatStatus, token);
	if (combatStatusResponse.inCombat)
	{
		Console.WriteLine($"In combat ({combatStatusResponse.source}) turn {combatStatusResponse.turn}");
		Console.WriteLine($"  Player:");
		Console.WriteLine($"    HP: {combatStatusResponse.player.hp} / {combatStatusResponse.player.maxHp}, AC: {combatStatusResponse.player.ac}");
		Console.WriteLine($"  Monster: {combatStatusResponse.monster.name} ({combatStatusResponse.monster.id})");
		Console.WriteLine($"    HP: {combatStatusResponse.monster.hp} / {combatStatusResponse.monster.maxHp}, AC: {combatStatusResponse.monster.ac}");
		foreach (var log in combatStatusResponse.log)
		{
			Console.WriteLine(log);
		}
	}
	else
	{
		Console.WriteLine("Not in combat");
	}
}
);

commands["combataction"] = ("Combat action (attack, cast, use_item, flee)", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("action");
	var combatActionInfo = ReadValidatedInput(["attack", "cast", "use_item", "flee"]);
	var combatAction = new CombatAction();
	combatAction.action = combatActionInfo;

	var spell = string.Empty;
	var item = string.Empty;
	if (combatActionInfo == "cast")
	{
		Console.WriteLine("Spell:");
		Console.Write(">");
		spell = Console.ReadLine();
	}
	if (combatActionInfo == "use_item")
	{
		Console.WriteLine("Item:");
		Console.Write(">");
		item = Console.ReadLine();
	}
	combatAction.spellId = spell;
	combatAction.itemId = item;
	jsonString = JsonSerializer.Serialize(combatAction, options);
	var combatActionResponse = await Post<CombatActionResult>(jsonString, Constants.CombatAction, token);
	foreach (var log in combatActionResponse.log)
	{
		Console.WriteLine(log);
	}
	Console.WriteLine(combatActionResponse.outcome);
	if (combatActionResponse.outcome == "ongoing")
	{
		Console.WriteLine(combatActionResponse.monster.name);
		Console.WriteLine($"{combatActionResponse.monster.hp} / {combatActionResponse.monster.maxHp} [AC: {combatActionResponse.monster.ac}]");
	}
}
);

commands["inventory"] = ("Get inventory", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var subinventory = ReadValidatedInput(["backpack", "equipment"]);

	var inventoryResponse = await Get<Inventory>(Constants.Inventory, token);
	if (inventoryResponse == null)
	{
		Console.WriteLine("(No inventory / request failed)");
		return;
	}

	Console.WriteLine($"{inventoryResponse.carryWeight}lbs / {inventoryResponse.carryCapacity}lbs");

	if (subinventory == "equipment")
	{
		if (inventoryResponse.equipment.weapon != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.weapon.name} ({inventoryResponse.equipment.weapon.id}) {inventoryResponse.equipment.weapon.type} {inventoryResponse.equipment.weapon.rarity}");
		}

		if (inventoryResponse.equipment.armor != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.armor.name} ({inventoryResponse.equipment.armor.id}) {inventoryResponse.equipment.armor.type} {inventoryResponse.equipment.armor.rarity}");
		}

		if (inventoryResponse.equipment.helmet != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.helmet.name} ({inventoryResponse.equipment.helmet.id}) {inventoryResponse.equipment.helmet.type} {inventoryResponse.equipment.helmet.rarity}");
		}

		if (inventoryResponse.equipment.shield != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.shield.name} ({inventoryResponse.equipment.shield.id}) {inventoryResponse.equipment.shield.type} {inventoryResponse.equipment.shield.rarity}");
		}

		if (inventoryResponse.equipment.leggings != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.leggings.name} ({inventoryResponse.equipment.leggings.id}) {inventoryResponse.equipment.leggings.type} {inventoryResponse.equipment.leggings.rarity}");
		}

		if (inventoryResponse.equipment.boots != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.boots.name} ({inventoryResponse.equipment.boots.id}) {inventoryResponse.equipment.boots.type} {inventoryResponse.equipment.boots.rarity}");
		}

		if (inventoryResponse.equipment.ring1 != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.ring1.name} ({inventoryResponse.equipment.ring1.id}) {inventoryResponse.equipment.ring1.type} {inventoryResponse.equipment.ring1.rarity}");
		}

		if (inventoryResponse.equipment.ring1 != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.ring1.name} ({inventoryResponse.equipment.ring1.id}) {inventoryResponse.equipment.ring1.type} {inventoryResponse.equipment.ring1.rarity}");
		}

		if (inventoryResponse.equipment.ring2 != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.ring2.name} ({inventoryResponse.equipment.ring2.id}) {inventoryResponse.equipment.ring2.type} {inventoryResponse.equipment.ring2.rarity}");
		}

		if (inventoryResponse.equipment.amulet != null)
		{
			Console.WriteLine($"{inventoryResponse.equipment.amulet.name} ({inventoryResponse.equipment.amulet.id}) {inventoryResponse.equipment.amulet.type} {inventoryResponse.equipment.amulet.rarity}");
		}
	}

	if (subinventory == "backpack")
	{
		foreach (var item in inventoryResponse.backpack)
		{
			Console.WriteLine($"{item.name} ({item.itemId}) x{item.quantity} {item.weight}lbs");
			Console.WriteLine($"Type: {item.type} Rarity: ({item.rarity}) Level required: {item.levelRequired} Class restriction: {item.classRestriction}");
		}
	}
}
);

commands["pickup"] = ("Pick up an item", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("itemid");
	var itemid = Console.ReadLine();
	Console.WriteLine("quantity");
	var quantity = Console.ReadLine();
	var pickup = new Pickup();
	pickup.itemId = itemid;
	pickup.quantity = Convert.ToInt32(quantity);
	jsonString = JsonSerializer.Serialize(pickup, options);
	var pickupResponse = await Post<PickupResponse>(jsonString, Constants.Pickup, token);
	Console.WriteLine(pickupResponse.message);
}
);

commands["drop"] = ("Drop an item", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("itemid");
	var itemidDropped = Console.ReadLine();
	Console.WriteLine("quantity");
	var quantityDropped = Console.ReadLine();
	var drop = new Drop();
	drop.itemId = itemidDropped;
	drop.quantity = Convert.ToInt32(quantityDropped);
	jsonString = JsonSerializer.Serialize(drop, options);
	var droppedResponse = await Post<DropResponse>(jsonString, Constants.Drop, token);
	Console.WriteLine(droppedResponse.message);
}
);

commands["equip"] = ("Equip an item", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("itemid");
	var itemidEquip = Console.ReadLine();
	Console.WriteLine("slot");
	var slot = ReadValidatedInput(["weapon", "armor", "helmet", "shield", "leggings", "boots", "gloves", "ring1", "ring2", "amulet"]);
	var equip = new Equip();
	equip.itemId = itemidEquip;
	equip.slot = slot;
	jsonString = JsonSerializer.Serialize(equip, options);
	var equipResponse = await Post<EquipResponse>(jsonString, Constants.Equip, token);
	Console.WriteLine(equipResponse.message);
	if (equipResponse.unequipped != null)
	{
		Console.WriteLine(equipResponse.unequipped.item.itemId);
	}
}
);

commands["unequip"] = ("Unequip slot", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("slot");
	var unequipslot = ReadValidatedInput(["weapon", "armor", "helmet", "shield", "leggings", "boots", "gloves", "ring1", "ring2", "amulet"]);
	var unequip = new Unequip();
	unequip.slot = unequipslot;
	jsonString = JsonSerializer.Serialize(unequip, options);
	var unequipResponse = await Post<UnequipResponse>(jsonString, Constants.Unequip, token);
	Console.WriteLine(unequipResponse.message);
}
);

commands["use"] = ("Use consumable", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("itemid");
	var useitem = Console.ReadLine();
	var use = new Use();
	use.itemId = useitem;
	jsonString = JsonSerializer.Serialize(use, options);
	var useResponse = await Post<UseResponse>(jsonString, Constants.Use, token);
	if (!string.IsNullOrEmpty(useResponse.used))
	{
		Console.WriteLine(useResponse.used);
		Console.WriteLine(useResponse.message);
		Console.WriteLine(useResponse.result);
		Console.WriteLine($"{useResponse.hp}/{useResponse.maxHp}HP");
		Console.WriteLine($"{useResponse.mana}/{useResponse.maxMana}mana");
		Console.WriteLine($"{useResponse.learned}");
		if (useResponse.effect != null)
		{
			Console.WriteLine($"{useResponse.effect.type} {useResponse.effect.duration} {useResponse.effect.value} {useResponse.effect.source}");
		}
	}
	else
	{
		Console.WriteLine(useResponse.message);
	}
}
);

commands["spellbook"] = ("Get spellbook", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var spellbookResponse = await Get<SpellbookResponse>(Constants.Spells, token);
	Console.WriteLine($"{spellbookResponse.mana}/{spellbookResponse.maxMana}");
	foreach (var spell in spellbookResponse.spells)
	{
		Console.WriteLine($"{spell.name} ({spell.id})");
		Console.WriteLine($"  School: {spell.school} Level: {spell.spell_level} Mana: {spell.mana_cost}");
	}
}
);

commands["spelllearn"] = ("Learn spell", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("spellId");
	var learnSpellId = Console.ReadLine();
	var learnSpell = new LearnSpell();
	learnSpell.spellId = learnSpellId;
	jsonString = JsonSerializer.Serialize(learnSpell, options);
	var learnSpellResponse = await Post<LearnSpellResponse>(jsonString, Constants.SpellsLearn, token);
	Console.WriteLine(learnSpellResponse.message);
	Console.WriteLine(learnSpellResponse.learned);
}
);

commands["spellcast"] = ("Cast spell (out of combat)", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("spellId");
	var castSpellId = Console.ReadLine();
	var castSpell = new CastSpell();
	castSpell.spellId = castSpellId;
	jsonString = JsonSerializer.Serialize(castSpell, options);
	var castSpellResponse = await Post<CastSpellResponse>(jsonString, Constants.SpellsCast, token);
	if (String.IsNullOrEmpty(castSpellResponse.error))
	{
		Console.WriteLine($"{castSpellResponse.cast} {castSpellResponse.manaCost}");
		Console.WriteLine($"{castSpellResponse.effect}");
		Console.WriteLine($"{castSpellResponse.result}");
		if (castSpellResponse.effect != null)
		{
			Console.WriteLine($"{castSpellResponse.effect.type} {castSpellResponse.effect.duration} {castSpellResponse.effect.value} {castSpellResponse.effect.source}");
		}
	}
	else
	{
		Console.WriteLine($"Error {castSpellResponse.error}");
		Console.WriteLine($"{castSpellResponse.message}");
	}
}
);

commands["restcamp"] = ("Start camping", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("duration seconds (10-600)");
	var campDuration = Console.ReadLine();
	var startCamp = new StartCampRequest();
	startCamp.duration = Convert.ToInt32(campDuration);
	jsonString = JsonSerializer.Serialize(startCamp, options);
	var startCampResponse = await Post<StartCampResponse>(jsonString, Constants.RestCamp, token);

	if (String.IsNullOrEmpty(startCampResponse.error))
	{
		Console.WriteLine($"{startCampResponse.resting}");
		Console.WriteLine($"{startCampResponse.type}");
		Console.WriteLine($"{startCampResponse.startedAt} {startCampResponse.until}");
		if (startCampResponse.willRecover != null)
		{
			Console.WriteLine($"{startCampResponse.willRecover.hp} {startCampResponse.willRecover.mana}");
		}
	}
	else
	{
		Console.WriteLine($"Error {startCampResponse.error}");
		Console.WriteLine($"{startCampResponse.message}");
	}
}
);

commands["restinn"] = ("Rest at an inn", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var innRestResponse = await Post<InnRestResponse>(string.Empty, Constants.RestInn, token);
	if (String.IsNullOrEmpty(innRestResponse.error))
	{
		if (innRestResponse.rested)
		{
			Console.WriteLine($"Rested {innRestResponse.type} - {innRestResponse.location}");
			Console.WriteLine($"HP restored {innRestResponse.hpRestored} ({innRestResponse.hp}/{innRestResponse.maxHp})");
			Console.WriteLine($"Mana restored {innRestResponse.manaRestored} ({innRestResponse.mana}/{innRestResponse.maxMana})");
		}
		else
		{
			Console.WriteLine($"Not rested");
		}
	}
	else
	{
		Console.WriteLine($"Error {innRestResponse.error}");
		Console.WriteLine($"{innRestResponse.message}");
	}
}
);

commands["reststatus"] = ("Get rest status", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var restStatusResponse = await Get<RestStatus>(Constants.RestStatusPath, token);
	if (restStatusResponse.resting)
	{
		Console.WriteLine($"{restStatusResponse.type}");
		Console.WriteLine($"{restStatusResponse.startedAt} {restStatusResponse.until}");
		Console.WriteLine($"{restStatusResponse.elapsedSeconds} {restStatusResponse.totalDurationSeconds}");
		Console.WriteLine($"Current: {restStatusResponse.currentHp}/{restStatusResponse.maxHp}HP, {restStatusResponse.currentMana}/{restStatusResponse.maxMana} mana");
		Console.WriteLine($"Recovered: {restStatusResponse.recovered.hp}HP, {restStatusResponse.recovered.mana} mana");
	}
	else
	{
		Console.WriteLine($"Not resting");
	}
}
);

commands["reststop"] = ("Stop resting", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var stopRestResponse = await Post<StopRestResponse>(string.Empty, Constants.RestStop, token);
	if (String.IsNullOrEmpty(stopRestResponse.error))
	{
		Console.WriteLine($"Stopped: {stopRestResponse.stopped} Completed: {stopRestResponse.completed}");
		Console.WriteLine($"{stopRestResponse.elapsedSeconds}");
		Console.WriteLine($"{stopRestResponse.elapsedSeconds}");
		Console.WriteLine($"Current: {stopRestResponse.hp}/{stopRestResponse.maxHp}HP, {stopRestResponse.mana}/{stopRestResponse.maxMana} mana");
		Console.WriteLine($"Recovered: {stopRestResponse.recovered.hp}HP, {stopRestResponse.recovered.mana} mana");
	}
	else
	{
		Console.WriteLine(stopRestResponse.message);
	}
}
);

commands["shop"] = ("Shop inventory", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("townId");
	var shopTownId = Console.ReadLine();
	var shopResponse = await Get<ShopResponse>($"{Constants.Shops}/{shopTownId}", token);
	if (String.IsNullOrEmpty(shopResponse.error))
	{
		Console.WriteLine($"{shopResponse.shop}");
		Console.WriteLine($"{shopResponse.description} Restock: {shopResponse.restockMinutes}m Gold: {shopResponse.gold}");
		foreach (var item in shopResponse.items)
		{
			Console.WriteLine($"  {item.name} ({item.id})");
			Console.WriteLine($"    {item.type} {item.rarity}");
			Console.WriteLine($"    Price: {item.price} Sell price: {item.sellPrice}");
			Console.WriteLine($"    Level required: {item.levelRequired} Class restriction: {item.classRestriction}");
			Console.WriteLine($"    {item.weight}lbs Stock: {item.stock}/{item.maxStock}");
		}
	}
	else
	{
		Console.WriteLine($"Error {shopResponse.error}");
		Console.WriteLine($"{shopResponse.message}");
	}
}
);

commands["shopbuy"] = ("Buy items", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
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
	Console.WriteLine(buyResponse.message);
	Console.WriteLine($"Remaining gold: {buyResponse.gold}");
}
);

commands["shopsell"] = ("Sell items", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
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
	Console.WriteLine(sellResponse.message);
	Console.WriteLine($"Remaining gold: {sellResponse.gold}");
}
);

commands["questavailable"] = ("List available quests (at current town)", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var questAvailableResponse = await Get<AvailableQuestsResponse>(Constants.QuestAvailable, token);
	foreach (var quest in questAvailableResponse.quests)
	{
		Console.WriteLine($"{quest.name} ({quest.id})");
		Console.WriteLine($"  {quest.description}");
		Console.WriteLine($"  Level {quest.levelMin}-{quest.levelMax}");
		foreach (var objective in quest.objectives)
		{
			Console.WriteLine($"  {objective.type}");
			Console.WriteLine($"  {objective.description}");
			Console.WriteLine($"  {objective.target}");
			Console.WriteLine($"  {objective.required}");
			Console.WriteLine($"  {objective.quantity}");
			//Console.WriteLine($"  {objective.current}");
		}
		Console.WriteLine($"  XP: {quest.rewards.xp}");
		Console.WriteLine($"  Gold: {quest.rewards.gold}");
		foreach (var item in quest.rewards.items)
		{
			Console.WriteLine($"  {item.itemId} {item.quantity}x");
		}

	}
}
);

commands["questactive"] = ("List active quests", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var questActiveResponse = await Get<ActiveQuestsResponse>(Constants.QuestActive, token);

	Console.WriteLine($"Active quests: {questActiveResponse.activeCount} / {questActiveResponse.maxActive}");
	foreach (var quest in questActiveResponse.quests)
	{
		Console.WriteLine($"{quest.name} ({quest.id}) - {quest.status}");
		Console.WriteLine($"  {quest.description}");
		Console.WriteLine($"  Giver: {quest.giverTown}");
		foreach (var objective in quest.objectives)
		{
			Console.WriteLine($"  - {objective.description}");
			Console.WriteLine($"  - {objective.target} - {objective.current}");
		}
		Console.WriteLine($"  Rewards: {quest.rewards.gold} gold, {quest.rewards.xp} xp");
		foreach (var item in quest.rewards.items)
		{
			Console.WriteLine($" - ({item.itemId}) x{item.quantity}");
		}
	}
}
);

commands["questaccept"] = ("Accept quest", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("questId");
	var acceptQuestId = Console.ReadLine();
	var acceptQuest = new AcceptQuest();
	acceptQuest.questId = acceptQuestId;
	jsonString = JsonSerializer.Serialize(acceptQuest, options);
	var acceptQuestResponse = await Post<AcceptQuestResponse>(jsonString, Constants.QuestAccept, token);
	Console.WriteLine(acceptQuestResponse.message);
}
);

commands["questturnin"] = ("Complete a quest", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("questId");
	var turnInQuestId = Console.ReadLine();
	var turnInQuest = new TurnInQuest();
	turnInQuest.questId = turnInQuestId;
	jsonString = JsonSerializer.Serialize(turnInQuest, options);
	var turnInQuestResponse = await Post<TurnInQuestResponse>(jsonString, Constants.QuestTurnIn, token);
	Console.WriteLine(turnInQuestResponse.message);
	if (turnInQuestResponse.rewards != null)
	{
		foreach (var item in turnInQuestResponse.rewards.items)
		{
			Console.WriteLine($"  {item.name} ({item.itemId}) {item.quantity}x");
		}
	}
}
);

commands["questabandon"] = ("Abandon a quest", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("questId");
	var abandonQuestId = Console.ReadLine();
	var abandonQuest = new AbandonQuest();
	abandonQuest.questId = abandonQuestId;
	jsonString = JsonSerializer.Serialize(abandonQuest, options);
	var abandonQuestResponse = await Post<AbandonQuestResponse>(jsonString, Constants.QuestAbandon, token);
	Console.WriteLine(abandonQuestResponse.message);
}
);

commands["dungeonstatus"] = ("Dungeon status", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var dungeonStatusResponse = await Get<DungeonStatus>(Constants.DungeonStatusPath, token);
	if (dungeonStatusResponse.inDungeon)
	{
		Console.WriteLine($"{dungeonStatusResponse.poiName} ({dungeonStatusResponse.dungeonId}) {(dungeonStatusResponse.completed ? "Completed" : string.Empty)}");
		Console.WriteLine($"{dungeonStatusResponse.poiName} ({dungeonStatusResponse.poiId})");
		Console.WriteLine($"{dungeonStatusResponse.dungeonLevel} ({dungeonStatusResponse.rooms} / {dungeonStatusResponse.totalRooms})");
		foreach (var room in dungeonStatusResponse.rooms)
		{
			Console.WriteLine($"Room index: {room.index} {room.type} Cleared {room.cleared}");
			foreach (var log in room.log)
			{
				Console.WriteLine($"  {log}");
			}
		}
	}
	else
	{
		Console.WriteLine("Not in a dungeon");
	}
}
);

commands["dungeonenter"] = ("Enter dungeon", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("poiId");
	var dungeonPoiId = Console.ReadLine();
	var enterDungeon = new EnterDungeon();
	enterDungeon.poiId = dungeonPoiId;
	jsonString = JsonSerializer.Serialize(enterDungeon, options);
	var enterDungeonResponse = await Post<DungeonStatus>(jsonString, Constants.DungeonEnter, token);
	if (String.IsNullOrEmpty(enterDungeonResponse.error))
	{
		Console.WriteLine($"{enterDungeonResponse.dungeonId} {enterDungeonResponse.poiName} ({enterDungeonResponse.poiId}) Completed: {enterDungeonResponse.completed}");
		Console.WriteLine($"{enterDungeonResponse.dungeonLevel} Room {enterDungeonResponse.currentRoom} Total rooms: {enterDungeonResponse.totalRooms})");
		foreach (var room in enterDungeonResponse.rooms)
		{
			Console.WriteLine($"Room index: {room.index} {room.type} Cleared {room.cleared}");
			foreach (var log in room.log)
			{
				Console.WriteLine($"  {log}");
			}
		}
	}
	else
	{
		Console.WriteLine($"Error {enterDungeonResponse.error}");
		Console.WriteLine($"{enterDungeonResponse.message}");
	}
}
);

commands["dungeonadvance"] = ("Advance to next dungeon room", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var dungeonAdvanceResponse = await Post<DungeonAdvanceResponse>(string.Empty, Constants.DungeonAdvance, token);
	if (String.IsNullOrEmpty(dungeonAdvanceResponse.error))
	{
		Console.WriteLine($"  Room: {dungeonAdvanceResponse.roomIndex} {dungeonAdvanceResponse.roomType}");
		Console.WriteLine($"  {dungeonAdvanceResponse.status}");
		Console.WriteLine($"  {dungeonAdvanceResponse.message}");
		Console.WriteLine($"  {dungeonAdvanceResponse.trapType}");
		Console.WriteLine($"  {dungeonAdvanceResponse.avoided}");
		Console.WriteLine($"  {dungeonAdvanceResponse.hp}/{dungeonAdvanceResponse.maxHp}HP {dungeonAdvanceResponse.mana}/{dungeonAdvanceResponse.maxMana}mana");

		foreach (var log in dungeonAdvanceResponse.log)
		{
			Console.WriteLine(log);
		}

		Console.WriteLine($"  Reward");
		Console.WriteLine($"  {dungeonAdvanceResponse.gold}");
		foreach (var loot in dungeonAdvanceResponse.loot)
		{
			Console.WriteLine($"  {loot.itemId} {loot.quantity}x");
		}

		if (dungeonAdvanceResponse.recovered != null)
		{
			Console.WriteLine($"  Recovered");
			Console.WriteLine($"  {dungeonAdvanceResponse.recovered.hp}HP {dungeonAdvanceResponse.recovered.mana}mana");
		}
	}
	else
	{
		Console.WriteLine($"Error {dungeonAdvanceResponse.error}");
		Console.WriteLine($"{dungeonAdvanceResponse.message}");
	}
}
);

commands["dungeonleave"] = ("Leave dungeon", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var dungeonLeaveResponse = await Post<DungeonLeaveResponse>(string.Empty, Constants.DungeonLeave, token);
	Console.WriteLine(dungeonLeaveResponse.message);
	if (dungeonLeaveResponse.left)
	{
		Console.WriteLine($"Position: [{dungeonLeaveResponse.position.x},{dungeonLeaveResponse.position.y}]");
	}
}
);

commands["gatheringskills"] = ("Gathering skill levels", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var gatheringSkillsResponse = await Get<GatheringSkillsResponse>(Constants.GatheringSkills, token);
	Console.WriteLine($"Herbalism: Level {gatheringSkillsResponse.skills.herbalism.level} XP {gatheringSkillsResponse.skills.herbalism.xp} XP to level {gatheringSkillsResponse.skills.herbalism.xpToNext}");
	Console.WriteLine($"Mining: Level {gatheringSkillsResponse.skills.mining.level} XP {gatheringSkillsResponse.skills.mining.xp} XP to level {gatheringSkillsResponse.skills.mining.xpToNext}");
	Console.WriteLine($"Woodcutting: Level {gatheringSkillsResponse.skills.woodcutting.level} XP {gatheringSkillsResponse.skills.woodcutting.xp} XP to level {gatheringSkillsResponse.skills.woodcutting.xpToNext}");
}
);

commands["gatheringnodes"] = ("Nearby gathering nodes", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var gatheringNodesResponse = await Get<GatheringNodesResponse>(Constants.GatheringNodes, token);
	if (gatheringNodesResponse.nodes.Length > 0)
	{
		foreach (var node in gatheringNodesResponse.nodes)
		{
			Console.WriteLine($"{node.name} ({node.poiId}) [{node.skill}]");
			Console.WriteLine($"{node.ready} ({node.cooldownMinutes}) XP: [{node.xpReward}]");
		}
	}
}
);

commands["gatheringharvest"] = ("Harvest node", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("nodeId");
	var harvestNodeId = Console.ReadLine();
	var harvest = new Harvest();
	harvest.nodeId = harvestNodeId;
	jsonString = JsonSerializer.Serialize(harvest, options);
	var harvestResponse = await Post<HarvestResponse>(jsonString, Constants.GatheringHarvest, token);
	Console.WriteLine($"{harvestResponse.harvested} ({harvestResponse.itemId}) {harvestResponse.quantity}x");
	Console.WriteLine($"{harvestResponse.skill} XP gained: {harvestResponse.xpGained} XP: {harvestResponse.skillXp} Level: {harvestResponse.skillLevel}");
	if (harvestResponse.levelUp != null)
	{
		Console.WriteLine($"Levelled up: {harvestResponse.levelUp.skill} - new level {harvestResponse.levelUp.newLevel}");
	}
	Console.WriteLine($"Cooldown: {harvestResponse.cooldownMinutes} minutes - available at {harvestResponse.availableAt}");
}
);

commands["craftingskills"] = ("Crafting skill levels", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var craftingSkillsResponse = await Get<CraftingSkillsResponse>(Constants.CraftingSkills, token);
	Console.WriteLine($"Alchemy: Level {craftingSkillsResponse.skills.alchemy.level} XP {craftingSkillsResponse.skills.alchemy.xp} XP to level {craftingSkillsResponse.skills.alchemy.xpToNext}");
	Console.WriteLine($"Blacksmithing: Level {craftingSkillsResponse.skills.blacksmithing.level} XP {craftingSkillsResponse.skills.blacksmithing.xp} XP to level {craftingSkillsResponse.skills.blacksmithing.xpToNext}");
	Console.WriteLine($"Woodworking: Level {craftingSkillsResponse.skills.woodworking.level} XP {craftingSkillsResponse.skills.woodworking.xp} XP to level {craftingSkillsResponse.skills.woodworking.xpToNext}");
}
);

commands["craftingrecipes"] = ("Crafting recipes", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("skill filter (blacksmithing, alchemy, woodworking, blank for all)");
	var craftingSkillFilter = ReadValidatedInput(["blacksmithing", "alchemy", "woodworking", ""]);
	var recipesUrl = string.IsNullOrWhiteSpace(craftingSkillFilter) ? Constants.CraftingRecipes : $"{Constants.CraftingRecipes}?skill={craftingSkillFilter}";
	var craftingRecipesResponse = await Get<CraftingRecipesResponse>(recipesUrl, token);
	foreach (var recipie in craftingRecipesResponse.recipes)
	{
		Console.WriteLine($"{recipie.name} ({recipie.id})");
		Console.WriteLine($"  {recipie.skill} {recipie.minLevel} {recipie.meetsLevel} {recipie.hasIngredients} {recipie.canCraft} {recipie.xpReward}");
		foreach (var ingredient in recipie.ingredients)
		{
			Console.WriteLine($"    ({ingredient.itemId}) {ingredient.quantity}x {ingredient.have}");
		}
		Console.WriteLine($"  ({recipie.output.itemId}) {recipie.output.quantity}x");
	}

}
);

commands["craftingstations"] = ("Nearby crafting stations", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	var craftingStationsResponse = await Get<CraftingStationsResponse>(Constants.CraftingStations, token);
	Console.WriteLine($"Current position: [{craftingStationsResponse.position.x},{craftingStationsResponse.position.y}]");
	if (craftingStationsResponse.stations != null)
	{
		foreach (var station in craftingStationsResponse.stations)
		{
			Console.WriteLine($"  {station.name} ({station.id}) {station.type} {station.skill}");
			Console.WriteLine($"    {station.description}");
		}
	}
}
);

commands["craftingcraft"] = ("Craft item", async () =>
{
	if (string.IsNullOrWhiteSpace(token))
	{
		Console.WriteLine("You must be logged in to use this command.");
		return;
	}
	Console.WriteLine("recipeId");
	var recipeId = Console.ReadLine();
	var craft = new Craft();
	craft.recipeId = recipeId;
	jsonString = JsonSerializer.Serialize(craft, options);
	var craftResponse = await Post<CraftResponse>(jsonString, Constants.CraftingCraft, token);
	if (String.IsNullOrEmpty(craftResponse.error))
	{
		Console.WriteLine($"{craftResponse.crafted} ({craftResponse.itemId}) {craftResponse.quantity}x");
		Console.WriteLine($"{craftResponse.xpGained}");
		Console.WriteLine($"{craftResponse.skill}");
		Console.WriteLine($"{craftResponse.skillLevel}");
		Console.WriteLine($"{craftResponse.skillXp}");
		if (craftResponse.levelUp != null)
		{
			Console.WriteLine($"Skill {craftResponse.levelUp.skill} levelled up - new level {craftResponse.levelUp.newLevel}");
		}
	}
	else
	{
		Console.WriteLine($"Error {craftResponse.error}");
		Console.WriteLine(craftResponse.message);
	}
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

	Console.WriteLine(JsonSerializer.Serialize(gameItemsResponse, options));
	foreach (var item in gameItemsResponse)
	{
		Console.WriteLine($"{item.name} ({item.id})");
		Console.WriteLine($"  {item.type} ({item.rarity})");
		Console.WriteLine($"  {item.damage_min} ({item.damage_max})");
		Console.WriteLine($"  {item.armor}");
		Console.WriteLine($"  {item.value}");
		Console.WriteLine($"  {item.weight}lbs");
		Console.WriteLine($"  {item.level_req}");
		Console.WriteLine($"  {item.class_req}");
		//Console.WriteLine($"  {item.statusEffect}");
		Console.WriteLine($"  {item.statusChance}");
	}
}
);

commands["gamespells"] = ("Game data: all spells", async () =>
{
	var gameSpellsResponse = await Get<GameSpellDef[]>(Constants.GameSpells, token);

	foreach (var spell in gameSpellsResponse)
	{
		Console.WriteLine($"{spell.name} ({spell.id}) {spell.level_req} {spell.mana_cost}");
		Console.WriteLine($"  {spell.description}");
		Console.WriteLine($"  {spell.cooldown}");
		Console.WriteLine($"  {spell.targetType}");
		Console.WriteLine($"  {spell.effectType}");
		Console.WriteLine($"  {spell.effectValue}");
		Console.WriteLine($"  {spell.damageType}");
		Console.WriteLine($"  {spell.spellClass}");
		//Console.WriteLine($"  {spell.statusEffect}");
	}
}
);

commands["help"] = ("Show this command list", () =>
{
	foreach (var kv in commands.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
	{
		Console.WriteLine($"  {kv.Key,-22} {kv.Value.Help}");
	}
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
		var result = JsonSerializer.Deserialize<T>(body, jsonReadOptions);
		if (result == null)
		{
			Console.WriteLine($"Failed to deserialize response: {body}");
			return default!;
		}
		return result;
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		return default!;
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
		var result = JsonSerializer.Deserialize<T>(body, jsonReadOptions);
		if (result == null)
		{
			Console.WriteLine($"Failed to deserialize response: {body}");
			return default!;
		}
		return result;
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		return default!;
	}
}

string ReadValidatedInput(string[] validoptions)
{
	foreach (var option in validoptions)
	{
		Console.WriteLine($"  {option}");
	}

	Console.Write(">");
	var response = Console.ReadLine();
	while (validoptions != null && !validoptions.Contains(response))
	{
		Console.WriteLine($"Invalid response. Valid options: {string.Join(", ", validoptions)}");
		Console.Write(">");
		response = Console.ReadLine();
	}
	return response ?? string.Empty;
}

int ReadValidatedIntInput(int min, int max)
{
	Console.Write(">");
	var response = Console.ReadLine();
	while (!(Int32.TryParse(response, out var value) && value >= min && value <= max))
	{
		Console.WriteLine($"Invalid response. Valid range {min}-{max}");
		Console.Write(">");
		response = Console.ReadLine();
	}
	return Convert.ToInt32(response);
}

static DateTime? GetBuildDate(Assembly assembly)
{
	var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
	if (string.IsNullOrEmpty(info))
		return null;

	const string prefix = "+build";
	var index = info.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
	if (index < 0)
		return null;

	var value = info[(index + prefix.Length)..];

	if (DateTime.TryParseExact(
			value,
			"yyyyMMddHHmmss",
			CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
			out var date))
	{
		return date;
	}

	return null;
}

file static class TerrainGlyph
{
	public static (string, ConsoleColor) FromTerrain(string t)
	{
		if (string.IsNullOrWhiteSpace(t))
			return ("__", ConsoleColor.Gray);
		return t.Trim().ToLowerInvariant() switch
		{
			"ocean" => ("oo", ConsoleColor.DarkBlue),
			"forest" => ("ff", ConsoleColor.DarkGreen),
			"plains" => ("pp", ConsoleColor.Green),
			"coast" => ("cc", ConsoleColor.Blue),
			"swamp" => ("ss", ConsoleColor.Magenta),
			"mountain" => ("mm", ConsoleColor.DarkYellow),
			"desert" => ("dd", ConsoleColor.Yellow),
			_ => ("__", ConsoleColor.Gray)
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