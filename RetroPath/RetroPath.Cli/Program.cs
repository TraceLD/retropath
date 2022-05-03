using GraphMolWrap;
using RetroPath.Core;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using Serilog;

var inputConfig = new InputConfiguration(
    @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
    @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\sink_B.csv",
    @"C:\Users\Lukasz\Desktop\test.csv",
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


var a = 0;

var sources2 = new List<RWMol>
{
    RWMol.MolFromSmiles("[H]Oc1c([H])c(O[H])c(C(=O)C([H])=C([H])c2c([H])c([H])c([H])c([H])c2[H])c(O[H])c1[H]"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(O[H])c2c(c1[H])OC(O[H])(c1c([H])c([H])c([H])c([H])c1[H])C([H])([H])C2=O"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(OC([H])([H])[H])c([H])c2c1C(=O)C([H])([H])C([H])(c1c([H])c([H])c([H])c([H])c1[H])O2"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(OC2([H])OC([H])(C([H])([H])O[H])C([H])(O[H])C([H])(O[H])C2([H])O[H])c([H])c2c1C(=O)C([H])([H])C([H])(c1c([H])c([H])c([H])c([H])c1[H])O2"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(O[H])c2c(=O)c([H])c(-c3c([H])c([H])c([H])c([H])c3[H])oc2c1[H]"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(O[H])c2c(c1[H])OC([H])(c1c([H])c([H])c([H])c([H])c1[H])C([H])(O[H])C2=O"),
    RWMol.MolFromSmiles("[H]Oc1c([H])c(OC2([H])OC([H])(C([H])([H])OC3([H])OC([H])(C([H])([H])[H])C([H])(O[H])C([H])(O[H])C3([H])O[H])C([H])(O[H])C([H])(O[H])C2([H])O[H])c([H])c2c1C(=O)C([H])([H])C([H])(c1c([H])c([H])c([H])c([H])c1[H])O2")
};

foreach (var f in sources2)
{
    RDKFuncs.addHs(f);
}

var sources3 = sources2.Select(s => new ChemicalCompound(new HashSet<string>(), "aaa", s.MolToSmiles(), s)).ToList();

var groupedRules = parsedRules
    .AsParallel()
    .GroupBy(r => r.Diameter)
    .OrderByDescending(r => r.Key)
    .ToList();

Log.Information("Firing rules...");

var rulesFirer = new RulesFirer(sources3, new(), groupedRules);
var res = rulesFirer.FireRules();

var gpParser = new GeneratedProductsParser(res, 0, 0);
gpParser.Parse();

Log.Information("Done firing rules...");

Console.WriteLine("Goodbye");
