using GraphMolWrap;

namespace RetroPath.Core.Models;

public struct ChemicalCompound : IDisposable
{
    public HashSet<string> Names { get; set; }
    public string Inchi { get; set; }
    public string Smiles { get; set; }
    public RWMol? Mol { get; set; }

    public ChemicalCompound(HashSet<string> names, string inchi, string smiles)
    {
        Names = names;
        Inchi = inchi;
        Smiles = smiles;
        Mol = null;
    }

    public ChemicalCompound(HashSet<string> names, string inchi, string smiles, RWMol mol)
    {
        Names = names;
        Inchi = inchi;
        Smiles = smiles;
        Mol = mol;
    }
    
    public void Dispose()
    {
        Mol?.Dispose();
    }
}