namespace RetroPath.Core.Models.Configuration;

public record InputConfiguration(
    string RulesFilePath,
    string SinkFilePath,
    string SourceFilePath,
    double SourceMw,
    string? CofactorsFilePath,
    double CofactorMw,
    int MinDiameter,
    int MaxDiameter,
    int PathwayLength,
    int MaxStructures
);