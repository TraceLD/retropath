using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using Serilog;

namespace RetroPath.Core.Parsers;

public sealed class RulesParser : Parser<List<ReactionRule>>
{
    private sealed class RawRule
    {
        [Name("Rule ID")] public string RuleId { get; set; } = null!;

        [Name("Rule")] public string RuleSmarts { get; set; } = null!;
        
        [Name("EC number")] public string? EcNumber { get; set; }
        
        public HashSet<string>? SplitEc { get; set; }
        
        [Name("Reaction order")] public int? ReactionOrder { get; set; }
        
        [Name("Diameter")] public int? Diameter { get; set; }
        
        [Name("Score")] public double? Score { get; set; }
    }

    public RulesParser(InputConfiguration inputConfiguration) : base(inputConfiguration)
    {
    }

    public override List<ReactionRule> Parse(string filePath)
    {
        // we will group rules based on SMARTS + Diameter to get rid of duplicates;
        Dictionary<(string ruleSmarts, int diameter), ReactionRule> groupedRules = new();
        
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var rules = csv.GetRecords<RawRule>();

        foreach (var r in rules)
        {
            if (r.Diameter < InputConfiguration.MinDiameter || r.Diameter > InputConfiguration.MaxDiameter)
            {
                continue;
            }

            r.EcNumber ??= "NOEC";
            r.ReactionOrder ??= 1;
            r.Diameter ??= 1;
            r.Score ??= 1;
            r.SplitEc = new HashSet<string>(r.EcNumber.Split(';'));

            (string ruleSmarts, int diameter) key = (r.RuleSmarts, r.Diameter.Value);

            if (groupedRules.TryGetValue(key, out var val))
            {
                val.RuleIds.Add(r.RuleId);
                val.ReactionOrder = Math.Max(val.ReactionOrder, r.ReactionOrder.Value);
                val.Score = Math.Min(val.Score, r.Score.Value);
                val.EcNumber.UnionWith(r.SplitEc);
            }
            else
            {
                groupedRules.Add(key, new ReactionRule
                {
                    RuleIds = new HashSet<string> {r.RuleId},
                    RuleSmarts = r.RuleSmarts,
                    Diameter = r.Diameter.Value,
                    ReactionOrder = r.ReactionOrder.Value,
                    Score = r.Score.Value,
                    EcNumber = r.SplitEc
                });
            }
        }

        var rulesList = groupedRules.Values.ToList();
        
        return rulesList;
    }
}