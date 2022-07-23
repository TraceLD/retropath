using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using RetroPath.Chem.Extensions;
using RetroPath.Tests.TestUtils;
using Xunit;

namespace RetroPath.Tests.Core.Extensions;

public class DisposableLinqExtensionsTests
{
    public class TakeAndDisposeTests
    {
        [Fact]
        [SuppressMessage("Performance", "CA1806:Do not ignore method results")] // justification: return irrelevant for this test;
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")] // same as above;
        public void ShouldDisposeRest()
        {
            var c0 = new DisposeTestClass();
            var c1 = new DisposeTestClass();
            var collection = new List<DisposeTestClass> {c0, c1};
            
            collection.TakeAndDispose(1).ToList();

            var el0 = collection[0];
            var el1 = collection[1];

            el0.HasBeenDisposed.Should().BeFalse();
            el1.HasBeenDisposed.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldDisposeRestWhenSetToZero()
        {
            var c0 = new DisposeTestClass();
            var collection = new List<DisposeTestClass> {c0};
            
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // justification: returns irrelevant for what we are testing;
#pragma warning disable CA1806
            collection.TakeAndDispose(0).ToList();
#pragma warning restore CA1806

            var el0 = collection[0];
            
            el0.HasBeenDisposed.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldDisposeRestWhenSelected()
        {
            var collection = new List<DisposeTestClass> {new(), new()};
            
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            // justification: returns irrelevant for what we are testing;
#pragma warning disable CA1806
            collection.TakeAndDispose(1).Select(x => x.HasBeenDisposed).ToList();
#pragma warning restore CA1806

            var el0 = collection[1];
            
            el0.HasBeenDisposed.Should().BeTrue();
        }

        [Fact]
        public void ShouldTakeCorrectAmount()
        {
            var collection = new List<DisposeTestClass> { new(), new(), new() };

            var expected = collection.Take(2).ToList().Count;
            
            var actual = collection.TakeAndDispose(2).ToList().Count;

            actual.Should().Be(expected);
        }
        
        [Fact]
        public void ShouldTakeCorrectAmountWhenSetToOne()
        {
            var collection = new List<DisposeTestClass> { new() };

            var expected = collection.Take(1).ToList().Count;
            var actual = collection.TakeAndDispose(1).ToList().Count;

            actual.Should().Be(expected);
        }
        
        [Fact]
        public void ShouldTakeCorrectAmountWhenSetToZero()
        {
            var collection = new List<DisposeTestClass> { new() };

            var expected = collection.Take(0).ToList();
            var actual = collection.TakeAndDispose(0).ToList();

            actual.Count.Should().Be(expected.Count);
        }
    }
}