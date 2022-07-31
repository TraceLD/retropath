using RetroPath.Core.Models.Intermediate.Scope;

namespace RetroPath.Core.Scope;

using SourcesLookupMap = List<SourcesLookupElement>;
using SideCompoundsLookupMap = List<SideLookupElement>;
using SinksLookupSet = HashSet<string>; // <CID>

using Reactions = List<Reaction>;
using AllCompounds = List<Compound>;

public class ScopeEngine
{
    internal class FireableTransformation
    {
        public bool Fireable { get; set; }
        public bool Visited { get; set; }
    }

    private class ReachableCompound
    {
        public bool Source { get; set; }
        
        public bool Reachable { get; set; }
        public int Sink { get; set; } // TODO: should this be a bool?
        public int FirstIteration { get; set; }
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
        // key == TrID;
        var fireableTransformations = GetAllFireableTransformations(sInfo);
        
        // key == CID;
        var reachableCompounds = GetAllReachableCompounds(fireableTransformations, sInfo);

        // remove transformations with source in right side;
        {
            var frTrsWithRightSide = fireableTransformations
                .Join(
                    _data.RightLookup,
                    f => f.Key,
                    r => r.TransformationId,
                    (f, r) => (FrTr: f, RightCpd: r)
                )
                .Where(x => reachableCompounds.ContainsKey(x.RightCpd.CID) && reachableCompounds[x.RightCpd.CID].Source);

            foreach (var (fireableTransformation, _) in frTrsWithRightSide)
            {
                fireableTransformation.Value.Fireable = false;
            }
        }
        
        RefreshFirstIterationValues(reachableCompounds, fireableTransformations);
        
        
    }
    
    private void RefreshFirstIterationValues(
        Dictionary<string, ReachableCompound> reachableCompounds,
        Dictionary<string, FireableTransformation> fireableTransformations
    )
    {
        foreach (var (_, r) in reachableCompounds)
        {
            if (r.Source)
            {
                r.FirstIteration = -1;
            }
            else
            {
                r.FirstIteration = -2;
            }
        }

        foreach (var t in fireableTransformations)
        {
            t.Value.Visited = false;
        }

        var iterCounter = -1;

        while (true)
        {
            var nbToVisit = fireableTransformations.GetTransformationsNotVisistedCount();

            if (nbToVisit > 0)
            {
                var toProcess = new HashSet<string>();

                foreach (var reachableCompound in reachableCompounds)
                {
                    if (reachableCompound.Value.FirstIteration != iterCounter)
                    {
                        continue;
                    }
                    
                    var left = _data.LeftLookup.FirstOrDefault(x => x.CID == reachableCompound.Key);
                    if (left is null)
                    {
                        continue;
                    }

                    var isInFireable = fireableTransformations.TryGetValue(left.TransformationId, out var frTr);
                    if (!isInFireable)
                    {
                        continue;
                    }

                    if (frTr!.Fireable && !frTr.Visited)
                    {
                        toProcess.Add(left.TransformationId);
                    }
                }
                
                foreach (var reachableCompound in reachableCompounds)
                {
                    if (reachableCompound.Value.FirstIteration != -2)
                    {
                        continue;
                    }
                    
                    var right = _data.RightLookup.FirstOrDefault(x => x.CID == reachableCompound.Key);
                    if (right is null)
                    {
                        continue;
                    }
                
                    var isInFireable = fireableTransformations.TryGetValue(right.TransformationId, out var frTr);
                    if (!isInFireable)
                    {
                        continue;
                    }

                    if (!toProcess.Contains(right.TransformationId))
                    {
                        continue;
                    }

                    reachableCompound.Value.FirstIteration = iterCounter + 1;
                }

                foreach (var fr in fireableTransformations
                             .Where(fr => toProcess.Contains(fr.Key)))
                {
                    fr.Value.Visited = true;
                }
            }

            var newNbToVisit = fireableTransformations.GetTransformationsNotVisistedCount();
            
            iterCounter++;

            if (nbToVisit == newNbToVisit)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Gets all fireable transformations from source.
    /// </summary>
    /// <param name="sInfo">Source information.</param>
    /// <returns>All fireable transformations from source. Key == Transformation ID.</returns>
    private Dictionary<string, FireableTransformation> GetAllFireableTransformations(SourceInformation sInfo)
        => _data.Reactions
            .Where(r => r.Fireable)
            .Join(
                _data.SourcesLookup.Where(s => s.SourceName == sInfo.Name),
                r => r.TransformationId,
                fs => fs.TransformationId,
                (rxn, _) => rxn.TransformationId
            )
            .Distinct()
            .ToDictionary(k => k, _ => new FireableTransformation {Fireable = true, Visited = false});

    private Dictionary<string, ReachableCompound> GetAllReachableCompounds(IReadOnlyDictionary<string, FireableTransformation> fireableTransformations, SourceInformation sInfo)
    {
        // key == CID;
        var res = new Dictionary<string, ReachableCompound>();
        
        var reachable = _data.LeftLookup
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        var reachableViaRight = _data.RightLookup
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        
        reachable.UnionWith(reachableViaRight);

        foreach (var compound in _data.Compounds)
        {
            if (!reachable.Contains(compound.CID) || res.ContainsKey(compound.CID))
            {
                continue;
            }
            
            var rc = new ReachableCompound
            {
                Reachable = compound.Reachable,
                Source = compound.CID.Equals(sInfo.Smiles),
                Sink = compound.Sink,
                FirstIteration = compound.FirstIteration
            };
            
            res.Add(compound.CID, rc);
        }

        return res;
    }
}

internal static class FireableTransformationsExtensions
{
    internal static int GetTransformationsNotVisistedCount(this Dictionary<string, ScopeEngine.FireableTransformation> frs)
        => frs.Count(x => x.Value.Fireable && x.Value.Visited == false);
}