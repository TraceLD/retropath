using GraphMolWrap;

namespace RetroPath.Chem;

public class Aromatiser
{
    public RWMol? Aromatise(RWMol mol, bool rescueKek)
    {
        var temp = new RWMol(mol); // TODO: I'm like 50% sure there is a memory leak here;
        
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