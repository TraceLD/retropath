using BenchmarkDotNet.Attributes;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;

namespace RetroPath.Benchmarks;

[MemoryDiagnoser]
public class RuleParserBenchmark
{
    private readonly InputConfiguration _rpConfig;
    private readonly RulesParser _parser;

    public RuleParserBenchmark()
    {
        _rpConfig = new InputConfiguration(
            @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
            @"sink_file",
            "source_file",
            1000,
            "cofacts_file",
            1000,
            0,
            1000,
            4
        );
        _parser = new RulesParser(_rpConfig);
    }

    [Benchmark]
    public void ParseBigRulesFile() => _parser.Parse(_rpConfig.RulesFilePath);
}