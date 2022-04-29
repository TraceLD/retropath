namespace RetroPath.Core.Models.Configuration;

public record InputConfiguration(
    string RulesFilePath,
    string SinkFilePath,
    string SourceFilePath,
    double SourceMw,
    int MinDiameter,
    int MaxDiameter,
    int PathwayLength
);