using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using RetroPath.Core;
using RetroPath.Core.Models.Configuration;
using RetroPath.Core.Parsers;
using RetroPath.Tests.TestUtils.Fixtures;
using Xunit;

namespace RetroPath.Tests.Core;

public class RuleParserTests : IClassFixture<BigRulesFixture>
{
    private readonly BigRulesFixture _fixture;

    public RuleParserTests(BigRulesFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ShouldHaveCorrectRulesCount()
    {
        var expected = 229862;
        var actual = _fixture.Rules.Count;

        actual.Should().Be(expected);
    }

    [Fact]
    public void ShouldPopulateAllFieldsCorrectly()
    {
        var expected = new ReactionRule(
            new() {"RR-02-fbdda75e23f518b6-02-F"},
            "([#6&v4:1](=[#8&v2:2])(-[#6&v4:3](-[#6&v4:4])(-[#1&v1:5])-[#1&v1:6])-[#1&v1:7])>>([#6&v4:1](-[#8&v2:2]-[#1&v1:6])(-[#6&v4:3](-[#6&v4:4])(-[#8&v2]-[#1&v1])-[#1&v1:5])(-[#1&v1:7])-[#1&v1])",
            new() {"NOEC"},
            2,
            1,
            4.2956110769238762
        );

        var actual = _fixture.Rules
            .First(x => x.FoldedRuleId.Equals("[RR-02-fbdda75e23f518b6-02-F]@2"));

        expected.Should().BeEquivalentTo(actual);
    }
    
    // TODO: add tests for empty file;
}