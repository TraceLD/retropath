namespace RetroPath.Core.Models;

public record GlobalResult(
    HashSet<string> InitialSource,
    string TransformationId,
    string ReactionSmiles,
    string SubstrateSmiles,
    string SubstrateInchi,
    string ProductSmiles,
    string ProductInchi,
    bool InSink,
    HashSet<string> SinkNames,
    int Diameter,
    HashSet<string> RuleId,
    HashSet<string> EcNumber,
    double Score
);
