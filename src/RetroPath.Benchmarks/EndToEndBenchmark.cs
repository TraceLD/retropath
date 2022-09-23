using BenchmarkDotNet.Attributes;
using RetroPath.Core.Models.Configuration;

namespace RetroPath.Benchmarks;

public class EndToEndBenchmark
{
    private InputConfiguration? _inputConfig;
    private OutputConfiguration? _outputConfig;
    
    [GlobalSetup]
    public void Setup()
    {
        _inputConfig = new InputConfiguration(
            @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\benchmark_rules.csv",
            @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\sink_B.csv",
            @"C:\Users\Lukasz\Documents\projects\rp\RetroPath2.0\tutorial_data\pinocembrin\source.csv",
            1000,
            null,
            1000,
            0,
            1000,
            4,
            100
        );
        _outputConfig = new OutputConfiguration(Path.Combine(Directory.GetCurrentDirectory(), "results"));
    }

    [Benchmark]
    public async Task RunEntireRp()
    {
        using var rp = new Core.RetroPath(_inputConfig!, _outputConfig!);
        
        rp.PrepareOutputDir();
        await rp.ParseInputsAsync();
        rp.Compute();
        await rp.WriteResultsToCsvAsync();
    }
}