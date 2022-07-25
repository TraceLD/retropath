namespace RetroPath.Core.Models.Intermediate.Scope;

public record ParsedReactions(IEnumerable<ParsedReactionSide> Left, IEnumerable<ParsedReactionSide> Right)
{
    public static ParsedReactions FromGroupedGlobalResults(IEnumerable<GroupedGlobalResult> r)
    {
        var rList = r.ToList();
        
        var left = new List<ParsedReactionSide>();
        var right = new List<ParsedReactionSide>();

        foreach (var result in rList)
        {
            left.AddRange(result.SubstrateSmiles.Select(s =>
                new ParsedReactionSide(result.TransformationId, s, result.Iteration)));

            right.AddRange(result.ProductSmiles.Select(s =>
                new ParsedReactionSide(result.TransformationId, s, result.Iteration)));
        }

        return new(left, right);
    }
}

public record ParsedReactionSide(string TransformationId, string Smiles, int Iteration);