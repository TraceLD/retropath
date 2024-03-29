﻿using RetroPath.Chem;
using RetroPath.Chem.Utils;
using RetroPath.Core.Extensions;
using RetroPath.Core.Models;
using RetroPath.Core.Models.Intermediate;

namespace RetroPath.Core;

/// <summary>
/// Builder for building final results from a recursive iteration.
/// </summary>
public class GlobalResultsBuilder : IResultsBuilder<GlobalResult>
{
    private readonly List<ParsedGeneratedCompound> _left;
    private readonly List<ParsedGeneratedCompound> _right;
    private readonly Dictionary<string, TransformationInfo> _transformations;
    private readonly List<ChemicalCompound> _newSourceInSink;
    private readonly int _iteration;

    /// <summary>
    /// Constructor that instantiates a new instance of <see cref="GlobalResultsBuilder"/>.
    /// </summary>
    /// <param name="left">Left (substrates) coming from <see cref="GeneratedProductsParser"/>.</param>
    /// <param name="right">Right (products) coming from <see cref="GeneratedProductsParser"/>.</param>
    /// <param name="transformations">Transformations coming from <see cref="GeneratedProductsParser"/>.</param>
    /// <param name="newSourceInSink">Post-update sources in sink.</param>
    /// <param name="iteration">Iteration.</param>
    public GlobalResultsBuilder(
        List<ParsedGeneratedCompound> left,
        List<ParsedGeneratedCompound> right,
        Dictionary<string, TransformationInfo> transformations,
        List<ChemicalCompound> newSourceInSink,
        int iteration
    )
    {
        _left = left;
        _right = right;
        _transformations = transformations;
        _newSourceInSink = newSourceInSink;
        _iteration = iteration;
    }

    /// <inheritdoc />
    public IEnumerable<GlobalResult> Build()
    {
        var leftUngrouped = _left.UngroupTransformations().ToList();
        var rightUngrouped = _right.UngroupTransformations().ToList();
        var leftAgg = leftUngrouped.Aggregate();
        var rightAgg = rightUngrouped.Aggregate();

        var reactions = leftAgg
            // joins left and right on transformation ID and creates reaction SMARTS;
            .Join(
                rightAgg,
                l => l.Key, r => r.Key,
                (left, right) => new
                {
                    TranformationId = left.Key,
                    Left = left.Value,
                    Right = right.Value,
                    ReactionSmarts = SmilesUtils.CreateReactionSmarts(left.Value.Smiles, right.Value.Smiles)
                })
            .ToList();
        var substratesInfo = reactions
            .Join(
                leftUngrouped,
                r => r.TranformationId,
                l => l.TransformationId,
                (reaction, left) => new
                {
                    reaction.TranformationId,
                    reaction.ReactionSmarts,
                    SubstrateSmiles = left.Smiles,
                    SubstrateInchi = left.Inchi
                });
        var productsInfo = reactions
            .Join(
                rightUngrouped,
                rxn => rxn.TranformationId,
                r => r.TransformationId, (reaction, right) => new
                {
                    reaction.TranformationId,
                    reaction.ReactionSmarts,
                    ProductSmiles = right.Smiles,
                    ProductInchi = right.Inchi
                });

        var globalResults = substratesInfo
            // join substrates with products into final reactions;
            .Join(
                productsInfo,
                s => s.TranformationId,
                p => p.TranformationId,
                (substrate, product) => new
                {
                    substrate.TranformationId,
                    substrate.ReactionSmarts,
                    substrate.SubstrateSmiles,
                    substrate.SubstrateInchi,
                    product.ProductInchi,
                    product.ProductSmiles
                })
            // join rule info
            .Join(
                _transformations,
                rxn => rxn.TranformationId,
                t => t.Key,
                (rxn, transformation) => (rxn, transformation.Value))
            // join sink info
            .GroupJoin(
                _newSourceInSink,
                i => i.rxn.ProductInchi,
                s => s.Inchi,
                (rxnInfo, sinkInfo) =>
                {
                    var sink = sinkInfo.SingleOrDefault();
                    
                    var initialSource = rxnInfo.Value.InitialSource;
                    var transformationId = rxnInfo.rxn.TranformationId;
                    var reactionSmarts = rxnInfo.rxn.ReactionSmarts;
                    var substrateSmiles = rxnInfo.rxn.SubstrateSmiles;
                    var substrateInchi = rxnInfo.rxn.SubstrateInchi;
                    var productSmiles = rxnInfo.rxn.ProductSmiles;
                    var productInchi = rxnInfo.rxn.ProductInchi;
                    var inSink = sink?.Initial ?? false;
                    var sinkNames = sink?.Names is not null
                        ? sink.Names.Any() ? sink.Names : new() {"None"}
                        : new() {"None"};
                    var diameter = rxnInfo.Value.Diameter;
                    var ruleIds = rxnInfo.Value.RuleIds;
                    var ecNumber = rxnInfo.Value.EcNumber;
                    var score = rxnInfo.Value.Score;

                    return new GlobalResult(initialSource, transformationId, reactionSmarts, substrateSmiles,
                        substrateInchi, productSmiles, productInchi, inSink, sinkNames, diameter, ruleIds, ecNumber,
                        score, _iteration);
                });

        return globalResults;
    }
}