using RetroPath.Core.Models;
using RetroPath.Core.Scope.Preprocessing;

namespace RetroPath.Core.Scope;

public class ScopeCalculator
{
    private struct FireableTransformation
    {
        public bool Fireable { get; set; }
        public bool Visited { get; set; }
    }

    private struct ReachableCompound
    {
        public bool Reachable { get; set; }
        public int Sink { get; set; } // TODO: should this be a bool?
        public int FirstIteration { get; set; } // TODO: should this be a bool?
    }
    
    private readonly List<GlobalResult> _globalResults;
    private readonly List<ScopeCalculationSourceInformation> _sourceInfos;
    private readonly DataPreprocessor _preprocessor;

    private PreprocessedTables _tables;
    
    public ScopeCalculator(List<GlobalResult> globalResults, List<ScopeCalculationSourceInformation> sourceInfos)
    {
        _globalResults = globalResults;
        _sourceInfos = sourceInfos;

        _preprocessor = new(_globalResults);
    }

    public IEnumerable<object> Compute()
    {
        if (!_globalResults.Any())
        {
            return Enumerable.Empty<object>();
        }
        
        ComputeOverallScope();
        
        // TODO: this should be done for every _sourceInfos info;
        
        
        
        throw new NotImplementedException();
    }

    private void GetTransformationsFireableFromSource(ScopeCalculationSourceInformation sInfo)
    {
        var reachableCompounds = new Dictionary<string, ReachableCompound>(); // key == CID;
        
        // get all transformations fireable from source;
        // key == TrID;
        Dictionary<string, FireableTransformation> fireableTransformations = _tables.ReactionsTable 
            .Where(r => r.Fireable)
            .Join(
                _tables.FromSourcesTable.Where(s => s.SourceName == sInfo.Name),
                r => r.TransformationId,
                fs => fs.TransformationId,
                (rxn, _) => rxn.TransformationId
            )
            .Distinct()
            .ToDictionary(k => k, _ => new FireableTransformation { Fireable = true, Visited = false});
        
        // get all reachable compounds;
        var reachableViaLeft = _tables.HasLeftTable
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        var reachableViaRight = _tables.HasRightTable
            .Where(x => fireableTransformations.ContainsKey(x.TransformationId))
            .Select(x => x.CID)
            .ToHashSet();
        
        
    }

    private void ComputeOverallScope()
    {
        SetupTables();
        SetInitialSinks();
        ComputeOverallReachability();
    }

    private void ComputeOverallReachability()
    {
        var prediction = false;

        while (!prediction)
        {
            // how many reachable compounds are we starting with?
            var reachableStartCount = _tables.GetReachableCompoundsCount();
            
            _tables.UpdateFireableReactions();
            _tables.UpdateReachableCompounds();

            var newReachableCount = _tables.GetReachableCompoundsCount();

            prediction = reachableStartCount == newReachableCount;
        }
    }
    
    private void SetupTables() 
        => _tables = _preprocessor.PreprocessTables();

    private void SetInitialSinks()
    {
        var compounds = _tables.CompoundsTable;

        foreach (var c in compounds)
        {
            // mark as reachable if in sink;
            if (_tables.SinkTable.Contains(c.CID))
            {
                c.Reachable = true;
                c.Sink = 1; // TODO: should this be a bool?
            }
        }
    }
}