namespace RetroPath.Core.Models;

public struct ChemicalCompound
{
    public HashSet<string> Names { get; set; }
    public string Inchi { get; set; }
    public string Smiles { get; set; }

    public ChemicalCompound(HashSet<string> names, string inchi, string smiles)
    {
        Names = names;
        Inchi = inchi;
        Smiles = smiles;
    }
}