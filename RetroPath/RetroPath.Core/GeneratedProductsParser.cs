using System.Collections.Concurrent;
using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions;

namespace RetroPath.Core;

public class GeneratedProductsParser : IDisposable
{
    public record ParsedResults(
        List<ParsedGeneratedCompound> Left,
        List<ParsedGeneratedCompound> Right,
        Dictionary<string, TransformationInfo> TransformationInfos
    );
    
    public record TransformationIdCoeff(string TransformationId, int Coeff);

    internal record SideCompound(
        string SideSmiles,
        RWMol? Mol,
        List<string> TransformationIds,
        List<TransformationIdCoeff> TransformationIdCoeff
    ) : IDisposable
    {
        public void Dispose() => Mol?.Dispose();

        public ParsedGeneratedCompound ToParsedGeneratedCompound()
        {
            if (Mol is null)
            {
                throw new NullReferenceException(
                    "Mol cannot be empty when attempting to convert SideCompound to ParsedGeneratedCompound");
            }

            var inchi = Inchi.MolToInchiSimple(Mol);
            var liteInchi = LiteInchi.ToLiteInchiExtended(Mol);
            var smiles = RDKFuncs.getCanonSmiles(Mol);

            return new(TransformationIds, inchi!, liteInchi, smiles, Mol, TransformationIdCoeff);
        }
    }

    private record Transformation(
        List<string> Left,
        List<string> Right,
        Dictionary<string, ReactionRule> ReactionRules, // <foldedId, rule>
        ChemicalCompound Source,
        int SourceIter,
        int StepIter,
        int RId
    )
    {
        public string TransformationId => $"TRS_{SourceIter}_{StepIter}_{RId}";
    }

    private record ExtractRightResult(List<SideCompound> Res, HashSet<string> BadTransformations);
    
    private readonly ConcurrentBag<GeneratedProduct> _generatedProducts;
    private readonly int _sourceIter;
    private readonly int _stepIter;
    private readonly CompoundStandardiser _standardiser;

    public GeneratedProductsParser(ConcurrentBag<GeneratedProduct> generatedProducts, int sourceIter, int stepIter)
    {
        _generatedProducts = generatedProducts;
        _sourceIter = sourceIter;
        _stepIter = stepIter;
        _standardiser = new();
    }

    public ParsedResults Parse()
    {
        var transformations = GetTransformations();
        
        var extractedLeft = ExtractLeft(transformations);
        var (extractedRight, badTransformations) = ExtractRight(transformations);
        
        var filteredLeft = extractedLeft.FilterOutBadTransformations(badTransformations);
        var filteredRight = extractedRight.FilterOutBadTransformations(badTransformations);
        
        var parsedLeft = filteredLeft.Select(x => x.ToParsedGeneratedCompound()).ToList();
        var parsedRight = filteredRight.Select(x => x.ToParsedGeneratedCompound()).ToList();

        var transformationInfos = transformations
            // filter out bad transformations;
            .Where(t => !badTransformations.Contains(t.TransformationId))
            // each raw Transformation into TransformationInfo;
            .Select(t =>
            {
                var ruleIds = new HashSet<string>();
                var diameter = int.MinValue;
                var ecNumber = new HashSet<string>();
                var score = double.MaxValue;

                foreach (var (_, rule) in t.ReactionRules)
                {
                    ruleIds.UnionWith(rule.RuleIds);
                    diameter = Math.Max(diameter, rule.Diameter);
                    ecNumber.UnionWith(rule.EcNumber);
                    score = Math.Min(score, rule.Score);
                }

                return new TransformationInfo(t.TransformationId, ruleIds, diameter, ecNumber, score, t.Source.Names);
            })
            .ToList();

        // TODO: improve efficiency of this by creating the dict earlier;
        return new(parsedLeft, parsedRight, transformationInfos.ToDictionary(x => x.TransformationId));
    }

    private List<Transformation> GetTransformations() =>
        _generatedProducts
            .GroupBy(p => (string.Join(".", p.Left), string.Join(".", p.Right)))
            .Select((products, i) =>
            {
                var pList = products.ToList();
                var first = pList.First();
                var rules = new Dictionary<string, ReactionRule>();
                var left = first.Left;
                var right = first.Right;
                var allRules = products.Select(x => x.Rule);

                foreach (var r in allRules)
                {
                    var foldedId = r.GetFoldedRuleId();

                    if (!rules.ContainsKey(foldedId))
                    {
                        rules.Add(foldedId, r);
                    }
                }

                ChemicalCompound? source = null;
                for (var j = 0; j < pList.Count; j++)
                {
                    if (j != 0)
                    {
                        pList[j].Source.Mol?.Dispose();
                    }
                    else
                    {
                        source = pList[j].Source;
                        pList[j].Source.Mol?.Dispose();
                    }
                }

                return new Transformation(left, right, rules, source!, _sourceIter, _stepIter, i);
            })
            .ToList();

    private IEnumerable<SideCompound> ExtractLeft(List<Transformation> transformations)
    {
        var extractedLeft = transformations
            .Select(x => (x.TransformationId, x.Left))
            .Extract();
        
        foreach (var s in extractedLeft)
        {
            if (s.Mol is null) continue;

            var standardised = _standardiser.Standardise(s.Mol);

            if (standardised.StandardiseFailed || standardised.Mol is null)
            {
                standardised.Dispose();
                s.Mol?.Dispose();
                continue;
            }

            yield return s with {Mol = standardised.Mol};
        }
    }

    private ExtractRightResult ExtractRight(List<Transformation> transformations)
    {
        var extractedRight = transformations
            .Select(x => (x.TransformationId, x.Right))
            .Extract();
        var badTransformations = new HashSet<string>();
        var res = new List<SideCompound>();

        foreach (var s in extractedRight)
        {
            if (s.Mol is null) continue;

            var standardised = _standardiser.Standardise(s.Mol);

            if (standardised.StandardiseFailed || standardised.Mol is null)
            {
                standardised.Dispose();
                s.Mol?.Dispose();

                foreach (var t in s.TransformationIds)
                {
                    badTransformations.Add(t);
                }
                
                continue;
            }

            res.Add(s with {Mol = standardised.Mol});
        }

        return new(res, badTransformations);
    }

    public void Dispose()
    {
        _standardiser.Dispose();
    }
}