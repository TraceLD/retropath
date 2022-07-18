using System.Collections;
using RetroPath.Core.Models;

namespace RetroPath.Core.Scope.Preprocessing;

public class DataPreprocessor
{
    public record GroupedGlobalResult(string TransformationId, HashSet<string> SubstrateSmiles,
        HashSet<string> ProductSmiles, HashSet<string> InitialSource, int Iteration);

    public record ParsedReaction(string TransformationId, string Smiles, int Iteration);
    
    private readonly List<GlobalResult> _globalResults;

    public DataPreprocessor(List<GlobalResult> globalResults)
        => _globalResults = globalResults;

    public PreprocessedTables PreprocessTables()
    {
        // group GlobalResults by TransformationId;
        var groupedResults = _globalResults
            .AsParallel()
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
            })
            .ToList();

        var parsedReactions = ParseReactions(groupedResults);
        var preProcessedTables = groupedResults.CreateTables(parsedReactions, _globalResults);

        return preProcessedTables;
    }

    private static (IEnumerable<ParsedReaction> Left, IEnumerable<ParsedReaction> Right) ParseReactions(
            List<GroupedGlobalResult> groupedResults)
    {
        var left = new List<ParsedReaction>();
        var right = new List<ParsedReaction>();

        foreach (var result in groupedResults)
        {
            left.AddRange(result.SubstrateSmiles.Select(s =>
                new ParsedReaction(result.TransformationId, s, result.Iteration)));

            right.AddRange(result.ProductSmiles.Select(s =>
                new ParsedReaction(result.TransformationId, s, result.Iteration)));
        }

        return (left, right);
    }
}