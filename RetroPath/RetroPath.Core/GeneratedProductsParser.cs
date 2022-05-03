using System.Collections.Concurrent;
using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions;

namespace RetroPath.Core;

public class GeneratedProductsParser : IDisposable
{
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

    internal record TransformationIdCoeff(string TransformationId, int Coeff);

    internal record SideCompound(string SideSmiles, RWMol? Mol, List<string> TransformationIds, List<TransformationIdCoeff> TransformationIdCoeff) 
        : IDisposable
    {
        public void Dispose()
        {
            Mol?.Dispose();
        }
    };

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

    public void Parse()
    {
        var transformations = GetTransformations();
        var extractedLeft = ExtractLeft(transformations);
        var (extractedRight, badTransformations) = ExtractRight(transformations);
        
        
        
        throw new NotImplementedException();
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

                foreach (var (t, _) in s.TransformationIdCoeff)
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