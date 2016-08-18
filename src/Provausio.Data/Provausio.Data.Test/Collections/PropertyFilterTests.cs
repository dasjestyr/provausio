using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Provausio.Data.Collections;
using Xunit;

namespace Provausio.Data.Test.Collections
{
    [ExcludeFromCodeCoverage]
    public class PropertyFilterTests
    {
        [Fact]
        public void Ctor_NullQuery_DoesNotThrow()
        {
            // arrange

            // act
            var filter = new PropertyFilter<FakeClass>(null);

            // assert
            Assert.NotNull(filter);
        }

        [Fact]
        public void Ctor_EmptyQuery_DoesNotThrow()
        {
            // arrange

            // act
            var filter = new PropertyFilter<FakeClass>(string.Empty);

            // assert
            Assert.NotNull(filter);
        }

        [Fact]
        public void Ctor_ValidQuery_Initializes()
        {
            // arrange

            // act
            var filter = new PropertyFilter<FakeClass>("foo");

            // assert
            Assert.NotNull(filter);
        }

        [Fact]
        public void Ctor_SingleProperty_Initializes()
        {
            // arrange

            // act
            var filter = new PropertyFilter<FakeClass>("foo", t => t.Property1);

            // assert
            Assert.NotNull(filter);
            Assert.Equal(1, filter.IncludedProperties.Count);
        }

        [Fact]
        public void Ctor_MultipleProperties_Initializes()
        {
            // arrange

            // act
            var filter = new PropertyFilter<FakeClass>("foo", 
                t => t.Property1, 
                t => t.Property2);

            // assert
            Assert.NotNull(filter);
            Assert.Equal(2, filter.IncludedProperties.Count);
        }

        [Fact]
        public void IncludeAll_IncludesAllProperties()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("foo");

            // act
            filter.IncludeAll();

            // assert   
            Assert.Equal(2, filter.IncludedProperties.Count);
        }

        [Fact]
        public void Apply_NullQuery_ReturnsSource()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>(null);
            filter.IncludeAll();
            var coll = GetSampleList().AsQueryable();

            // act
            var result = filter.Apply(coll);

            // assert
            Assert.Equal(coll, result);
        }

        [Fact]
        public void Apply_EmptyQuery_ReturnsSource()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>(string.Empty);
            filter.IncludeAll();
            var coll = GetSampleList().AsQueryable();

            // act
            var result = filter.Apply(coll);

            // assert
            Assert.Equal(coll, result);
        }

        [Fact]
        public void Apply_ExpectedResult()
        {
            // arrange
            var term = "foo";
            var coll = GetSampleList().AsQueryable();
            var filter = new PropertyFilter<FakeClass>(term);
            filter.IncludeAll();

            // act
            var result = filter.Apply(coll).ToList();

            // assert
            Assert.True(result.Count(t => t.Property1.Equals(term, StringComparison.OrdinalIgnoreCase)) == 1);
        }

        [Fact]
        public void Apply_Enumerable_ReturnsEnumerable()
        {
            // arrange
            var term = "foo";
            var coll = GetSampleList();
            var filter = new PropertyFilter<FakeClass>(term);
            filter.IncludeAll();

            // act
            var result = filter.Apply(coll).ToList();

            // assert
            Assert.True(result.Count(t => t.Property1.Equals(term, StringComparison.OrdinalIgnoreCase)) == 1);
        }

        private static IEnumerable<FakeClass> GetSampleList()
        {
            return new List<FakeClass>
            {
                new FakeClass {Property1 = "Foo", Property2 = 1},
                new FakeClass {Property1 = "Jon", Property2 = 16},
                new FakeClass {Property1 = "sweet", Property2 = 16}
            };
        }
    }

    [ExcludeFromCodeCoverage]
    internal class FakeClass
    {
        public string Property1 { get; set; }

        public int Property2 { get; set; }
    }
}
