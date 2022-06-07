using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using Serilog;

namespace RetroPath.Core;

public class PathwayLoop : IRetroPathLoop<List<GlobalResult>>
{
    private readonly InputConfiguration _inputConfiguration;
    private readonly int _pathwayLength;
    private readonly int _iOuter;
    private readonly List<IGrouping<int, ReactionRule>> _rules;
    private readonly List<GlobalResult> _results;

    private List<ChemicalCompound> _iSourcesInSink;
    private List<ChemicalCompound> _iSourcesNotInSink;
    private Dictionary<string, ChemicalCompound> _iSourcesAndSinks;
    
    public int I { get; private set; }
    
    public PathwayLoop(
        InputConfiguration inputConfiguration,
        int iOuter,
        List<IGrouping<int, ReactionRule>> rules,
        List<ChemicalCompound> starterSourcesInSink,
        List<ChemicalCompound> starterSourcesNotInSink,
        Dictionary<string, ChemicalCompound> starterSourcesAndSinks
    )
    {
        _inputConfiguration = inputConfiguration;
        _pathwayLength = inputConfiguration.PathwayLength;
        _iOuter = iOuter;
        _rules = rules;
        _results = new();

        _iSourcesInSink = starterSourcesInSink;
        _iSourcesNotInSink = starterSourcesNotInSink;
        _iSourcesAndSinks = starterSourcesAndSinks;
        
        I = 0;
    }

    public List<GlobalResult> Run()
    {
        while (true)
        {
            if (I > _pathwayLength || _iSourcesAndSinks.Count < 1 || _iSourcesNotInSink.Count < 1)
            {
                return _results;
            }
            
            Log.Information("Iteration: {IInner} for source {ISource}, Number of sources to iterate through: {SourcesCount}", I, _iOuter, _iSourcesNotInSink.Count);
            
            RunIteration();
        }
    }

    public void RunIteration()
    {
        var iRulesFirer = new RulesFirer(_iSourcesNotInSink, new(), _rules);
        var iRes = iRulesFirer.FireRules();

        using var iGpParser = new GeneratedProductsParser(iRes, _iOuter, I);
        var iParsedProducts = iGpParser.Parse();

        var iUpdater = new SourceSinkUpdater(_iSourcesAndSinks, iParsedProducts.Right, iParsedProducts.TransformationInfos, _inputConfiguration);
        var (iNewSourcesInSink, iNewSink, iNewSources) = iUpdater.Update();

        // TODO: this needs extensive testing;
        // dispose sources that will no longer be needed;
        if (I != 0)
        {
            foreach (var s in _iSourcesNotInSink)
            {
                s.Dispose();
            }
        }
        else
        {
            _iSourcesNotInSink[_iOuter].Dispose();
        }
            
        var iBuilder = new GlobalResultsResultsBuilder(iParsedProducts.Left, iParsedProducts.Right, iParsedProducts.TransformationInfos, _iSourcesInSink);
        var iResults = iBuilder.Build().ToList();

        Log.Information("Generated {ResCount} results", iResults.Count);

        _results.AddRange(iResults);

        I++;
        _iSourcesInSink = iNewSourcesInSink;
        _iSourcesNotInSink = iNewSources;
        _iSourcesAndSinks = iNewSink;
    }
}