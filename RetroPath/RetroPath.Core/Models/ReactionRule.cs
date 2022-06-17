using System.Collections;
using GraphMolWrap;
using RetroPath.Core.Extensions;

namespace RetroPath.Core.Models;

public class ReactionRule
{
    public HashSet<string> RuleIds { get; set; } = null!;
    public string RuleSmarts { get; set; } = null!;
    public HashSet<string> EcNumber { get; set; } = null!;
    public int ReactionOrder { get; set; }
    public int Diameter { get; set; }
    public double Score { get; set; }
    
    public ReactionRuleLeft? Left { get; private set; }

    public string GetFoldedRuleId() => $"[{string.Join(",", RuleIds)}]@{Diameter}";

    public bool IsMono() => ReactionOrder <= 1;

    public void CalculateFingerprint()
    {
        if (Left is not null) return; // already calculated;

        var smartsLeft = RuleSmarts.Split(">>")[0];
        if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
        {
            smartsLeft = smartsLeft[1..^1];
        }

        BitArray? arrFingerprint;
        using (var mol = RWMol.MolFromSmarts(smartsLeft, 0, true))
        {
            using (var fp = RDKFuncs.PatternFingerprintMol(mol, FingerprintSettings.PreProcessingPatternFingerprintSize))
            {
                arrFingerprint = fp.ToCsBitArray();
            }
        }

        var cardinality = arrFingerprint.FastGetCardinality();

        Left = new(smartsLeft, cardinality, arrFingerprint);
    }
}