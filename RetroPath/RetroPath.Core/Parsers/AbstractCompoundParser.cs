using CsvHelper.Configuration.Attributes;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;

namespace RetroPath.Core.Parsers;

public abstract class AbstractCompoundParser : Parser<List<ChemicalCompound>>
{
    private struct RawCompound
    {
        public string Name { get; set; }
        [Name("inchi")] public string Inchi { get; set; }
    }

    protected AbstractCompoundParser(InputConfiguration inputConfiguration) : base(inputConfiguration)
    {
    }

    public abstract override List<ChemicalCompound> Parse(string filePath);
    
    protected 
}