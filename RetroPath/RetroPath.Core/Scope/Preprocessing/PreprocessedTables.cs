using System.Collections.Immutable;
using RetroPath.Core.Models;

namespace RetroPath.Core.Scope.Preprocessing;

using FromSourcesTable = List<PreprocessedTables.FromSourcesTableElement>;
using ReactionsTable = List<PreprocessedTables.ReactionsTableElement>;
using HasSideTable = List<PreprocessedTables.HasSideTableElement>;
using CompoundsTable = List<PreprocessedTables.CompoundsTableElement>;

public record PreprocessedTables(
    FromSourcesTable FromSourcesTable,
    ReactionsTable ReactionsTable,
    HasSideTable HasLeftTable,
    HasSideTable HasRightTable,
    CompoundsTable CompoundsTable,
    ImmutableHashSet<string> SinkTable
)
{
    public record FromSourcesTableElement(string TransformationId, string SourceName);

    public record ReactionsTableElement(string TransformationId, HashSet<string> InitialSource)
    {
        public bool Fireable { get; set; }
    };

    public record HasSideTableElement(string TransformationId, string CID);

    public record CompoundsTableElement(string Name, string CID, int FirstIteration)
    {
        public bool Reachable { get; set; }
        public int Sink { get; set; }
    }
    
    public int GetReachableCompoundsCount() => CompoundsTable.Count(x => x.Reachable);

    public void UpdateFireableReactions()
    {
        var rxns = ReactionsTable
            .Where(x => HasRightTable
                .Join(CompoundsTable
                        .Where(c => !c.Reachable),
                    hr => hr.CID,
                    c => c.CID,
                    (hr, _) => hr.TransformationId)
                .Distinct()
                .Contains(x.TransformationId));

        foreach (var reaction in rxns)
        {
            reaction.Fireable = true;
        }
    }

    public void UpdateReachableCompounds()
    {
        var newReachableCompounds = CompoundsTable
            .Where(x => HasLeftTable
                .Join(ReactionsTable
                        .Where(c => c.Fireable),
                    hr => hr.TransformationId,
                    c => c.TransformationId,
                    (hl, _) => hl.CID)
                .Distinct()
                .Contains(x.CID));
        
        foreach (var compound in newReachableCompounds)
        {
            compound.Reachable = true;
        }
    }
}