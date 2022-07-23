using GraphMolWrap;
using RetroPath.Chem.Fingerprints;

namespace RetroPath.Core.Chem.Reactions;

public class ReactionRule
{
    private Fingerprint? _cachedLeftFingerprint;
    
    public HashSet<string> RuleIds { get; }
    public string RuleSmarts { get; }
    public HashSet<string> EcNumber { get; }
    public int Diameter { get; }
    
    public int ReactionOrder { get; set; }
    public double Score { get; set; }

    public string LeftSmarts { get; }

    public bool HasCachedFingerprint => _cachedLeftFingerprint is not null;
    public bool IsMono => ReactionOrder <= 1;
    public string FoldedRuleId => $"[{string.Join(",", RuleIds)}]@{Diameter}";

    public ReactionRule(
        HashSet<string> ruleIds,
        string ruleSmarts,
        HashSet<string> ecNumber,
        int diameter,
        int reactionOrder,
        double score
    )
    {
        RuleIds = ruleIds;
        RuleSmarts = ruleSmarts;
        EcNumber = ecNumber;
        Diameter = diameter;
        ReactionOrder = reactionOrder;
        Score = score;

        _cachedLeftFingerprint = default;
        LeftSmarts = GetLeftSideSmarts();
    }

    /// <summary>
    /// Explicitly calculates a <see cref="Fingerprint"/> for the left side of this rule and caches it.
    ///
    /// If a fingerprint has already been calculated and cached this will re-calculate and override it.
    /// </summary>
    /// <returns>The calculated <see cref="Fingerprint"/>.</returns>
    public Fingerprint CalculateLeftSideFingerprint()
    {
        using var mol = RWMol.MolFromSmarts(LeftSmarts, 0, true);
        
        _cachedLeftFingerprint = Fingerprint.CalcPatternFingerprintForMol(mol);

        return _cachedLeftFingerprint;
    }

    /// <summary>
    /// Retrieves the left <see cref="Fingerprint"/> from cache if present, calculates, caches the result and returns it otherwise.
    /// </summary>
    /// <returns>The cached calculated <see cref="Fingerprint"/> if present, newly calculated <see cref="Fingerprint"/> otherwise.</returns>
    public Fingerprint GetLeftFingerprint()
        => HasCachedFingerprint ? _cachedLeftFingerprint! : CalculateLeftSideFingerprint();
    
    private string GetLeftSideSmarts()
    {
        var smartsLeft = RuleSmarts.Split(">>")[0];
        if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
        {
            smartsLeft = smartsLeft[1..^1];
        }

        return smartsLeft;
    }
}