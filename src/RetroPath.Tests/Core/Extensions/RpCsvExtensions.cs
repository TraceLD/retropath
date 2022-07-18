using System.Collections.Generic;
using FluentAssertions;
using RetroPath.Core.Extensions;
using Xunit;

namespace RetroPath.Tests.Core.Extensions;

public class RpCsvExtensions
{
    public class EnumerableStringsToCsvStringTests
    {
        [Fact]
        public void ShouldReturnBracketsForEmpty()
        {
            var expected = "[]";

            var actual = new List<string>().ToCsvString();

            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldWorkCorrectlyForOneItem()
        {
            var expected = "[test]";

            var actual = new List<string> { "test" }.ToCsvString();

            actual.Should().Be(expected);
        }
        
        [Fact]
        public void ShouldWorkCorrectlyForMultipleItems()
        {
            var expected = "[test,test2]";

            var actual = new List<string> { "test", "test2" }.ToCsvString();

            actual.Should().Be(expected);
        }
    }
}