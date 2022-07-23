using GraphMolWrap;

namespace RetroPath.Chem.Utils;

public static class LiteInchi
{
    public static string? ToLiteInchiExtended(ROMol mol)
    {
        var extra = new ExtraInchiReturnValues();
        var inchi = RDKFuncs.MolToInchi(mol, extra);
        extra.Dispose();

        if (inchi is null)
        {
            return null;
        }

        if (inchi.Contains("/q"))
        {
            inchi = inchi[..inchi.IndexOf("/q", StringComparison.Ordinal)];
        }
        else
        {
            if (inchi.Contains("/p"))
            {
                inchi = inchi[..inchi.IndexOf("/p", StringComparison.Ordinal)];
            }
            else
            {
                if (inchi.Contains("/b"))
                {
                    inchi = inchi[..inchi.IndexOf("/b", StringComparison.Ordinal)];
                }
                else
                {
                    if (inchi.Contains("/t"))
                    {
                        inchi = inchi[..inchi.IndexOf("/t", StringComparison.Ordinal)];
                    }
                }
            }
        }

        return inchi;
    }

    public static string? ToLiteInchi(ROMol mol)
    {
        var inchi = Inchi.MolToInchiSimple(mol);

        if (inchi is null)
        {
            return null;
        }

        if (inchi.Contains("/b"))
        {
            inchi = inchi[..inchi.IndexOf("/b", StringComparison.Ordinal)];
        }
        else
        {
            if (inchi.Contains("/t"))
            {
                inchi = inchi[..inchi.IndexOf("/t", StringComparison.Ordinal)];
            }
        }

        return inchi;
    }
}