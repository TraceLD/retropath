﻿namespace RetroPath.RDKit.Abstractions;

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

    /// <summary>
    /// Concats a sequence of SMILES strings into a single SMILES string.
    /// </summary>
    /// <param name="smilesEnum">Sequence of SMILES to concat.</param>
    /// <returns>Single SMILES string.</returns>
    ///
    /// <remarks>Uses "." to concat the SMILES strings (see: SMILES bonds description).</remarks>
    public static string ConcatSmiles(IEnumerable<string> smilesEnum) => string.Join(".", smilesEnum);
}