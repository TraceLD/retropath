using RetroPath.Core.Models;

namespace RetroPath.Core.Scope;

public record ScopeCalculationSourceInformation(string Name, string Smiles)
{
    public static IEnumerable<ScopeCalculationSourceInformation> FromSource(ChemicalCompound source)
        // ungroup sources;
        => source.Names.Select(sourceName => new ScopeCalculationSourceInformation(sourceName, source.Smiles));
}