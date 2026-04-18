using System.Text;
using System.Text.Json;

const string email = "";
const string password = "";

const string Register = "/api/auth/register";
const string Login = "/api/auth/login";
const string CreateCharacter = "/api/characters";
const string GetCharacter = "/api/characters/me";
const string Travel = "/api/travel/move";
const string Go = "/api/travel/go";
const string TravelStatus = "/api/travel/status";
const string TravelCancel = "/api/travel/cancel";
const string Me = "/api/auth/me";


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
	public int level_min { get; set; }
	public int level_max { get; set; }
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