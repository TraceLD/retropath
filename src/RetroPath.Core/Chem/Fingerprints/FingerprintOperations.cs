using RetroPath.Core.Extensions;

namespace RetroPath.Core.Chem.Fingerprints;

public static class FingerprintOperations
{
    /// <summary>
    /// Looks for a potential substructure match using fingerprints.
    ///
    /// A potential SSS(A,B) match is found if OBC(FP(A)) &lt;= OBC(FP(B)) AND OBC(FP(A) &amp; FB(B)) == OBC(FP(A)),
    /// where A is the rule and B is the source molecule.
    /// </summary>
    /// <param name="molFingerprint">Molecule <see cref="Fingerprint"/>.</param>
    /// <param name="queryFingerprint">Query <see cref="Fingerprint"/>.</param>
    /// <returns>Whether the <see cref="molFingerprint"/> is a potential match for the <see cref="queryFingerprint"/>.</returns>
    ///
    /// <remarks>
    /// The performance improvement vs deep check is particularly big if there are many queries (usually rules),
    /// which will be most of the time as retropath is usually ran on databases of 200k+ reaction rules.
    ///
    /// Based on RDKit KNIME implementation of substructure filter: <see href="https://github.com/rdkit/knime-rdkit/blob/5fe11f9c021ca9b21d36d6231db62d943eb50eaa/org.rdkit.knime.nodes/src/org/rdkit/knime/nodes/moleculesubstructfilter/RDKitMoleculeSubstructFilterNodeModel.java#L484"/>
    /// </remarks>
    public static bool IsPotentialMatch(Fingerprint molFingerprint, Fingerprint queryFingerprint)
        => queryFingerprint.Cardinality <= molFingerprint.Cardinality &&
           queryFingerprint.Bits.NewAnd(molFingerprint.Bits).FastGetCardinality() == queryFingerprint.Cardinality;
}