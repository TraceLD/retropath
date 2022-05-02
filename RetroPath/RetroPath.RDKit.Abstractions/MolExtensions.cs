using GraphMolWrap;
using Serilog;

namespace RetroPath.RDKit.Abstractions;

public static class MolExtensions
{
    public static RWMol? Transform(this RWMol reactant, IEnumerable<string> smartsList, int maxCycles)
    {
        var strSmiles = reactant.MolToSmiles();
        var mol = reactant;
        var iReaction = 0;

        foreach (var smarts in smartsList)
        {
            using var rxn = ChemicalReaction.ReactionFromSmarts(smarts);
            iReaction++;
            var iCycle = 0;

            try
            {
                while (iCycle < maxCycles)
                {
                    using var vReactant = new ROMol_Vect();
                    vReactant.Add(mol);

                    RWMol? molProduct = null;
                    using (var vvProducts = rxn.runReactants(vReactant))
                    {
                        if (vvProducts is not null && vvProducts.Any())
                        {
                            for (var i = 0; i < vvProducts.Count; i++)
                            {
                                using var vProds = vvProducts[i];

                                for (int j = 0; j < vProds.Count; j++)
                                {
                                    if (i == 0 && j == 0)
                                    {
                                        molProduct?.Dispose();
                                        molProduct = new RWMol(vProds[j]);
                                        vProds[j].Dispose();
                                    }
                                    else
                                    {
                                        vProds[j].Dispose();
                                    }
                                }
                            }
                        }
                    }

                    if (molProduct is not null)
                    {
                        iCycle++;
                        molProduct.updatePropertyCache(true);
                        mol.Dispose();
                        mol = molProduct;
                    }
                    else
                    {
                        break;
                    }
                }

                Log.Debug("Ran reaction {ReactionIteration} ('{Smarts}') with {Cycle} cycle for {Reactant}", iReaction,
                    smarts, iCycle, strSmiles);
            }
            catch (Exception e)
            {
                Log.Error(e, "Reaction {ReactionIteration} ('{Smarts}') failed for {Reactant} on {Cycle} cycle",
                    iReaction, smarts, strSmiles, iCycle);
            }
        }

        if (mol.getNumAtoms() > 0)
        {
            try
            {
                RDKFuncs.sanitizeMol(mol);
                return mol;
            }
            catch
            {
                Log.Error("A result product molecule could not be sanitized successfully - Result will be empty");
            }
        }
        
        return null;
    }
}