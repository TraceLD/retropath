using System.Collections;
using GraphMolWrap;
using RetroPath.Core.Extensions;

namespace RetroPath.Core.Models;

public class ChemicalCompound : IDisposable
{
    public HashSet<string> Names { get; }
    public string Inchi { get; }
    public string Smiles { get; }
    public RWMol? Mol { get; set; }
    public bool Initial { get; set; }
    public Fingerprint? Fingerprint { get; private set; }
    
    public ChemicalCompound(
        HashSet<string> names,
        string inchi,
        string smiles,
        RWMol? mol,
        bool initial
    )
    {
        Names = names;
        Inchi = inchi;
        Smiles = smiles;
        Mol = mol;
        Initial = initial;
    }
    
    public void CalculateFingerprint()
    {
        if (Fingerprint is not null)
        {
            return;
        }
        
        BitArray arrFp;
        using (var fp = RDKFuncs.PatternFingerprintMol(Mol, FingerprintSettings.PreProcessingPatternFingerprintSize))
        {
            arrFp = fp.ToBclBitArray();
        }
        
        var cardinality = arrFp.FastGetCardinality();

        Fingerprint = new(arrFp, cardinality);
    }
    
    public void Dispose() => Mol?.Dispose();
}