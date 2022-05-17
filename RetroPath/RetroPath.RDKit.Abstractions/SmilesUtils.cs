namespace RetroPath.RDKit.Abstractions;

/// <summary>
/// Useful utility methods for working with SMILES strings.
/// </summary>
public static class SmilesUtils
{
    /// <summary>
    /// Checks whether a substrate is mono molecular.
    /// </summary>
    /// <param name="smiles">Substrate in the SMILES form.</param>
    /// <returns>Returns whether a substrate is mono molecular.</returns>
    public static bool IsMonomolecular(string smiles) => smiles.Split('.').Length == 1;
}