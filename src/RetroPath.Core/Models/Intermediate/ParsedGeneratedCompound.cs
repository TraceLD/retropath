using GraphMolWrap;
using RetroPath.Chem;

namespace RetroPath.Core.Models.Intermediate;

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

    public ChemicalCompound ToChemicalCompound(HashSet<string>? names = null, bool disposeMol = false)
    {
        var mol = Mol;
        if (disposeMol)
        {
            Dispose();
            mol = null;
        }

        if (LiteInchi is null)
        {
            throw new NullReferenceException(nameof(LiteInchi) + " cannot be null when converting to " +
                                             nameof(ChemicalCompound) + ".");
        }

        return new(names ?? new(), LiteInchi, Smiles, mol, false);
    }
}