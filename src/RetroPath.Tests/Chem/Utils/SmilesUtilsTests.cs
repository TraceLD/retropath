using System.Collections.Generic;
using FluentAssertions;
using RetroPath.Chem.Utils;
using Xunit;

namespace RetroPath.Tests.Chem.Utils;

public class SmilesUtilsTests
{
    public class IsMonomolecularTests
    {
        [Fact]
        public void ShouldReturnTrueForMono()
        {
            var smiles = "[H+]";
            var res = SmilesUtils.IsMonomolecular(smiles);
            
            res.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldReturnFalseForBi()
        {
            var smiles = "[H+].[K+]";
            var res = SmilesUtils.IsMonomolecular(smiles);
            
            res.Should().BeFalse();
        }
    }

    public class ConcatSmilesTests
    {
        [Fact]
        public void ShouldConcatWhenMultiple()
        {
            var smilesList = new List<string> {"[H+]", "[K+]"};
            var expected = "[H+].[K+]";
            var actual = SmilesUtils.ConcatSmiles(smilesList);

            actual.Should().Be(expected);
        }
        
        [Fact]
        public void ShouldNotAddDotWhenSingle()
        {
            var smilesList = new List<string> {"[H+]"};
            var expected = "[H+]";
            var actual = SmilesUtils.ConcatSmiles(smilesList);

            actual.Should().Be(expected);
        }
        
        [Fact]
        public void ShouldReturnEmptyWhenEmpty()
        {
            var smilesList = new List<string>();
            var expected = "";
            var actual = SmilesUtils.ConcatSmiles(smilesList);

            actual.Should().Be(expected);
        }
    }
}