namespace RetroPath.Core;

/// <summary>
/// Represents a generic results builder.
/// </summary>
/// <typeparam name="T">Results output type.</typeparam>
public interface IBuilder<out T>
{
    /// <summary>
    /// Builds the results.
    /// </summary>
    /// <returns>Built results.</returns>
    public IEnumerable<T> Build();
}