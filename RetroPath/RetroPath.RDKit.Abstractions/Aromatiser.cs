using GraphMolWrap;

namespace RetroPath.RDKit.Abstractions;

public class Aromatiser
{
    public RWMol? Aromatise(RWMol mol, bool rescueKek)
    {
        var temp = new RWMol(mol);
        
        if (RDKFuncs.setAromaticity(temp) != 0)
        {
            RDKFuncs.adjustHs(temp);
            return temp;
        }

        if (!rescueKek)
        {
            return null;
        }
        
        mol.Kekulize();
        return mol;
    }
}