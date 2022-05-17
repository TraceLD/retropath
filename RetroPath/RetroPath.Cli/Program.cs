using GraphMolWrap;
using RetroPath.Core;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using Serilog;

var inputConfig = new InputConfiguration(
    @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
    @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\sink_B.csv",
    @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\source.csv",
    1000,
    "cofacts_file",
    1000,
    0,
    1000,
    4,
    100
);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Parsing rules...");

var rulesParser = new RulesParser(inputConfig);
var parsedRules = rulesParser.Parse(inputConfig.RulesFilePath);

Log.Information("Parsing sinks/cofactors/sources...");

using var compoundParser = new CompoundParser(inputConfig);
var parsedSinks = compoundParser.Parse(inputConfig.SinkFilePath, ChemicalType.Sink);
var parsedSources = compoundParser.Parse(inputConfig.SourceFilePath, ChemicalType.Source);

var (sourcesInSink, sourcesNotInSink, sourcesAndSinks) = SinkAndSourceDivider.Divide(parsedSources, parsedSinks);

Log.Information("Rules: {RulesCount}", parsedRules.Count);
Log.Information("Sinks + sources: {SpsCount}", sourcesAndSinks.Count);
Log.Information("Sources in sink: {SisCount}", sourcesInSink.Count);
Log.Information("Sources NOT in sink: {SnisCount}", sourcesNotInSink.Count);

// TODO: extract this into its own method;
var groupedRules = parsedRules
    .AsParallel()
    .GroupBy(r => r.Diameter)
    .OrderByDescending(r => r.Key)
    .ToList();

Log.Information("Firing rules...");

var rulesFirer = new RulesFirer(sourcesNotInSink, new(), groupedRules);
var res = rulesFirer.FireRules();

using var gpParser = new GeneratedProductsParser(res, 0, 0);
var parsedResults = gpParser.Parse();

var updater = new SourceSinkUpdater(sourcesAndSinks, parsedResults.Right, parsedResults.TransformationInfos, inputConfig);
var (sourceInSink, newSink, newSources) = updater.Update();

Log.Information("Done firing rules...");

Console.WriteLine("Goodbye");

// TODO: make the recursive loop a nice class;
// TODO: currently not using the loop for debug purposes (easier to test stuff without a recursive loop)
List<string> Recurse(int pathwayLength, int iOuter, int iInner, InputConfiguration inputConfig, List<ChemicalCompound> iSourcesInSink, List<ChemicalCompound> iSourcesNotInSink, Dictionary<string, ChemicalCompound> iSourcesAndSinks,List<IGrouping<int, ReactionRule>> rules, List<string> results)
{
    if (iInner > pathwayLength || iSourcesAndSinks.Count < 1 || iSourcesNotInSink.Count < 1)
    {
        return results;
    }
    
    var iRulesFirer = new RulesFirer(iSourcesNotInSink, new(), rules);
    var iRes = iRulesFirer.FireRules();

    using var iGpParser = new GeneratedProductsParser(iRes, iOuter, iInner);
    var iParsedProducts = iGpParser.Parse();

    var iUpdater = new SourceSinkUpdater(iSourcesAndSinks, iParsedProducts.Right, iParsedProducts.TransformationInfos,
        inputConfig);
    var (iNewSourcesInSink, iNewSink, iNewSources) = iUpdater.Update();

    var iNewInner = iInner + 1;

    return Recurse(pathwayLength, iOuter, iNewInner, inputConfig, iNewSourcesInSink, iNewSources, iNewSink, rules, results);
}
