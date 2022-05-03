using GraphMolWrap;

namespace RetroPath.Core;

internal static class TransformationsLinqExtensions
{
    private record UngroupedSide(GeneratedProductsParser.SideCompound)
    
    private static IEnumerable<(string TransformationId, string Side)> Ungroup(this IEnumerable<(string TransformationId, List<string> Side)> e)
        => from c in e from s in c.Side select (c.TransformationId, s);

    internal static IEnumerable<GeneratedProductsParser.SideCompound> Extract(
        this IEnumerable<(string TransformationId, List<string> Side)> e)
        => e
            .Ungroup()
            .GroupBy(x => (x.TransformationId, x.Side))
            .Select(x => new
            {
                x.Key.TransformationId,
                TransformationIdCoeff =
                    new GeneratedProductsParser.TransformationIdCoeff(x.Key.TransformationId, x.Count()),
                x.Key.Side
            })
            .GroupBy(x => x.Side)
            .Select(x =>
                new GeneratedProductsParser.SideCompound(x.Key, RWMol.MolFromSmiles(x.Key),
                    x.Select(tc => tc.TransformationId).ToList(),
                    x.Select(tc => tc.TransformationIdCoeff).ToList()));

    internal static IEnumerable<GeneratedProductsParser.SideCompound> FilterOutBadTransformations(this IEnumerable<GeneratedProductsParser.SideCompound> e, HashSet<string> badTransformations)
    {
        var cs = e.ToList();

        var ungrouped = new List<(string transformationId, GeneratedProductsParser.SideCompound)>();
        for (int i = 0; i < cs.Count; i++)
        {
            foreach (var transformationId in cs[i].TransformationIds)
            {
                
            }
        }
    }
}