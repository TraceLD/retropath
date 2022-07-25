using RetroPath.Core.Models;
using RetroPath.Core.Models.Intermediate.Scope;

namespace RetroPath.Core.Scope;

using SourcesLookupMap = List<SourcesLookupElement>;
using SideCompoundsLookupMap = List<SideLookupElement>;
using SinksLookupSet = HashSet<string>; // <CID>

using Reactions = List<Reaction>;
using AllCompounds = List<Compound>;

/// <summary>
/// Pre-processed data that will be used to calculate the scope.
/// </summary>
public class PreProcessedData
{
    public SourcesLookupMap SourcesLookup { get; private set; }
    public SinksLookupSet SinksLookup { get; private set; }
    
    public SideCompoundsLookupMap LeftLookup { get; private set; }
    public SideCompoundsLookupMap RightLookup { get; private set; }


    public Reactions Reactions { get; private set; }
    public AllCompounds Compounds { get; private set; }
    
    public void InitAndPopulate(IEnumerable<GlobalResult> globalResults)
    {
        var globalResultsList = globalResults.ToList();
        var groupedRes = GroupedGlobalResult
            .FromGlobalResults(globalResultsList)
            .ToList();
        var rxns = ParsedReactions.FromGroupedGlobalResults(groupedRes);

        SourcesLookup = (
                from r in groupedRes
                from sourceName in r.InitialSource
                select new SourcesLookupElement(r.TransformationId, sourceName)
            )
            .ToList();

        Reactions = groupedRes
            .Select(x => new Reaction(x.TransformationId, x.InitialSource) {Fireable = false})
            .ToList();

        LeftLookup = rxns.Left.Select(x => new SideLookupElement(x.TransformationId, x.Smiles)).ToList();
        RightLookup = rxns.Right.Select(x => new SideLookupElement(x.TransformationId, x.Smiles)).ToList();

        Compounds = rxns.Left
            .Union(rxns.Right) // gets all involved compounds;
            .GroupBy(x => x.Smiles)
            .Select(x
                => new Compound("NA", x.Key, x.Min(y => y.Iteration))
                {
                    Reachable = false,
                    Sink = 0
                })
            .ToList();
        
        SinksLookup = globalResultsList
            .GroupBy(gr => gr.ProductSmiles)
            .Select(grouping =>
            {
                // returns true if any element with that SMILES is in sink;
                var inSink = grouping.Any(x => x.InSink);

                return (Smiles: grouping.Key, InSink: inSink);
            })
            .Where(x => x.InSink)
            .Select(x => x.Smiles)
            .ToHashSet();
    }
    
    public int GetReachableCompoundsCount() => Compounds.Count(x => x.Reachable);
    
    public void UpdateFireableReactions()
    {
        var rxns = Reactions
            .Where(x => RightLookup
                .Join(Compounds
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
        var newReachableCompounds = Compounds
            .Where(x => LeftLookup
                .Join(Reactions
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

public record SourcesLookupElement(string TransformationId, string SourceName);
public record SideLookupElement(string TransformationId, string CID);

public record Reaction(string TransformationId, HashSet<string> InitialSource)
{
    public bool Fireable { get; set; }
}

public record Compound(string Name, string CID, int FirstIteration)
{
    public bool Reachable { get; set; }
    public int Sink { get; set; }
}