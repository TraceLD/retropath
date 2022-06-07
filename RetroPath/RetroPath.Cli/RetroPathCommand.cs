using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using RetroPath.Core.Models.Configuration;
using Serilog;

namespace RetroPath.Cli;

[Command]
public class RetroPathCommand : ICommand
{
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
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var inputConfig = GetInputConfigurationObject();
        var outputConfig = GetOutputConfigurationObject();

        Log.Information("Writing results to: \"{ResultsDirPath}\"", outputConfig.OutputDir);
        
        using var rp = new RetroPath.Core.RetroPath(inputConfig, outputConfig);

        await rp.ParseInputs();
        var results = rp.Compute();
        
        Log.Information("Generated {GlobalResultsCount} global results", results.Count);
    }

    private InputConfiguration GetInputConfigurationObject()
        => new(RulesFilePath, SinkFilePath, SourceFilePath, SourceMw, CofactorsFilePath, CofactorMw, MinDiameter,
            MaxDiameter, PathwayLength, MaxStructures);

    private OutputConfiguration GetOutputConfigurationObject() => new(OutputDir);
}