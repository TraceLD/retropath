using RetroPath.Chem;

namespace RetroPath.Core.Parsers;

public record DividedSinkSource(
    List<ChemicalCompound> SourcesInSink,
    List<ChemicalCompound> SourcesNotInSink,
    Dictionary<string, ChemicalCompound> SourcesAndSinks
);