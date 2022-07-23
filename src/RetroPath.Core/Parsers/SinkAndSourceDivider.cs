using RetroPath.Chem;

namespace RetroPath.Core.Parsers;

public static class SinkAndSourceDivider
{
    public static DividedSinkSource Divide(Dictionary<string, ChemicalCompound> parsedSources, Dictionary<string, ChemicalCompound> parsedSinks)
    {
        var sourcesInSink = new Dictionary<string, ChemicalCompound>();
        var sourcesNotInSink = new Dictionary<string, ChemicalCompound>();
        foreach (var (inchi, compound) in parsedSources)
        {
            if (parsedSinks.ContainsKey(inchi))
            {
                sourcesInSink.Add(inchi, compound);
            }
            else
            {
                sourcesNotInSink.Add(inchi, compound);
            }
        }

        foreach (var (inchi, compound) in parsedSources)
        {
            var sinkCompound = new ChemicalCompound(compound.Names, compound.Inchi, compound.Smiles, null, false);
            parsedSinks.Add(inchi, sinkCompound);
        }

        return new(sourcesInSink.Values.ToList(), sourcesNotInSink.Values.ToList(), parsedSinks.Values.ToDictionary(x => x.Inchi));
    }
}