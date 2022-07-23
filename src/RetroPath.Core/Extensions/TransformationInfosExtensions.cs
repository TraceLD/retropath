using RetroPath.Chem.Utils;
using RetroPath.Core.Chem;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Intermediate;

namespace RetroPath.Core;

public static class TransformationInfosExtensions
{
    public static Dictionary<string, (string Smiles, string Inchi)> Aggregate(this IEnumerable<(string Smiles, string Inchi, string TransformationId, int Coeff)> ungroupedCompounds)
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
            .ToDictionary(x => x.Key, x => (SmilesUtils.ConcatSmiles(x.Select(y => y.Smiles)), x.First().Inchi));

    public static IEnumerable<(string Smiles, string Inchi, string TransformationId, int Coeff)> UngroupTransformations(
        this IEnumerable<ParsedGeneratedCompound> generatedCompounds)
    {
        foreach (var compound in generatedCompounds)
        {
            foreach (var (transformationId, coeff) in compound.TransformationIdCoeff)
            {
                var liteInchi = compound.LiteInchi ?? throw new NullReferenceException("LiteInchi cannot be null when ungrouping transformations.");
                
                yield return (compound.Smiles, liteInchi, transformationId, coeff);
            }
        }
    }
}