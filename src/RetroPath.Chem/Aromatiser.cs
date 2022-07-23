using GraphMolWrap;

namespace RetroPath.Chem;

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

        temp.Dispose();

        if (!rescueKek)
        {
            return null;
        }
        
        mol.Kekulize();
        return mol;
    }
}