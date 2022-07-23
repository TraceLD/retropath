using System.Collections;
using GraphMolWrap;
using RetroPath.Core.Extensions;

namespace RetroPath.Core.Chem.Fingerprints;

public class Fingerprint
{
    public BitArray Bits { get; }
    public int Cardinality { get; }

    private Fingerprint(BitArray bits, int cardinality)
    {
        Bits = bits;
        Cardinality = cardinality;
    }

    /// <summary>
    /// Calculates a pre-processing pattern fingerprint for a <see cref="RWMol"/>.
    /// </summary>
    /// <param name="mol">RDKit molecule to calculate the fingerprint for.</param>
    /// <returns>The calculated fingerprint.</returns>
    public static Fingerprint CalcPatternFingerprintForMol(RWMol mol)
    {
        BitArray bits;
        using (var fp = RDKFuncs.PatternFingerprintMol(mol, FingerprintSettings.PreProcessingPatternFingerprintSize))
        {
            bits = fp.ToBclBitArray();
        }
        
        var cardinality = bits.FastGetCardinality();

        return new(bits, cardinality);
    }
}