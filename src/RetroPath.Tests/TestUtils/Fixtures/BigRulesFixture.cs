using System.Collections.Generic;
using RetroPath.Core;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;

namespace RetroPath.Tests.TestUtils.Fixtures;

public class BigRulesFixture
{
    public List<ReactionRule> Rules { get; }
    
    public BigRulesFixture()
    {
        var rpConfig = new InputConfiguration(
            @"C:\Users\Lukasz\Documents\projects\retropath-standalone\data\retrorules_rr02_rp2_flat_all.csv",
            @"sink_file",
            "source_file",
            1000,
            "cofacts_file",
            1000,
            0,
            1000,
            4,
            100
        );

        Rules = new RulesParser(rpConfig).Parse(rpConfig.RulesFilePath);
    }
}