using RetroPath.Core.Models.Intermediate.Scope;

namespace RetroPath.Core.Scope;

using SourcesLookupMap = List<SourcesLookupElement>;
using SideCompoundsLookupMap = List<SideLookupElement>;
using SinksLookupSet = HashSet<string>; // <CID>

using Reactions = List<Reaction>;
using AllCompounds = List<Compound>;

public class ScopeEngine
{
    private struct FireableTransformation
    {
        public bool Fireable { get; set; }
        public bool Visited { get; set; }
    }

    private class ReachableCompound
    {
        public string? Source { get; set; }
        
        public bool Reachable { get; set; }
        public int Sink { get; set; } // TODO: should this be a bool?
        public int FirstIteration { get; set; } // TODO: should this be a bool?
    }

    private readonly List<SourceInformation> _sourceInfos;
    private readonly PreProcessedData _data;
    
    public ScopeEngine(List<SourceInformation> sourceInfos, PreProcessedData data)
    {
        _sourceInfos = sourceInfos;
        _data = data;
    }

    public string Compute()
    {
        ComputeOverallScope();

        foreach (var sInfo in _sourceInfos)
        {
            GetTransformationsFireableFromSource(sInfo);
        }
        
        throw new NotImplementedException();
    }

    private void ComputeOverallScope()
        => ComputeOverallReachability();

    private void ComputeOverallReachability()
    {
        var prediction = false;

        while (!prediction)
        {
            // how many reachable compounds are we starting with?
            var reachableStartCount = _data.GetReachableCompoundsCount();
            
            _data.UpdateFireableReactions();
            _data.UpdateReachableCompounds();

            var newReachableCount = _data.GetReachableCompoundsCount();

            prediction = reachableStartCount == newReachableCount;
        }
    }

    private void GetTransformationsFireableFromSource(SourceInformation sInfo)
    {
        var reachableCompounds = new Dictionary<string, ReachableCompound>(); // key == CID;
        
        // get all transformations fireable from source;
        // key == TrID;
        Dictionary<string, FireableTransformation> fireableTransformations = _data.Reactions 
            .Where(r => r.Fireable)
            .Join(
                _data.SourcesLookup.Where(s => s.SourceName == sInfo.Name),
                r => r.TransformationId,
                fs => fs.TransformationId,
                (rxn, _) => rxn.TransformationId
            )
            .Distinct()
            .ToDictionary(k => k, _ => new FireableTransformation { Fireable = true, Visited = false});
        
        // get all reachable compounds;
        var reachable = _data.LeftLookup
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        var reachableViaRight = _data.RightLookup
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        
        reachable.UnionWith(reachableViaRight);
        
        
        
    }
}