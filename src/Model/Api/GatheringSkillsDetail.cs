namespace FromTheForge.Model;

public class GatheringSkillsDetail
{
	public GatheringSkillEntry mining { get; set; } = new GatheringSkillEntry();
	public GatheringSkillEntry herbalism { get; set; } = new GatheringSkillEntry();
	public GatheringSkillEntry woodcutting { get; set; } = new GatheringSkillEntry();
}
