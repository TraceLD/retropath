namespace RetroPath.RDKit.Abstractions;

/// <summary>
/// Useful extensions for working with SMILES.
/// </summary>
public static class SmilesExtensions
{
    /// <summary>
    /// Checks whether a substrate is mono molecular.
    /// </summary>
    /// <param name="smiles">Substrate in the SMILES form.</param>
    /// <returns>Returns whether a substrate is mono molecular.</returns>
    public static bool IsMonomolecular(string smiles) => smiles.Split('.').Length == 1;
}