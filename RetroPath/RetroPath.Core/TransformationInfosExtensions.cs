using RetroPath.Core.Models;
using RetroPath.RDKit.Abstractions;

namespace RetroPath.Core;

public static class TransformationInfosExtensions
{
    public static Dictionary<string, string> Aggregate(this IEnumerable<(string Smiles, string TransformationId, int Coeff)> ungroupedCompounds)
        => ungroupedCompounds
            .Select(x =>
            {
                var newSmiles = x.Smiles;
                for (var i = 0; i < x.Coeff; i++)
                {
                    if (i != 0)
                    {
                        newSmiles = newSmiles + "." + x.Smiles;
                    }
                }

                x.Smiles = newSmiles;

                return x;
            })
            .GroupBy(x => x.TransformationId)
            .ToDictionary(x => x.Key, x => SmilesUtils.ConcatSmiles(x.Select(y => y.Smiles)));

    public static IEnumerable<(string Smiles, string TransformationId, int Coeff)> UngroupTransformations(
        this IEnumerable<ParsedGeneratedCompound> generatedCompounds)
    {
        foreach (var compound in generatedCompounds)
        {
            foreach (var (transformationId, coeff) in compound.TransformationIdCoeff)
            {
                yield return (compound.Smiles, transformationId, coeff);
            }
        }
    }
}