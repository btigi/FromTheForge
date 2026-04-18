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
