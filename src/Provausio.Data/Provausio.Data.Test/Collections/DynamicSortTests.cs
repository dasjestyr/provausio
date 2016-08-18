using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Provausio.Data.Collections;
using Xunit;

namespace Provausio.Data.Test.Collections
{
    [ExcludeFromCodeCoverage]
    public class DynamicSortTests
    {
        [Fact]
        public void Ctor_NullDefault_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => new DynamicSort<FakeClass>(null));
        }

        [Fact]
        public void AddKey_AddReferenceType_DoesNotThrow()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);

            // act
            sort.AddKey("name", "name desc", t => t.Name);

            // assert
            Assert.True(true);
        }

        [Fact]
        public void AddKey_AddValueType_DoesNotThrow()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);

            // act
            sort.AddKey("name", "name desc", t => t.Age);

            // assert
            Assert.True(true);
        }

        [Fact]
        public void Apply_SortByName_SortedHighestToLowest()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);
            sort.AddKey("age", "age desc", t => t.Age);
            var coll = GetSampleList();

            // act
            var result = sort.Apply("age", coll);
            var enumerated = result.ToList();

            // assert
            FakeSortClass current = null;
            foreach (var e in enumerated)
            {
                if (current == null)
                {
                    current = e;
                    continue;
                }

                Assert.True(e.Age > current.Age);

                current = e;
            }
        }

        [Fact]
        public void Apply_EmptyKey_SortsByDefault()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);
            sort.AddKey("age", "age desc", t => t.Age);
            var coll = GetSampleList().ToList();

            // act
            var result = sort.Apply(string.Empty, coll);
            
            // assert
            Assert.Equal("Jeremy", result.First().Name);
        }

        [Fact]
        public void Apply_NoMapper_SortsByDefault()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);
            var coll = GetSampleList();

            // act
            var result = sort.Apply("age", coll);

            // assert
            Assert.Equal("Jeremy", result.First().Name);
        }

        [Fact]
        public void Apply_WithThenBy_ExpectedOrder()
        {
            // arrange
            var sort = new DynamicSort<FakeSortClass>(t => t.Name);
            sort.AddKey("age", "age desc", t => t.Name, t => t.Age);
            var coll = GetSampleList();
            
            // act
            var result = sort.Apply("age", coll);

            // assert (verify that the grouping is ordered, basically)
            var oldestJulieAge = result.Where(p => p.Name.Equals("Julie")).Max(p => p.Age);
            var julies = result.Where(p => p.Name.Equals("Julie"));
            Assert.Equal(oldestJulieAge, julies.Last().Age);
        }

        private static IEnumerable<FakeSortClass> GetSampleList()
        {
            return new[]
            {
                new FakeSortClass
                {
                    Name = "Jeremy",
                    Age = 36,
                    BirthDate = new DateTime(1980, 5, 3)
                },
                new FakeSortClass
                {
                    Name = "Robyn",
                    Age = 33,
                    BirthDate = new DateTime(1983, 6, 20)
                },
                new FakeSortClass
                {
                    Name = "Julie",
                    Age = 37,
                    BirthDate = new DateTime(1979, 1, 14)
                },
                new FakeSortClass
                {
                    Name  = "Julie",
                    Age = 31,
                    BirthDate = new DateTime(1985, 9, 20)
                },
            };
        }
    }

    [ExcludeFromCodeCoverage]
    public class FakeSortClass
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
