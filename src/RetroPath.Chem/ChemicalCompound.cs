using GraphMolWrap;
using RetroPath.Chem.Fingerprints;

namespace RetroPath.Chem;

public class ChemicalCompound : IDisposable
{
    private Fingerprint? _cachedFingerprint;
    
    public HashSet<string> Names { get; }
    public string Inchi { get; }
    public string Smiles { get; }
    public RWMol? Mol { get; set; }
    public bool Initial { get; set; }
    
    public bool HasCachedFingerprint => _cachedFingerprint is not null;

    public ChemicalCompound(
        HashSet<string> names,
        string inchi,
        string smiles,
        RWMol? mol,
        bool initial
    )
    {
        _cachedFingerprint = default;
        
        Names = names;
        Inchi = inchi;
        Smiles = smiles;
        Mol = mol;
        Initial = initial;
    }
    
    /// <summary>
    /// Explicitly calculates a <see cref="Fingerprint"/> for this chemical compound and caches it.
    ///
    /// If a fingerprint has already been calculated and cached this will re-calculate and override it.
    /// </summary>
    /// <returns>The calculated <see cref="Fingerprint"/>.</returns>
    public Fingerprint CalculateFingerprint()
    {
        if (Mol is null)
        {
            throw new Exception(
                "Cannot calculate fingerprints for ChemicalCompounds without an associated RDKit molecule.");
        }
        
        _cachedFingerprint = Fingerprint.CalcPatternFingerprintForMol(Mol);

        return _cachedFingerprint;
    }

    /// <summary>
    /// Retrieves the <see cref="Fingerprint"/> from cache if present, calculates, caches the result and returns it otherwise.
    /// </summary>
    /// <returns>The cached calculated <see cref="Fingerprint"/> if present, newly calculated <see cref="Fingerprint"/> otherwise.</returns>
    public Fingerprint GetFingerprint()
        => HasCachedFingerprint ? _cachedFingerprint! : CalculateFingerprint();

    public void Dispose()
        => Mol?.Dispose();
}