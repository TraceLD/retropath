using System.Collections.Concurrent;
using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions.Reactions;

namespace RetroPath.Core;

/// <summary>
/// Class that applies all the rules on source compounds.
/// </summary>
public class RulesFirer
{
    private readonly List<ChemicalCompound> _sources;
    private readonly Dictionary<string, ChemicalCompound> _cofactors;
    private readonly List<IGrouping<int,ReactionRule>> _groupedRules;

    /// <summary>
    /// Constructor that instantiates a new instance of <see cref="RulesFirer"/>.
    /// </summary>
    /// <param name="sources">Sources over which to fire rules.</param>
    /// <param name="cofactors">Cofactors to use for bi-molecular reactions.</param>
    /// <param name="groupedRules">Rules grouped by diameter.</param>
    public RulesFirer(List<ChemicalCompound> sources, Dictionary<string, ChemicalCompound> cofactors, List<IGrouping<int,ReactionRule>> groupedRules)
    {
        _sources = sources;
        _cofactors = cofactors;
        _groupedRules = groupedRules;
    }

    /// <summary>
    /// Fires the rules on the given sources.
    /// </summary>
    /// <returns>Generated products.</returns>
    public ConcurrentBag<GeneratedProduct> FireRules()
    {
        var results = new ConcurrentBag<GeneratedProduct>();
        foreach (var rulesGrouping in _groupedRules)
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

    /// <summary>
    /// Processes a monomolecular reaction.
    /// </summary>
    /// <param name="rule">Reaction rule.</param>
    /// <returns>Generated products.</returns>
    private ConcurrentBag<GeneratedProduct> ProcessMono(ReactionRule rule)
    {
        var generatedProducts = new ConcurrentBag<GeneratedProduct>();
        var smartsLeft = rule.RuleSmarts.Split(">>")[0];

        if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
        {
            smartsLeft = smartsLeft[1..^1];
        }

        // ReSharper disable once ConvertToUsingDeclaration justification: prefer explicit scope to show it's not disposed before ForEach and its lambda complete;
        using (var smartsLeftMol = RWMol.MolFromSmarts(smartsLeft))
        {
            Parallel.ForEach(_sources, source =>
            {
                // justification for null suppression: it's a source compound so Mol will always be populated;
                // ReSharper disable once AccessToDisposedClosure justification: the lambda always completes before smartsLeftMol is disposed;
                if (source.Mol!.hasSubstructMatch(smartsLeftMol))
                {
                    try
                    {
                        using var rxn = new OneComponentReaction(rule.RuleSmarts, source.Mol);
                        var products = rxn.RunReaction();

                        foreach (var p in products)
                        {
                            var leftSplit = source.Smiles.Split('.').ToList();
                            var rightSplit = p.Split('.').ToList();
                            var gp = new GeneratedProduct(leftSplit, rightSplit, rule, source);

                            generatedProducts.Add(gp);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
        }
        
        return generatedProducts;
    }

    // TODO: add code for processing bi reactions
    // TODO: postponed until later because all the rules in the big rules file are mono;
    /// <summary>
    /// Processes a bi-molecular reaction.
    /// </summary>
    /// <param name="rule">Reaction rule.</param>
    /// <returns>Generated products.</returns>
    private void ProcessBi(ReactionRule rule) => throw new NotImplementedException();
}