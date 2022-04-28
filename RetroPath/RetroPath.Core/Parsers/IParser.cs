using RetroPath.Core.Models.Configuration;

namespace RetroPath.Core.Parsers;

public interface IParser<out T>
{
    public T Parse(InputConfiguration rpInputConfiguration, string filePath);
}
