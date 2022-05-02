using GraphMolWrap;

namespace RetroPath.Core.Models;

public record ChemicalCompound(
    HashSet<string> Names,
    string Inchi,
    string Smiles,
    RWMol? Mol
): IDisposable
{
    public void Dispose() => Mol?.Dispose();
};

public record ChemicalCompoundWithoutMol(
    HashSet<string> Names,
    string Inchi,
    string Smiles
)
{
    public ChemicalCompoundWithoutMol(ChemicalCompound c) : this(c.Names, c.Inchi, c.Smiles)
    {
    }
};

public record StandardisedCompound(
    string Name,
    string Smiles,
    string Inchi,
    RWMol? Mol
) : IDisposable
{
    public void Dispose() => Mol?.Dispose();
}