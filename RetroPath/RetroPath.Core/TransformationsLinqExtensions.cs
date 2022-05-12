using GraphMolWrap;

namespace RetroPath.Core;

internal static class TransformationsLinqExtensions
{
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

    /// <summary>
    /// Filters out transformations that are "bad" (i.e. contain products that can't be sanitised).
    /// </summary>
    /// <param name="e"><see cref="IEnumerable{T}"/> containing generated products</param>
    /// <param name="badTransformations"><see cref="HashSet{T}"/> containing bad transformations</param>
    /// <returns>Filtered products.</returns>
    internal static IEnumerable<GeneratedProductsParser.SideCompound> FilterOutBadTransformations(this IEnumerable<GeneratedProductsParser.SideCompound> e, HashSet<string> badTransformations)
    {
        // TODO: optimise this code;
        var cs = e.ToList();

        var filtered = new List<GeneratedProductsParser.SideCompound>();
        foreach (var compound in cs)
        {
            var hasBadTransformation = false;
            foreach (var transformationId in compound.TransformationIds)
            {
                if (badTransformations.Contains(transformationId))
                {
                    hasBadTransformation = true;
                }
            }

            if (!hasBadTransformation)
            {
                filtered.Add(compound);
            }
        }

        return filtered;
    }
}