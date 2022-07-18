using GraphMolWrap;

namespace RetroPath.RDKit.Abstractions;

public sealed class SaltStripper : IDisposable
{
    private readonly List<RWMol> _saltsToRemove;

    public SaltStripper(IEnumerable<string> saltsToRemove)
    {
        _saltsToRemove = saltsToRemove
            .Select(s => RWMol.MolFromSmarts(s))
            .Where(m => m is not null)
            .ToList();
    }

    public RWMol Strip(RWMol molToStrip, bool keepOnlyLargest)
    {
        // no salts to strip, return mol unchanged;
        if (!_saltsToRemove.Any())
        {
            return molToStrip;
        }

        var molStripping = new RWMol(molToStrip);

        foreach (var s in _saltsToRemove)
        {
            using var frags = RDKFuncs.getMolFrags(molStripping);
            
            if (frags.Count > 1)
            {
                var molStripped = RDKFuncs.deleteSubstructs(molStripping, s, true);
                var molStrippedRw = new RWMol(molStripped);

                if (molStrippedRw.getNumAtoms() > 0)
                {
                    molStripping.Dispose();
                    molStripping = molStrippedRw;
                }
                else
                {
                    molStrippedRw.Dispose();
                }
            }
            else
            {
                break;
            }
        }

        if (keepOnlyLargest)
        {
            using var listFrags = RDKFuncs.getMolFrags(molStripping);

            if (listFrags is not null && listFrags.Count > 1)
            {
                var iMaxAtomCount = 0;

                foreach (var frag in listFrags)
                {
                    var iAtomCount = (int)frag.getNumAtoms();

                    if (iAtomCount > iMaxAtomCount)
                    {
                        iMaxAtomCount = iAtomCount;
                        molStripping.Dispose();
                        molStripping = new RWMol(frag);
                    }
                    
                    frag.Dispose();
                }
            }
        }

        RDKFuncs.sanitizeMol(molStripping);

        return molStripping;
    }

    public void Dispose()
    {
        foreach (var m in _saltsToRemove)
        {
            m?.Dispose();
        }
    }
}