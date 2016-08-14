using System.Collections.Generic;
using System.Linq;
using Provausio.Data.Collections;
using Provausio.Data.Ext;
using Provausio.Data.Test.Collections;
using Xunit;

namespace Provausio.Data.Test.Ext
{
    public class WhereExtTests
    {
        [Fact]
        public void Where_ExactMatch_CaseSensitive_DoesMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("Foo");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Exact, true);

            // assert
            Assert.True(result.All(i => i.Property1 == "Foo"));
        }

        [Fact]
        public void Where_ExactMatch_CaseSensitive_DoesNotMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("Foo Bar Baz");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Exact, true);

            // assert
            Assert.False(result.Any());
        }

        [Fact]
        public void Where_ExactMatch_CaseInsensitive_DoesMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("foo");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Exact);

            // assert
            Assert.True(result.All(i => i.Property1 == "Foo"));
        }

        [Fact]
        public void Where_ExactMatch_CaseInsensitive_DoesNotMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("foo Bar Baz");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Exact);

            // assert
            Assert.False(result.Any());
        }

        [Fact]
        public void Where_LooseMatch_CaseSensitive_DoesMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("10 Bar");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Loose, true);

            // assert
            Assert.True(result.All(i => i.Property1.Contains("Foo")));
        }

        [Fact]
        public void Where_LooseMatch_CaseSensitive_DoesNotMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("space shuttle");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter, SearchMode.Loose, true);

            // assert
            Assert.False(result.Any());
        }

        [Fact]
        public void Where_LooseMatch_CaseInsensitive_DoesMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("10 foo");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter);

            // assert
            Assert.True(result.All(i => i.Property1.Contains("Foo")));
        }

        [Fact]
        public void Where_LooseMatch_CaseInsensitive_DoesNotMatch()
        {
            // arrange
            var filter = new PropertyFilter<FakeClass>("space shuttle");
            filter.IncludeAll();
            var source = GetSampleList();

            // act
            var result = source.Where(filter);

            // assert
            Assert.False(result.Any());
        }

        private static IEnumerable<FakeClass> GetSampleList()
        {
            return new List<FakeClass>
            {
                new FakeClass {Property1 = "Foo", Property2 = 1},
                new FakeClass {Property1 = "Jon", Property2 = 16},
                new FakeClass {Property1 = "sweet", Property2 = 16},
                new FakeClass {Property1 = "Foo Bar", Property2 = 11}
            };
        }
    }
}
