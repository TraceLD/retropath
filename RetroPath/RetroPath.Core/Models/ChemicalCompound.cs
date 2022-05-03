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
