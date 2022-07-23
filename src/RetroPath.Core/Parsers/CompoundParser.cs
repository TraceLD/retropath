using System.Collections.Concurrent;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using GraphMolWrap;
using RetroPath.Chem;
using RetroPath.Chem.Utils;
using RetroPath.Core.Chem;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;

namespace RetroPath.Core.Parsers;

public enum ChemicalType
{
    Source,
    Sink,
    Cofactor
}

public class CompoundParser : IDisposable
{
    private struct RawCompound
    {
        public string Name { get; set; }
        [Name("InChI")] public string Inchi { get; set; }
    }
    
    private record StandardisedCompound(
        string Name,
        string Smiles,
        string Inchi,
        RWMol? Mol
    ) : IDisposable
    {
        public void Dispose() => Mol?.Dispose();
    }
    
    private readonly InputConfiguration _inputConfiguration;
    private readonly CompoundStandardiser _standardiser;

    public CompoundParser(InputConfiguration inputConfiguration, CompoundStandardiser standardiser)
    {
        _inputConfiguration = inputConfiguration;
        _standardiser = standardiser;
    }

    public CompoundParser(InputConfiguration inputConfiguration)
    {
        _inputConfiguration = inputConfiguration;
        _standardiser = new();
    }

    public Dictionary<string, ChemicalCompound> Parse(string filePath, ChemicalType cType)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var compounds = csv.GetRecords<RawCompound>();

        var standardisedCompounds = new ConcurrentBag<StandardisedCompound>();

        Parallel.ForEach(compounds, compound =>
        {
            var standardised = StandardiseRawCompound(compound, cType);
            
            if (standardised is not null) standardisedCompounds.Add(standardised);
        });

        var groupedBySmiles = standardisedCompounds.GroupBy(c => c.Smiles);
        var groupedByInchi = new Dictionary<string, ChemicalCompound>();

        foreach (var grouping in groupedBySmiles)
        {
            var names = new HashSet<string>();
            var groupingList = grouping.ToList();

            string? smiles = null;
            RWMol? mol = null;
            for (var i = 0; i < groupingList.Count; i++)
            {
                names.Add(groupingList[i].Name);
                
                if (i != 0)
                {
                    groupingList[i].Dispose();
                }
                else
                {
                    mol = groupingList[i].Mol;
                    smiles = groupingList[i].Smiles;
                }
            }
            
            var inchi = LiteInchi.ToLiteInchiExtended(mol!);

            if (inchi is null)
            {
                continue;
            }

            if (cType is ChemicalType.Sink)
            {
                mol?.Dispose();
                mol = null;
            }

            if (groupedByInchi.TryGetValue(inchi, out var val))
            {
                val.Names.UnionWith(names);
            }
            else
            {
                groupedByInchi.Add(inchi, new ChemicalCompound(names, inchi, smiles!, mol, true));
            }
        }

        return groupedByInchi;
    }

    private StandardisedCompound? StandardiseRawCompound(RawCompound compound, ChemicalType cType)
    {
        using var mol = Inchi.InchiToMolSimple(compound.Inchi, true, false);

        if (mol is null)
        {
            return null;
        }

        var standardised = _standardiser.Standardise(mol);
        if (standardised.Mol is null || standardised.StandardiseFailed)
        {
            return null;
        }

        var canonSmiles = SmilesUtils.MolToCanonicalSmiles(standardised.Mol);

        if (!SmilesUtils.IsMonomolecular(canonSmiles))
        {
            return null;
        }

        // no max mw setting for Sink;
        if (cType is ChemicalType.Sink)
        {
            return new(compound.Name, canonSmiles, compound.Inchi, standardised.Mol);
        }

        var mw = RDKFuncs.calcExactMW(mol);
        bool isWithin = cType is ChemicalType.Source
            ? mw <= _inputConfiguration.SourceMw
            : mw <= _inputConfiguration.CofactorMw;

        return isWithin
            ? new(compound.Name, canonSmiles, compound.Inchi, standardised.Mol)
            : null;
    }

    public void Dispose()
    {
        _standardiser.Dispose();
    }
}