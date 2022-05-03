using System.Collections.Concurrent;
using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions.Reactions;

namespace RetroPath.Core;

public class RulesFirer
{
    private readonly ChemicalCompound _source;
    private readonly Dictionary<string, ChemicalCompound> _cofactors;
    private readonly List<ReactionRule> _rules;

    public RulesFirer(ChemicalCompound source, Dictionary<string, ChemicalCompound> cofactors, List<ReactionRule> rules)
    {
        _source = source;
        _cofactors = cofactors;
        _rules = rules;
    }

    public ConcurrentBag<GeneratedProduct> FireRules()
    {
        var groupedRules = _rules
            .AsParallel()
            .GroupBy(r => r.Diameter)
            .OrderByDescending(r => r.Key);

        var results = new ConcurrentBag<GeneratedProduct>();
        foreach (var rulesGrouping in groupedRules)
        {
            Parallel.ForEach(rulesGrouping, rule =>
            {
                if (rule.IsMono())
                {
                    var monoRes = ProcessMono(rule);

                    Parallel.ForEach(monoRes, product => results.Add(product));
                }
                else
                {
                    ProcessBi(rule);
                }
            });

            if (!results.IsEmpty)
            {
                break;
            }
        }

        return results;
    }

    private List<GeneratedProduct> ProcessMono(ReactionRule rule)
    {
        var generatedProducts = new List<GeneratedProduct>();
        var smartsLeft = rule.RuleSmarts.Split(">>")[0];

        if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
        {
            smartsLeft = smartsLeft[1..^1];
        }
        
        using var smartsLeftMol = RWMol.MolFromSmarts(smartsLeft);

        // justification for null suppression: it's a source compound so Mol will always be populated;
        if (_source.Mol!.hasSubstructMatch(smartsLeftMol))
        {
            try
            {
                using var rxn = new OneComponentReaction(rule.RuleSmarts, _source.Mol);
                var products = rxn.RunReaction();

                foreach (var p in products)
                {
                    var leftSplit = _source.Smiles.Split('.').ToList();
                    var rightSplit = p.Split('.').ToList();
                    var gp = new GeneratedProduct(leftSplit, rightSplit, rule);

                    generatedProducts.Add(gp);
                }
            }
            catch
            {
                // ignored
            }
        }
        
        return generatedProducts;
    }

    private void ProcessBi(ReactionRule rule) => throw new NotImplementedException();
}