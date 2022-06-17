using System.Collections;

namespace RetroPath.Core.Models;

public record ReactionRuleLeft(
    string Smarts,
    int Cardinality,
    BitArray Fingerprint
);