namespace RetroPath.Core.Models;

public class ReactionRule
{
    public HashSet<string> RuleIds { get; set; } = null!;
    public string RuleSmarts { get; set; } = null!;
    public HashSet<string> EcNumber { get; set; } = null!;
    public int ReactionOrder { get; set; }
    public int Diameter { get; set; }
    public double Score { get; set; }

    public string GetFoldedRuleId() => $"[{string.Join(",", RuleIds)}]@{Diameter}";
}