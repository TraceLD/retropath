using RetroPath.Chem;
using RetroPath.Core.Chem;
using RetroPath.Core.Models;

namespace RetroPath.Core.Parsers;

public record DividedSinkSource(
    List<ChemicalCompound> SourcesInSink,
    List<ChemicalCompound> SourcesNotInSink,
    Dictionary<string, ChemicalCompound> SourcesAndSinks
);