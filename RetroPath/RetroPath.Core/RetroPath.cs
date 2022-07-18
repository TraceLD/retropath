using RetroPath.Core.Exceptions;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Models.Csv;
using RetroPath.Core.Output;
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

    private List<GlobalResult>? _globalResults;

    public RetroPath(InputConfiguration inputConfiguration, OutputConfiguration outputConfiguration)
    {
        _inputConfiguration = inputConfiguration;
        _outputConfiguration = outputConfiguration;
        _rulesParser = new(_inputConfiguration);
        _compoundParser = new(_inputConfiguration);
    }

    public async Task ParseInputsAsync()
    {
        Log.Information("Parsing inputs...");
        
        await Task.Run(() => ParseRules(true));
        await ParseSourcesAndSinksAsync(true);
        
        Log.Information("Sinks + sources: {SpsCount}", _sourcesAndSinks!.Count);
        Log.Information("Sources in sink: {SisCount}", _sourcesInSink!.Count);
        Log.Information("Sources NOT in sink: {SnisCount}", _sourcesNotInSink!.Count);
    }

    public void PrepareOutputDir()
    {
        if (!Directory.Exists(_outputConfiguration.OutputDir))
        {
            Directory.CreateDirectory(_outputConfiguration.OutputDir);
            
            return;
        }

        foreach (var file in Directory.GetFiles(_outputConfiguration.OutputDir))
        {
            File.Delete(file);
        }
    }

    public void Compute()
    {
        if (_rules is null || _sourcesInSink is null || _sourcesNotInSink is null || _sourcesAndSinks is null)
        {
            throw new InputsNotParsedException(
                $"You must parse the inputs using the ${nameof(ParseRules)} method before running RP via ${nameof(Compute)}.");
        }

        var rpPathwayLoop = new PathwayLoop(_inputConfiguration, 0, _rules, _sourcesInSink,
            _sourcesNotInSink, _sourcesAndSinks);
        
        _globalResults = rpPathwayLoop.Run();
        
        Log.Information("Generated {GlobalResultsCount} global results", _globalResults.Count);
    }
    
    public async Task WriteResultsToCsvAsync()
    {
        if (_globalResults is null)
        {
            throw new ResultsNotGeneratedException("Results need to be generated first.");
        }

        var csvGlobalResults = _globalResults.Select(x => x.GetCsvRepresentation());
        var globalResultsWriter = new CsvOutputWriter<CsvGlobalResult>(_outputConfiguration.OutputDir, "results.csv", csvGlobalResults);

        await globalResultsWriter.WriteAsync();

        // TODO: write scope results here;
    }

    private void ParseRules(bool calculateFingerprints) // TODO: this should be configurable via an option
    {
        var rawRules = _rulesParser.Parse(_inputConfiguration.RulesFilePath);

        if (calculateFingerprints)
        {
            Log.Information("Calculating rule fingerprints");
            
            Parallel.ForEach(rawRules, rule => { rule.CalculateLeftFingerprint(); });
            
            Log.Information("Calculated rule fingerprints");
        }
        
        Log.Information("Rules: {RulesCount}", rawRules.Count);
        
        var rules = rawRules
            .AsParallel()
            .GroupBy(r => r.Diameter)
            .OrderByDescending(r => r.Key)
            .ToList();

        _rules = rules;
    }

    private async Task ParseSourcesAndSinksAsync(bool calculateSourceFingerprints) // TODO: this should be configurable via an option
    {
        var parsedSinks =
            await Task.Run(() => _compoundParser.Parse(_inputConfiguration.SinkFilePath, ChemicalType.Sink));
        var parsedSources =
            await Task.Run(() => _compoundParser.Parse(_inputConfiguration.SourceFilePath, ChemicalType.Source));

        if (calculateSourceFingerprints)
        {
            Log.Information("Calculating source fingerprints");
            
            // no point in using parallel as there's usually only a few sources max;
            // would actually be slower in parallel;
            foreach (var (_, source) in parsedSources)
            {
                source.CalculateFingerprint();
            }
            
            Log.Information("Calculated source fingerprints");
        }

        var (sourcesInSink, sourcesNotInSink, sourcesAndSinks) =
            SinkAndSourceDivider.Divide(parsedSources, parsedSinks);

        _sourcesInSink = sourcesInSink;
        _sourcesNotInSink = sourcesNotInSink;
        _sourcesAndSinks = sourcesAndSinks;
    }

    public void Dispose() => _compoundParser?.Dispose();
}