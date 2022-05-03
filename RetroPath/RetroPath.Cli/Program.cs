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
    4
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

foreach (var source in sourcesNotInSink)
{
    var rulesFirer = new RulesFirer(source.Value, new(), parsedRules);
    var generatedProducts = rulesFirer.FireRules();
    var gpParser = new GeneratedProductsParser(generatedProducts);
}
