using GraphMolWrap;

namespace RetroPath.Core.Models;

public record ChemicalCompound(
    HashSet<string> Names,
    string Inchi,
    string Smiles,
    RWMol? Mol
) : IDisposable
{
    public void Dispose() => Mol?.Dispose();
}

public record ParsedGeneratedCompound(
    List<string> TransformationIds,
    string Inchi,
    string? LiteInchi,
    string Smiles,
    RWMol? Mol,
    List<GeneratedProductsParser.TransformationIdCoeff> TransformationIdCoeff
) : IDisposable
{
    public void Dispose() => Mol?.Dispose();
}
