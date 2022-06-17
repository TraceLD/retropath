// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Text.Json;
using ConsoleApp1;
using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using RetroPath.RDKit.Abstractions;
using RetroPath.RDKit.Abstractions.Reactions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

var standard = new CompoundStandardiser();
var json = File.ReadAllText(@"./sources.json");
var sourcesRaw = JsonSerializer.Deserialize<List<RawSource>>(json);
var sources = sourcesRaw!
    .Select(x =>
    {
        var mol = Inchi.InchiToMolSimple(x.Inchi, true, false);
        var molS = standard.Standardise(mol!);
        var names = new HashSet<string>();

        var cc = new ChemicalCompound(names, x.Inchi, x.Smiles, molS.Mol, x.Initial);

        return cc;
    })
    .ToList();

var rpConfig = new InputConfiguration(
    @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
    @"sink_file",
    "source_file",
    1000,
    "cofacts_file",
    1000,
    0,
    1000,
    3,
    100
);

Log.Information("Parsing rules...");

var rulesParser = new RulesParser(rpConfig);
var rules = rulesParser
    .Parse(rpConfig.RulesFilePath)
    .AsParallel()
    .GroupBy(r => r.Diameter)
    .OrderByDescending(r => r.Key)
    .ToList();

Log.Information("Parsed rules");

Log.Information("Starting loop");

var generatedProducts = new ConcurrentBag<GeneratedProduct>();
foreach (var rulesGrouping in rules)
{
    //var results = new ConcurrentBag<(ChemicalCompound Source, ReactionRule Rule)>();

    Parallel.ForEach(rulesGrouping, rule =>
    {
        if (rule.IsMono())
        {
            var smartsLeft = rule.RuleSmarts.Split(">>")[0];

            if (smartsLeft.StartsWith("(") && smartsLeft.EndsWith(")"))
            {
                smartsLeft = smartsLeft[1..^1];
            }

            using (var smartsLeftMol = RWMol.MolFromSmarts(smartsLeft))
            {
                Parallel.ForEach(sources, source =>
                {
                    try
                    {
                        if (source.Mol!.hasSubstructMatch(smartsLeftMol))
                        {
                            //results.Add((source, rule));

                            using var rxn = new OneComponentReaction(rule.RuleSmarts, source.Mol);
                            var products = rxn.RunReaction();

                            foreach (var p in products)
                            {
                                var leftSplit = source.Smiles.Split('.').ToList();
                                var rightSplit = p.Split('.').ToList();
                                var gp = new GeneratedProduct(leftSplit, rightSplit, rule, source);

                                generatedProducts.Add(gp);
                            }
                        }
                    }
                    catch
                    {
                        // ignored;
                    }
                });
            }
        }
    });
    
    if (!generatedProducts.IsEmpty)
    {
        break;
    }
}

Log.Information("Finished loop");
Log.Information(generatedProducts.Count.ToString());