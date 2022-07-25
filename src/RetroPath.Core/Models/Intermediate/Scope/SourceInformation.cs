using RetroPath.Chem;

namespace RetroPath.Core.Models.Intermediate.Scope;

public record SourceInformation(string Name, string Smiles)
{
    public static IEnumerable<SourceInformation> FromSource(ChemicalCompound source)
        // ungroup sources by name;
        => source.Names.Select(sourceName => new SourceInformation(sourceName, source.Smiles));
}