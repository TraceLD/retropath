using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RetroPath.Core;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Output;
using Serilog;

namespace RetroPath.Cli;

[Command]
public class RetroPathCommand : ICommand
{
    private InputConfiguration? _inputConfiguration;
    private OutputConfiguration? _outputConfiguration;
    
    #region Parameters

    [CommandParameter(0, Name = "rules", Description = "Path to the CSV file defining reactions rules.")]
    public string RulesFilePath { get; init; } = null!;

    [CommandParameter(1, Name = "source", Description = "Path to the CSV file defining source compound(s).")]
    public string SourceFilePath { get; init; } = null!;

    [CommandParameter(2, Name = "sink", Description = "Path to the CSV file defining sink compound(s).")]
    public string SinkFilePath { get; init; } = null!;
    
    [CommandParameter(3, Name = "pathway-length", Description = "Number of iterations to use. Pathway length is an important parameter regarding combanory explosion. This value will impact execution time and memory consumption. We recommend to use pathway lengths lower than or equal to 3 on standard workstations.")]
    public int PathwayLength { get; init; }
    
    #endregion

    #region Options

    [CommandOption("cofacs", Description = "Path to the file defining cofactors. No cofactors are imported if this option is not set. Cofactors are only used for bi-substrate rules.")]
    public string? CofactorsFilePath { get; init; }
    
    [CommandOption("source-mw", Description = "Maximum molecular weight in daltons (Da) above which a source compound will be filtered out.")]
    public int SourceMw { get; init; } = 1000;
    
    [CommandOption("cofact-mw", Description = "Maximum molecular weight in daltons (Da) above which a cofactor compound will be filtered out.")]
    public int CofactorMw { get; init; } = 1000;

    [CommandOption("min-diameter", Description = "The minimum diameter allowed for rules.")]
    public int MinDiameter { get; init; } = 0;
    
    [CommandOption("max-diameter", Description = "The minimum diameter allowed for rules.")]
    public int MaxDiameter { get; init; } = 1000;

    [CommandOption("max-structures", Description = "Number of best structures to keep as substrates for a further iteration. This value is used at the end of a RetroSMARTS iteration -- in combination with the rule score -- in order to select best products that will be used for a further iteration.")]
    public int MaxStructures { get; init; } = 100;

    [CommandOption("output-dir", Description = "Path to the directory to which the results will be written. NOTE: Any previous results in this directory will be overriden. Defaults to \"./results\"")]
    public string OutputDir { get; init; } = Path.Combine(Directory.GetCurrentDirectory(), "results");

    #endregion

    // cannot be a ctor because the props aren't filled in by CliFx at that point;
    private void Init()
    {
        _inputConfiguration = new(RulesFilePath, SinkFilePath, SourceFilePath, SourceMw, CofactorsFilePath, CofactorMw,
            MinDiameter, MaxDiameter, PathwayLength, MaxStructures);
        
        _outputConfiguration = new(OutputDir);
    }
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        Init();
        
        LogInputConfig();
        LogOutputConfig();

        using var rp = new RetroPath.Core.RetroPath(_inputConfiguration!, _outputConfiguration!);
        
        rp.PrepareOutputDir();
        
        await rp.ParseInputsAsync();
        
        Log.Information("Running the RP algorithm...");
        
        rp.Compute();
        
        Log.Information("Writing the results to CSV...");
        
        await rp.WriteResultsToCsvAsync();
        
        Log.Information("Wrote all results to CSV");
    }

    private void LogInputConfig()
    {

        Log.Information(@"Running RetroPath with the following input configuration:
    Rules file: {RulesFile}
    Sink file: {SinkFile}
    Source file: {SourceFile}
    Source max weight: {SourceMw}
    Cofactors file: {CofactorsFile}
    Cofactor max weight: {CofactorMw}
    Min rule diameter: {MinRuleDiameter}
    Max rule diameter: {MaxRuleDiameter}
    Pathway length: {PathwayLength}
    Max structures: {MaxStructures}",
            _inputConfiguration!.RulesFilePath, _inputConfiguration.SinkFilePath, _inputConfiguration.SourceFilePath,
            _inputConfiguration.SourceMw, _inputConfiguration.CofactorsFilePath ?? "None",
            _inputConfiguration.CofactorMw, _inputConfiguration.MinDiameter, _inputConfiguration.MaxDiameter,
            _inputConfiguration.PathwayLength, _inputConfiguration.MaxStructures);

    }
    
    private void LogOutputConfig()
    {
        
        Log.Information(@"Running RetroPath with the following output configuration:
    Output directory: {OutputDir}",
            _outputConfiguration!.OutputDir);
        
    }
}