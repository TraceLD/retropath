using RetroPath.Core.Models;

namespace RetroPath.Core;

// TODO: this is WIP

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

public record GlobalResult();

/// <summary>
/// Builder for building final results from a recursive iteration.
/// </summary>
public class GlobalResultsBuilder : IBuilder<GlobalResult>
{
    private readonly List<ParsedGeneratedCompound> _left;
    private readonly List<ParsedGeneratedCompound> _right;
    private readonly Dictionary<string, TransformationInfo> _transformations;
    private readonly List<ChemicalCompound> _newSourceInSink;

    public GlobalResultsBuilder(
        List<ParsedGeneratedCompound> left,
        List<ParsedGeneratedCompound> right,
        Dictionary<string, TransformationInfo> transformations,
        List<ChemicalCompound> newSourceInSink
    )
    {
        _left = left;
        _right = right;
        _transformations = transformations;
        _newSourceInSink = newSourceInSink;
    }

    /// <inheritdoc />
    public IEnumerable<GlobalResult> Build()
    {
        var leftUngrouped = _left.UngroupTransformations();
        var rightUngrouped = _right.UngroupTransformations();
        var leftAgg = leftUngrouped.Aggregate();
        var rightAgg = rightUngrouped.Aggregate();

        throw new NotImplementedException();
    }
}