using RetroPath.Core.Models;

namespace RetroPath.Core.Parsers;

public record DividedSinkSource(
    Dictionary<string, ChemicalCompound> SourcesInSink,
    Dictionary<string, ChemicalCompound> SourcesNotInSink,
    Dictionary<string, ChemicalCompound> SourcesAndSinks
);