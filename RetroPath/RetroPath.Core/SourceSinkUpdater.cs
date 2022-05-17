using GraphMolWrap;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Configuration;

namespace RetroPath.Core;

/// <summary>
/// Class that updates the Sources and Sink with the parsed products coming from firing rules and parsing the generated products.
/// </summary>
public class SourceSinkUpdater
{
    public record UpdatedSinkSource(
        List<ChemicalCompound> SourceInSink,
        Dictionary<string, ChemicalCompound> NewSink,
        List<ChemicalCompound> NewSources
    );
    
    private readonly Dictionary<string, ChemicalCompound> _sink;
    private readonly List<ParsedGeneratedCompound> _rightCompounds;
    private readonly Dictionary<string, TransformationInfo> _transformations;
    private readonly InputConfiguration _inputConfiguration;

    /// <summary>
    /// Constructor that instantiates a new instance of <see cref="SourceSinkUpdater"/>.
    /// </summary>
    /// <param name="sink">The existing sink.</param>
    /// <param name="rightCompounds">The parsed generated products.</param>
    /// <param name="transformations">The transformations.</param>
    /// <param name="inputConfiguration">RetroPath 2.0 input configuration.</param>
    public SourceSinkUpdater(
        Dictionary<string, ChemicalCompound> sink,
        List<ParsedGeneratedCompound> rightCompounds,
        IEnumerable<TransformationInfo> transformations,
        InputConfiguration inputConfiguration
    )
    {
        _sink = sink;
        _rightCompounds = rightCompounds;
        _transformations = transformations.ToDictionary(x => x.TransformationId);
        _inputConfiguration = inputConfiguration;
    }

    /// <summary>
    /// Updates the Sources and Sink with the parsed products coming from firing rules and parsing the generated products.
    /// </summary>
    /// <returns>Updated sink and sources.</returns>
    public UpdatedSinkSource Update()
    {
        var (alreadyInSink, newSources) = SplitAndUpdateSink();

        return new(alreadyInSink, _sink, newSources);
    }

    /// <summary>
    /// Splits compounds into ones which are already in sink and new sources.
    /// Adds the new sources to the sink by modifying the state of <see cref="_sink"/>.
    /// </summary>
    /// <returns>Compounds which are already in the sink and new sources.</returns>
    ///
    /// <remarks>
    /// Compares Inchi on each Sink to Lite Inchi on each Right Compound.
    ///
    /// Modifies the state of <see cref="_sink"/>.
    /// </remarks>
    private (List<ChemicalCompound> AlreadyInSink, List<ChemicalCompound> NewSources) SplitAndUpdateSink()
    {
        var alreadyInSink = new List<ChemicalCompound>();
        var newSources = new List<(double Score, ChemicalCompound Compound)>();

        foreach (var rightCompound in _rightCompounds)
        {
            if (rightCompound.LiteInchi is null) continue;
            
            // checks if already in the sink;
            if (_sink.TryGetValue(rightCompound.LiteInchi, out var val))
            {
                alreadyInSink.Add(val);
                rightCompound.Dispose(); // sink compounds do not need RDKit Mol to be maintained so we dispose to free memory;
            }
            else
            {
                UpdateSink(rightCompound);
                
                // filter mass;
                var weight = RDKFuncs.calcExactMW(rightCompound.Mol);
                if (weight > _inputConfiguration.SourceMw) continue;

                // get score (derived from transformation rule score) and name (derived from initial source) for each new source;
                var score = double.MaxValue;
                var names = new HashSet<string>();
                foreach (var tId in rightCompound.TransformationIds)
                {
                    var tInfo = _transformations[tId];
                    
                    score = Math.Min(score, tInfo.Score);
                    names.UnionWith(tInfo.InitialSource);
                }

                var cc = rightCompound.ToChemicalCompound(names);

                newSources.Add((score, cc));
            }
        }
        
        var bestNewSources = TakeBestSources(newSources); 
        
        return (alreadyInSink, bestNewSources);
    }

    /// <summary>
    /// Take first n new sources with the best transformation score where n is specified by <see cref="_inputConfiguration"/>.
    /// </summary>
    /// <param name="sources">Sources to filter.</param>
    private List<ChemicalCompound> TakeBestSources(IEnumerable<(double Score, ChemicalCompound Compound)> sources)
        => sources
            .OrderByDescending(x => x.Score)
            .Select(x => x.Compound)
            .TakeAndDispose(_inputConfiguration.MaxStructures)
            .ToList();

    /// <summary>
    /// Updates the sink with a new parsed generated compound.
    /// </summary>
    /// <param name="rightCompound">Parsed right compound to be added to the sink.</param>
    /// <exception cref="ArgumentException">Compound already exists in the sink.</exception>
    /// <exception cref="NullReferenceException"><see cref="ParsedGeneratedCompound.LiteInchi"/> on <see cref="rightCompound"/> is null.</exception>
    private void UpdateSink(ParsedGeneratedCompound rightCompound)
    {
        if (rightCompound.LiteInchi is null)
        {
            throw new NullReferenceException(
                $"Property LiteInchi on parameter {nameof(rightCompound)} cannot be null.");
        }
        
        var ccSink = new ChemicalCompound(new(), rightCompound.LiteInchi!, rightCompound.Smiles, null, false);
        _sink.Add(ccSink.Inchi, ccSink);
    }
}