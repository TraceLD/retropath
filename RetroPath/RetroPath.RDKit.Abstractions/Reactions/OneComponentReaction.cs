using GraphMolWrap;

namespace RetroPath.RDKit.Abstractions.Reactions;

/// <summary>
/// Represents a one component chemical reaction.
/// </summary>
public class OneComponentReaction : IDisposable
{
    private readonly ChemicalReaction _reaction;
    private readonly RWMol _reactant;

    /// <summary>
    /// Instantiates a new instance of <see cref="OneComponentReaction"/>.
    /// </summary>
    /// <param name="reactionSmarts">The reaction SMARTS.</param>
    /// <param name="reactant">The reactant.</param>
    public OneComponentReaction(string reactionSmarts, ROMol reactant)
    {
        _reaction = ChemicalReaction.ReactionFromSmarts(reactionSmarts);
        _reactant = new RWMol(reactant);
    }

    /// <summary>
    /// Runs the reaction and returns the generated unique products.
    /// </summary>
    /// <returns>The generated unique products.</returns>
    public HashSet<string> RunReaction()
    {
        using var reactantVect = new ROMol_Vect();
        reactantVect.Add(_reactant);
        using var products = _reaction.runReactants(reactantVect);

        var productsSmilesSet = new HashSet<string>(); 
        
        foreach (var r in products)
        {
            foreach (var product in r)
            {
                using var productRw = new RWMol(product);

                RDKFuncs.sanitizeMol(productRw);

                var smiles = productRw.MolToSmiles();
                
                productsSmilesSet.Add(smiles);
            }
        }

        return productsSmilesSet;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _reaction.Dispose();
        _reactant.Dispose();
    }
}