using RetroPath.Core.Models;

namespace RetroPath.Core.Parsers;

public record DividedSinkSource(
    List<ChemicalCompound> SourcesInSink,
    List<ChemicalCompound> SourcesNotInSink,
    List<ChemicalCompound> SourcesAndSinks
);