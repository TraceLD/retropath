namespace RetroPath.Core.Models;

public record TransformationInfo(
    string TransformationId,
    HashSet<string> RuleIds,
    int Diameter,
    HashSet<string> EcNumber,
    double Score,
    HashSet<string> InitialSource
);
