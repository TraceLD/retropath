using GraphMolWrap;

namespace RetroPath.Core;

public static class Class1
{
    public static string Test()
    {
        using var mol = RWMol.MolFromSmiles("[13CH3]C");
        var str = mol.MolToSmiles();

        Console.WriteLine(RDKFuncs.calcExactMW(mol));

        return str;
    }
}