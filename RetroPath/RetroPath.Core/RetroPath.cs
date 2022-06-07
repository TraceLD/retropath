using RetroPath.Core.Exceptions;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using Serilog;

namespace RetroPath.Core;

public class RetroPath : IDisposable
{
    private readonly InputConfiguration _inputConfiguration;
    private readonly OutputConfiguration _outputConfiguration;

    private readonly RulesParser _rulesParser;
    private readonly CompoundParser _compoundParser;

    private List<IGrouping<int, ReactionRule>>? _rules;
    private List<ChemicalCompound>? _sourcesInSink;
    private List<ChemicalCompound>? _sourcesNotInSink;
    private Dictionary<string, ChemicalCompound>? _sourcesAndSinks;

    public RetroPath(InputConfiguration inputConfiguration, OutputConfiguration outputConfiguration)
    {
        _inputConfiguration = inputConfiguration;
        _outputConfiguration = outputConfiguration;
        _rulesParser = new(_inputConfiguration);
        _compoundParser = new(_inputConfiguration);
    }

    public async Task ParseInputs()
    {
        Log.Information("Parsing inputs...");
        
        await Task.Run(ParseRules);
        await ParseSourcesAndSinks();
        
        Log.Information("Sinks + sources: {SpsCount}", _sourcesAndSinks!.Count);
        Log.Information("Sources in sink: {SisCount}", _sourcesInSink!.Count);
        Log.Information("Sources NOT in sink: {SnisCount}", _sourcesNotInSink!.Count);
    }

    public void PrepareOutputDir()
    {
        if (!Directory.Exists(_outputConfiguration.OutputDir))
        {
            Directory.CreateDirectory(_outputConfiguration.OutputDir);
        }
    }

    public List<GlobalResult> Compute()
    {
        if (_rules is null || _sourcesInSink is null || _sourcesNotInSink is null || _sourcesAndSinks is null)
        {
            throw new InputsNotParsedException(
                $"You must parse the inputs using the ${nameof(ParseRules)} method before running RP via ${nameof(Compute)}.");
        }
        
        var rpLoop = new PathwayLoop(_inputConfiguration, 0, _rules, _sourcesInSink,
            _sourcesNotInSink, _sourcesAndSinks);
        
        return rpLoop.Run();
    }

    private void ParseRules()
    {
        var rawRules = _rulesParser.Parse(_inputConfiguration.RulesFilePath);
        
        Log.Information("Rules: {RulesCount}", rawRules.Count);
        
        var rules = rawRules
            .AsParallel()
            .GroupBy(r => r.Diameter)
            .OrderByDescending(r => r.Key)
            .ToList();

        _rules = rules;
    }

    private async Task ParseSourcesAndSinks()
    {
        var parsedSinks =
            await Task.Run(() => _compoundParser.Parse(_inputConfiguration.SinkFilePath, ChemicalType.Sink));
        var parsedSources =
            await Task.Run(() => _compoundParser.Parse(_inputConfiguration.SourceFilePath, ChemicalType.Source));

        var (sourcesInSink, sourcesNotInSink, sourcesAndSinks) =
            SinkAndSourceDivider.Divide(parsedSources, parsedSinks);

        _sourcesInSink = sourcesInSink;
        _sourcesNotInSink = sourcesNotInSink;
        _sourcesAndSinks = sourcesAndSinks;
    }

    public void Dispose() => _compoundParser?.Dispose();
}