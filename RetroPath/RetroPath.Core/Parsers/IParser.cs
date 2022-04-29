using RetroPath.Core.Models.Configuration;

namespace RetroPath.Core.Parsers;

public abstract class Parser<T>
{
    protected InputConfiguration InputConfiguration { get; }
    
    public Parser(InputConfiguration inputConfiguration)
    {
        InputConfiguration = inputConfiguration;
    }

    public abstract T Parse(string filePath);
}
