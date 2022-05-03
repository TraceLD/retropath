using System.Collections.Concurrent;

namespace RetroPath.Core.Models;

public class GeneratedProductsParser
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
    
    private readonly ConcurrentBag<GeneratedProduct> _generatedProducts;
    private readonly int _sourceIter;
    private readonly int _stepIter;

    public GeneratedProductsParser(ConcurrentBag<GeneratedProduct> generatedProducts, int sourceIter, int stepIter)
    {
        _generatedProducts = generatedProducts;
        _sourceIter = sourceIter;
        _stepIter = stepIter;
    }

    public void Parse()
    {
        var transformations = _generatedProducts
            .GroupBy(p => (string.Join(".", p.Left), string.Join(".", p.Right)))
            .Select((products, i) =>
            {
                var first = products.First();
                var rules = new Dictionary<string, ReactionRule>();
                var left = first.Left;
                var right = first.Right;
                //var source = first.Source;
                var allRules = products.Select(x => x.Rule);

                foreach (var r in allRules)
                {
                    var foldedId = r.GetFoldedRuleId();

                    if (!rules.ContainsKey(foldedId))
                    {
                        rules.Add(foldedId, r);
                    }
                }

                return new Transformation(left, right, rules, null, _sourceIter, _stepIter, i);
            })
            .ToList();

        throw new NotImplementedException();
    }
}