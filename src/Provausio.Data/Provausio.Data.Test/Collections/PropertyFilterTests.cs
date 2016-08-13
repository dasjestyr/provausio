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
        public void IsLooseMatch_ShouldMatch_MatchesFirstProperty()
        {
            // arrange
            var testClass = new FakeClass {Property1 = "Foo", Property2 = 10};
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void IsLooseMatch_ShouldMatch_MatchesSecondProperty()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("10");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void IsLooseMatch_ShouldNotMatch_MatchesFirstProperty()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("55");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.False(isMatch);
        }

        [Fact]
        public void IsLooseMatch_CaseSensitive_DoesNotMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsLooseMatch(testClass, true);

            // assert
            Assert.False(isMatch);
        }

        [Fact]
        public void IsLooseMatch_CaseInSensitive_DoesNotMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void IsExactMatch_MatchingPhrase_CaseSensitive_DoesMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("Foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsExactMatch(testClass);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void IsExactMatch_MatchingPhrase_CaseSensitive_DoesNotMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsExactMatch(testClass);

            // assert
            Assert.False(isMatch);
        }

        [Fact]
        public void IsExactMatch_MatchingPhrase_CaseInsensitive_DoesMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsExactMatch(testClass, false);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void IsExactMatch_MatchingPhrase_CaseInsensitive_DoesNotMatch()
        {
            // arrange
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            var filter = new PropertyFilter<FakeClass>("20");
            filter.Include(
                t => t.Property1,
                t => t.Property2);

            // act
            var isMatch = filter.IsExactMatch(testClass, false);

            // assert
            Assert.False(isMatch);
        }

        [Fact]
        public void ObjectCtor_DoesMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("foo");
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            filter.IncludeAll();

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.True(isMatch);
        }

        [Fact]
        public void ObjectCtor_DoesNotMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("bar");
            var testClass = new FakeClass { Property1 = "Foo", Property2 = 10 };
            filter.IncludeAll();

            // act
            var isMatch = filter.IsLooseMatch(testClass);

            // assert
            Assert.False(isMatch);
        }

        [Fact]
        public void IsLooseMatch_WithCollection_OnlyMatchesFound()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("16");
            filter.IncludeAll();

            // act
            var results = GetSampleList().Where(x => filter.IsLooseMatch(x));

            // assert
            Assert.True(results.All(r => r.Property2 == 16));
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
