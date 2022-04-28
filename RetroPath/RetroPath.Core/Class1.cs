using GraphMolWrap;

namespace RetroPath.Core;

public static class Class1
{
    public static string Test()
    {
        using var mol = RWMol.MolFromSmiles("C(COC(=O)O)C(=O)O");
        var str = mol.MolToSmiles();

        return str;
    }
}