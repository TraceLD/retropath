using RetroPath.Chem;
using RetroPath.Core.Chem;
using RetroPath.Core.Chem.Reactions;

namespace RetroPath.Core.Models.Intermediate;

public record GeneratedProduct(List<string> Left, List<string> Right, ReactionRule Rule, ChemicalCompound Source);