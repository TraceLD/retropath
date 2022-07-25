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
    public GlobalResultDto ToCsvDto()
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

    public static GlobalResult FromCsvDto(GlobalResultDto dto)
        => new(
            dto.InitialSource.Substring(1, dto.InitialSource.Length-2).Split(",").ToHashSet(),
            dto.TransformationId,
            dto.ReactionSmiles,
            dto.SubstrateSmiles,
            dto.SubstrateInchi,
            dto.ProductSmiles,
            dto.ProductInchi,
            dto.InSink == 1,
            dto.SinkName.Substring(1, dto.SinkName.Length-2).Split(",").ToHashSet(),
            dto.Diameter,
            dto.RuleId.Substring(1, dto.RuleId.Length-2).Split(",").ToHashSet(),
            dto.EcNumber.Substring(1, dto.EcNumber.Length-2).Split(",").ToHashSet(),
            dto.Score,
            dto.Iteration
        );
}
