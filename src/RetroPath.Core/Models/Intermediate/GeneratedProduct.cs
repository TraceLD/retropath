using RetroPath.Chem;

namespace RetroPath.Core.Models.Intermediate;

public record GeneratedProduct(List<string> Left, List<string> Right, ReactionRule Rule, ChemicalCompound Source);