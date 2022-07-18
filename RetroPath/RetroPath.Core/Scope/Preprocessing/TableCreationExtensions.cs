using System.Collections.Immutable;
using RetroPath.Core.Models;

namespace RetroPath.Core.Scope.Preprocessing;

using GlobalResults = IEnumerable<GlobalResult>;
using GroupedGlobalResults = IEnumerable<DataPreprocessor.GroupedGlobalResult>;

using FromSourcesTable = IEnumerable<PreprocessedTables.FromSourcesTableElement>;
using ReactionsTable = IEnumerable<PreprocessedTables.ReactionsTableElement>;
using HasSideTable = IEnumerable<PreprocessedTables.HasSideTableElement>;
using CompoundsTable = IEnumerable<PreprocessedTables.CompoundsTableElement>;

public static class TableCreationExtensions
{
    public static PreprocessedTables CreateTables(
        this GroupedGlobalResults results,
        (IEnumerable<DataPreprocessor.ParsedReaction> Left, IEnumerable<DataPreprocessor.ParsedReaction> Right) parsedReactions,
        GlobalResults rawResults
    )
    {
        var resultsList = results.ToList();
        
        // create all the tables;
        var fromSources = resultsList.CreateFromSourcesTable().ToList();
        var reactions = resultsList.CreateReactionsTable().ToList();
        var hasLeft = parsedReactions.CreateHasLeftTable().ToList();
        var hasRight = parsedReactions.CreateHasRightTable().ToList();
        var compounds = parsedReactions.CreateCompoundsTable().ToList();
        var sinks = rawResults.CreateSinkTable();

        return new(fromSources, reactions, hasLeft, hasRight, compounds, sinks);
    }

    private static FromSourcesTable CreateFromSourcesTable(this GroupedGlobalResults results)
        // ungrouped based on initial source;
        => from r in results
            from sourceName in r.InitialSource
            select new PreprocessedTables.FromSourcesTableElement(r.TransformationId, sourceName);

    private static ReactionsTable CreateReactionsTable(this GroupedGlobalResults results)
        => results.Select(x =>
            new PreprocessedTables.ReactionsTableElement(x.TransformationId, x.InitialSource) {Fireable = false});

    private static HasSideTable CreateHasLeftTable(this (IEnumerable<DataPreprocessor.ParsedReaction> Left, IEnumerable<DataPreprocessor.ParsedReaction> Right) parsedReactions)
        => parsedReactions.Left.Select(x => new PreprocessedTables.HasSideTableElement(x.TransformationId, x.Smiles));
    
    private static HasSideTable CreateHasRightTable(this (IEnumerable<DataPreprocessor.ParsedReaction> Left, IEnumerable<DataPreprocessor.ParsedReaction> Right) parsedReactions)
        => parsedReactions.Right.Select(x => new PreprocessedTables.HasSideTableElement(x.TransformationId, x.Smiles));

    private static CompoundsTable CreateCompoundsTable(this (IEnumerable<DataPreprocessor.ParsedReaction> Left, IEnumerable<DataPreprocessor.ParsedReaction> Right) parsedReactions)
        => parsedReactions.Left
            .Union(parsedReactions.Right) // gets all involved compounds;
            .GroupBy(x => x.Smiles)
            .Select(x 
                => new PreprocessedTables.CompoundsTableElement("NA", x.Key, x.Min(y => y.Iteration))
                {
                    Reachable = false,
                    Sink = 0
                }
            );

    private static ImmutableHashSet<string> CreateSinkTable(this GlobalResults globalResults)
        => globalResults
            .GroupBy(gr => gr.ProductSmiles)
            .Select(grouping =>
            {
                // returns true if any element with that SMILES is in sink;
                var inSink = grouping.Any(x => x.InSink);

                return (Smiles: grouping.Key, InSink: inSink);
            })
            .Where(x => x.InSink)
            .Select(x => x.Smiles)
            .ToImmutableHashSet();
}