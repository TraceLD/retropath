using BenchmarkDotNet.Attributes;
using RetroPath.Core;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;

namespace RetroPath.Benchmarks;

[MemoryDiagnoser]
public class RuleEngineBenchmark
{
    private RuleEngine? _ruleEngine;

    [GlobalSetup]
    public void Setup()
    {
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
        using var compoundParser = new CompoundParser(inputConfig);
        var source = compoundParser.Parse(inputConfig.SourceFilePath, ChemicalType.Source).First().Value;
        var rulesParser = new RulesParser(inputConfig);
        var parsedRules = rulesParser.Parse(inputConfig.RulesFilePath);
        var groupedRules = parsedRules
            .AsParallel()
            .GroupBy(r => r.Diameter)
            .OrderByDescending(r => r.Key)
            .ToList();

        _ruleEngine = new RuleEngine(new[] {source}.ToList(), new(), groupedRules);
    }
    
    [Benchmark]
    public void FireRules() => _ruleEngine!.FireRules();
}