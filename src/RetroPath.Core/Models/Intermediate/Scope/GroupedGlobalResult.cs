namespace RetroPath.Core.Models.Intermediate.Scope;

/// <summary>
/// Grouped global result is obtained by grouping <see cref="GlobalResult"/> by Transformation ID.
/// </summary>
public record GroupedGlobalResult(string TransformationId, HashSet<string> SubstrateSmiles,
    HashSet<string> ProductSmiles, HashSet<string> InitialSource, int Iteration)
{
    public static IEnumerable<GroupedGlobalResult> FromGlobalResults(IEnumerable<GlobalResult> r)
        => r.AsParallel()
            .GroupBy(x => x.TransformationId)
            .Select(x =>
            {
                var transformationId = x.Key;
                var substrateSmiles = x.Select(y => y.SubstrateSmiles).ToHashSet();
                var productSmiles = x.Select(y => y.ProductSmiles).ToHashSet();
                var initialSource = x.First().InitialSource;
                var iteration = x.Min(y => y.Iteration);

                return new GroupedGlobalResult(transformationId, substrateSmiles, productSmiles, initialSource,
                    iteration);
            });
}