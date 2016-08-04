using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Provausio.Rest.Client.Infrastructure;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    [ExcludeFromCodeCoverage]
    public class QueryParameterCollectionTests
    {
        [Fact]
        public void Ctor_Default_Initializes()
        {
            // arrange

            // act
            var collection = new QueryParameterCollection();

            // assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void Ctor_WithDictionary_Initializes()
        {
            // arrange
            var testValues = new Dictionary<string, string>
            {
                {"prop1", "val1" },
                {"prop2", "val2" }
            };

            // act
            var collection= new QueryParameterCollection(testValues);

            // assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void Ctor_WithAnonObject_Initializes()
        {
            // arrange
            var testParams = new {FirstName = "Jon", LastName = "Snow"};

            // act
            var collection = new QueryParameterCollection(testParams);

            // assert
            Assert.NotNull(collection);
        }

        [Fact]
        public void Ctor_WithEmptyObject_Throws()
        {
            // arrange
            var parameters = new object();

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new QueryParameterCollection(parameters));
        }

        [Fact]
        public void ToString_EmptyCollection_ReturnsEmpty()
        {
            // arrange
            var collection = new QueryParameterCollection();

            // act
            var result = collection.ToString();

            // assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ToString_WithParameters_ExpectedResult()
        {
            // arrange
            var parameters = new {FirstName = "Jon", LastName = "Snow"};
            var collection = new QueryParameterCollection(parameters);

            // act
            var result = collection.ToString();

            // assert
            Assert.Equal("FirstName=Jon&LastName=Snow", result);
        }
    }
}
