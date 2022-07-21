using System.Collections.Concurrent;
using GraphMolWrap;
using RetroPath.Core.Extensions;
using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions.Reactions;

namespace RetroPath.Core;

public class RuleEngine
{
    private readonly List<ChemicalCompound> _sources;
    private readonly Dictionary<string, ChemicalCompound> _cofactors;
    private readonly List<IGrouping<int,ReactionRule>> _groupedRules;

    /// <summary>
    /// Constructor that instantiates a new instance of <see cref="RuleEngine"/>.
    /// </summary>
    /// <param name="sources">Sources over which to fire rules.</param>
    /// <param name="cofactors">Cofactors to use for bi-molecular reactions.</param>
    /// <param name="groupedRules">Rules grouped by diameter.</param>
    public RuleEngine(List<ChemicalCompound> sources, Dictionary<string, ChemicalCompound> cofactors, List<IGrouping<int,ReactionRule>> groupedRules)
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

                    foreach (var r in monoRes)
                    {
                        monoRes.Add(r);
                    }
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

        if (rule.Left is null)
        {
            rule.CalculateLeftFingerprint();
        }

        // justification: prefer explicit scope to show it's not disposed before ForEach and its lambda complete;
        // ReSharper disable once ConvertToUsingDeclaration
        using (var smartsLeftMol = RWMol.MolFromSmarts(rule.Left!.Smarts))
        {
            Parallel.ForEach(_sources, source =>
            {
                if (source.Fingerprint is null)
                {
                    source.CalculateFingerprint();
                }
                
                // We look for a potential match using fingerprints as that's faster than running a deep check on every rule/source combination.
                // The performance improvement is particularly big if there are many rules, which will be most of the time
                // as retropath is usually ran on databases of 200k+ reaction rules.
                //
                // A potential SSS(A,B) match is found if OBC(FP(A)) <= OBC(FP(B)) AND OBC(FP(A) & FB(B)) == OBC(FP(A)),
                // where A is the rule and B is the source molecule;
                //
                // Based on RDKit KNIME implementation of substructure filter: https://github.com/rdkit/knime-rdkit/blob/5fe11f9c021ca9b21d36d6231db62d943eb50eaa/org.rdkit.knime.nodes/src/org/rdkit/knime/nodes/moleculesubstructfilter/RDKitMoleculeSubstructFilterNodeModel.java#L484
                var shouldDeepCheck = rule.Left!.Fingerprint.Cardinality <= source.Fingerprint!.Cardinality &&
                                      rule.Left!.Fingerprint.FingerprintArr.NewAnd(source.Fingerprint!.FingerprintArr)
                                          .FastGetCardinality() == rule.Left!.Fingerprint.Cardinality;

                // justification for null suppression: it's a source compound so Mol will always be populated;
                // ReSharper disable once AccessToDisposedClosure justification: the lambda always completes before smartsLeftMol is disposed;
                if (!shouldDeepCheck || !source.Mol!.hasSubstructMatch(smartsLeftMol))
                {
                    return;
                }
                
                try
                {
                    using var rxn = new OneComponentReaction(rule.RuleSmarts, source.Mol);
                    var products = rxn.RunReaction();

                    foreach (var p in products)
                    {
                        var rxnLeftSideCompounds = source.Smiles.Split('.').ToList();
                        var rxnRightSideCompounds = p.Split('.').ToList();
                        var gp = new GeneratedProduct(rxnLeftSideCompounds, rxnRightSideCompounds, rule, source);

                        generatedProducts.Add(gp);
                    }
                }
                catch
                {
                    // ignored -  we don't care about failed reactions;
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