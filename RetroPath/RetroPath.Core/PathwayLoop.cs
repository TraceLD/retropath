﻿using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Output;
using Serilog;

namespace RetroPath.Core;

public class PathwayLoop : IRetroPathLoop<List<GlobalResult>>
{
    private readonly InputConfiguration _inputConfiguration;
    private readonly OutputConfiguration _outputConfiguration;
    private readonly int _pathwayLength;
    private readonly int _iOuter;
    private readonly List<IGrouping<int, ReactionRule>> _rules;
    
    private readonly List<GlobalResult> _results;
    private List<GlobalResult>? _resultsToWrite;

    private List<ChemicalCompound> _iSourcesInSink;
    private List<ChemicalCompound> _iSourcesNotInSink;
    private Dictionary<string, ChemicalCompound> _iSourcesAndSinks;
    
    public int CurrentIteration { get; private set; }
    
    public PathwayLoop(
        InputConfiguration inputConfiguration,
        OutputConfiguration outputConfiguration,
        int iOuter,
        List<IGrouping<int, ReactionRule>> rules,
        List<ChemicalCompound> starterSourcesInSink,
        List<ChemicalCompound> starterSourcesNotInSink,
        Dictionary<string, ChemicalCompound> starterSourcesAndSinks
    )
    {
        _inputConfiguration = inputConfiguration;
        _outputConfiguration = outputConfiguration;
        _pathwayLength = inputConfiguration.PathwayLength;
        _iOuter = iOuter;
        _rules = rules;
        _results = new();

        _iSourcesInSink = starterSourcesInSink;
        _iSourcesNotInSink = starterSourcesNotInSink;
        _iSourcesAndSinks = starterSourcesAndSinks;
        
        CurrentIteration = 0;
    }

    public List<GlobalResult> Run()
    {
        while (true)
        {
            if (CurrentIteration > _pathwayLength || _iSourcesAndSinks.Count < 1 || _iSourcesNotInSink.Count < 1)
            {
                WriteResults();
                
                return _results;
            }
            
            Log.Information("Iteration: {IInner} for source {ISource}, Number of sources to iterate through: {SourcesCount}", CurrentIteration, _iOuter, _iSourcesNotInSink.Count);
            
            RunIteration();
        }
    }

    public void RunIteration()
    {
        var iRulesFirer = new RulesFirer(_iSourcesNotInSink, new(), _rules);
        var iRes = iRulesFirer.FireRules();

        using var iGpParser = new GeneratedProductsParser(iRes, _iOuter, CurrentIteration);
        var iParsedProducts = iGpParser.Parse();

        var iUpdater = new SourceSinkUpdater(_iSourcesAndSinks, iParsedProducts.Right, iParsedProducts.TransformationInfos, _inputConfiguration);
        var (iNewSourcesInSink, iNewSink, iNewSources) = iUpdater.Update();

        // TODO: this needs extensive testing;
        // dispose sources that will no longer be needed;
        if (CurrentIteration != 0)
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

        Log.Information("Added {ResCount} global results from iteration {IInner}", iResults.Count, CurrentIteration);

        _results.AddRange(iResults);
        _resultsToWrite = iResults;

        CurrentIteration++;
        _iSourcesInSink = iNewSourcesInSink;
        _iSourcesNotInSink = iNewSources;
        _iSourcesAndSinks = iNewSink;
    }

    private void WriteResults()
    {
        if (_resultsToWrite is null || !_resultsToWrite.Any())
        {
            return;
        }
        
        Log.Information("Saving global results to CSV");
        
        var globalResultsWriter =
            new CsvOutputWriter<GlobalResult>(_outputConfiguration.OutputDir, "global.csv", _resultsToWrite);
        
        globalResultsWriter.Write();
        
        Log.Information("Saved global results");
    }
}