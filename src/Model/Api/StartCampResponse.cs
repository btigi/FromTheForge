public class StartCampResponse
{
	public bool resting { get; set; }
	public string type { get; set; }
	public int durationSeconds { get; set; }
	public DateTime startedAt { get; set; }
	public DateTime until { get; set; }
	public Position position { get; set; }
	public RestRecoveryAmount willRecover { get; set; }

	public string error { get; set; }
	public string message { get; set; }
	public int statusCode { get; set; }
}
