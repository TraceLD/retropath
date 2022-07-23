using GraphMolWrap;
using RetroPath.Chem.Extensions;
using RetroPath.Chem.Utils;

namespace RetroPath.Chem;

public class CompoundStandardiser : IDisposable
{
    public record StandardiseResult( // TODO: This should REALLY be a Result<T>;
        RWMol? Mol,
        bool StandardiseFailed
    ) : IDisposable
    {
        public void Dispose()
        {
            Mol?.Dispose();
        }
    }

    private readonly SaltStripper _saltStripper;

    public CompoundStandardiser()
    {
        var saltsToRemoveSmarts = new List<string>
        {
            "[Cl,Br,I]",
            "[Li,Na,K,Ca,Mg]",
            "[O,N]",
            "[N](=O)(O)O",
            "[P](=O)(O)(O)O",
            "[P](F)(F)(F)(F)(F)F",
            "[S](=O)(=O)(O)O"
        };
        _saltStripper = new SaltStripper(saltsToRemoveSmarts);
    }

    public StandardiseResult Standardise(RWMol mol)
    {
        string? liteInchi;
        using (var saltStripped = _saltStripper.Strip(mol, true))
        {
            liteInchi = LiteInchi.ToLiteInchi(saltStripped);
        }

        if (liteInchi is null)
        {
            return new(null, false);
        }
        
        var extra = new ExtraInchiReturnValues();
        using var stripped3d = RDKFuncs.InchiToMol(liteInchi, extra);
        extra.Dispose();

        if (stripped3d is null)
        {
            return new(null, false);
        }

        var standardisingSmarts = new[]
        {
            "[*&-:1]-[*&+:2]>>[*&-0:1]=[*&+0:2]",
            "[*&-&H0:1]>>[*&-0&H1:1]",
            "[*&-&H1:1]>>[*&-0&H2:1]",
            "[*&-&H2:1]>>[*&-0&H3:1]",
            "[*&+&H:1]>>[*&+0&H0:1]",
            "[*&+&H2:1]>>[*&+0&H1:1]",
            "[*&+&H3:1]>>[*&+0&H2:1]"
        };

        using var transformed = stripped3d.Transform(standardisingSmarts, 100);

        if (transformed is null)
        {
            return new(null, true);
        }
        
        var withHs = transformed.addHs(false, true);
        var withHsRw = new RWMol(withHs);
        withHs.Dispose();
        
        var aromatiser = new Aromatiser();
        using var aromatised = aromatiser.Aromatise(withHsRw, true);

        if (aromatised is null)
        {
            return new(null, true);
        }
        
        var withHs2 = aromatised.addHs(false, true);
        var withHs2Rw = new RWMol(withHs2);
        withHs2.Dispose();

        return new(withHs2Rw, false);
    }

    public void Dispose()
    {
        _saltStripper.Dispose();
    }
}