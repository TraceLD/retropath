using System.Text;

namespace RetroPath.Core.Extensions;

public static class CsvExtensions
{
    public static string ToCsvString(this IEnumerable<string> e)
    {
        var sb = new StringBuilder("[");

        sb.Append(string.Join(",", e));

        sb.Append(']');

        return sb.ToString();
    }
}