namespace FromTheForge.Model;

public class LoginResponse
{
	public string AccessToken { get; set; } = "";
	public string RefreshToken { get; set; } = "";
	public int ExpiresIn { get; set; }
	public User User { get; set; } = new User();

	public string Error { get; set; } = "";
	public string[] Message { get; set; } = [];
	public int StatusCode { get; set; }
}
