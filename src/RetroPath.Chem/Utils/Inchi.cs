using GraphMolWrap;

namespace RetroPath.Chem.Utils;

public static class Inchi
{
    public static RWMol? InchiToMolSimple(string inchi, bool sanitise, bool removeHs)
    {
        RWMol? mol;
        using (var extra = new ExtraInchiReturnValues())
        {
            mol = RDKFuncs.InchiToMol(inchi, extra, sanitise, removeHs);
        }

        return mol;
    }

    public static string? MolToInchiSimple(ROMol mol)
    {
        string? inchi;
        using (var extra = new ExtraInchiReturnValues())
        {
            inchi = RDKFuncs.MolToInchi(mol, extra);
        }

        return inchi;
    }
}