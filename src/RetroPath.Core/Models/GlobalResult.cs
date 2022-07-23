using RetroPath.Core.Extensions;
using RetroPath.Core.Models.Dto;

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
    double Score,
    int Iteration
)
{
    public GlobalResultDto GetCsvRepresentation()
        => new()
        {
            InitialSource = InitialSource.ToCsvString(),
            TransformationId = TransformationId,
            ReactionSmiles = ReactionSmiles,
            SubstrateSmiles = SubstrateSmiles,
            SubstrateInchi = SubstrateInchi,
            ProductSmiles = ProductSmiles,
            ProductInchi = ProductInchi,
            InSink = InSink ? 1 : 0,
            SinkName = SinkNames.ToCsvString(),
            Diameter = Diameter,
            RuleId = RuleId.ToCsvString(),
            EcNumber = EcNumber.ToCsvString(),
            Score = Score,
            Iteration = Iteration
        };
}
