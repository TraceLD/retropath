using CsvHelper.Configuration.Attributes;

namespace RetroPath.Core.Models.Csv;

public class CsvGlobalResult
{
    [Name("Initial source")]
    public string InitialSource { get; init; } = null!;

    [Name("Transformation ID")]
    public string TransformationId { get; init; } = null!;

    [Name("Reaction SMILES")]
    public string ReactionSmiles { get; init; } = null!;

    [Name("Substrate SMILES")]
    public string SubstrateSmiles { get; init; } = null!;

    [Name("Substrate InChI")]
    public string SubstrateInchi { get; init; } = null!;

    [Name("Product SMILES")]
    public string ProductSmiles { get; init; } = null!;
    
    [Name("Product InChI")]
    public string ProductInchi { get; init; } = null!;
    
    [Name("In Sink")]
    public int InSink { get; init; }

    [Name("Sink name")]
    public string SinkName { get; init; } = null!;
    
    [Name("Diameter")]
    public int Diameter { get; init; }

    [Name("Rule ID")]
    public string RuleId { get; init; } = null!;

    [Name("EC number")]
    public string EcNumber { get; init; } = null!;
    
    [Name("Score")]
    public double Score { get; init; }
    
    [Name("Iteration")]
    public int Iteration { get; init; }
}