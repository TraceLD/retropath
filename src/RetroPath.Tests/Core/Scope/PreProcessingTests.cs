using FluentAssertions;
using RetroPath.Core.Scope;
using RetroPath.Tests.TestUtils.Fixtures;
using Xunit;

namespace RetroPath.Tests.Core.Scope;

public class PreProcessingTests : IClassFixture<PinocembrinResultsFixture>
{
    private readonly PinocembrinResultsFixture _fixture;

    public PreProcessingTests(PinocembrinResultsFixture fixture)
    {
        _fixture = fixture;
    }

    #region InitAndPopulateTests

    [Fact]
    public void ShouldPopulateSources()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 1032;
        var actualCount = populated.SourcesLookup.Count;

        actualCount.Should().Be(expectedCount);
    }
    
    [Fact]
    public void ShouldPopulateReactions()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 1032;
        var actualCount = populated.Reactions.Count;

        actualCount.Should().Be(expectedCount);
    }
    
    [Fact]
    public void ShouldPopulateLeftCompounds()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 1032;
        var actualCount = populated.LeftLookup.Count;

        actualCount.Should().Be(expectedCount);
    }
    
    [Fact]
    public void ShouldPopulateRightCompounds()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 2038;
        var actualCount = populated.RightLookup.Count;

        actualCount.Should().Be(expectedCount);
    }
    
    [Fact]
    public void ShouldPopulateCompounds()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 598;
        var actualCount = populated.Compounds.Count;

        actualCount.Should().Be(expectedCount);
    }
    
    [Fact]
    public void ShouldPopulateSinks()
    {
        var populated = new PreProcessedData();
        populated.InitAndPopulate(_fixture.GlobalResults);

        var expectedCount = 57;
        var actualCount = populated.SinksLookup.Count;

        actualCount.Should().Be(expectedCount);
    }

    #endregion
    
    
}