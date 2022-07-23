namespace RetroPath.Core.Models.Intermediate;

public record TransformationInfo(
    string TransformationId,
    HashSet<string> RuleIds,
    int Diameter,
    HashSet<string> EcNumber,
    double Score,
    HashSet<string> InitialSource
);
