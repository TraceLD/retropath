using BenchmarkDotNet.Attributes;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;

namespace RetroPath.Benchmarks;

[MemoryDiagnoser]
public class CompoundParserBenchmark
{
    private InputConfiguration? _rpConfig;
    private CompoundParser? _parser;

    [GlobalSetup]
    public void Setup()
    {
        _rpConfig = new InputConfiguration(
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
        _parser = new CompoundParser(_rpConfig);
    }

    [Benchmark]
    public void ParseBigSinksFile() => _parser!.Parse(_rpConfig!.SinkFilePath, ChemicalType.Sink);

    [GlobalCleanup]
    public void Cleanup() => _parser?.Dispose();
}