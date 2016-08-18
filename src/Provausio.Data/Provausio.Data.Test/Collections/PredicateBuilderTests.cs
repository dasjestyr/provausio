using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provausio.Data.Collections;
using Xunit;

namespace Provausio.Data.Test.Collections
{
    public class PredicateBuilderTests
    {
        [Fact]
        public void True_ReturnsTrueExpression()
        {
            // arrange
            var expr = PredicateBuilder.True<int>();
            var compiled = expr.Compile();

            // act
            var result = compiled(1);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void False_ReturnsFalseExpression()
        {
            // arrange
            var expr = PredicateBuilder.False<int>();
            var compiled = expr.Compile();

            // act
            var result = compiled(10);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void Or_TrueFalse_ReturnsTrue()
        {
            // arrange
            var left = PredicateBuilder.True<int>();
            var right = PredicateBuilder.False<int>();
            var or = left.Or(right);
            var compiled = or.Compile();

            // act
            var result = compiled(1);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Or_FalseTrue_ReturnsTrue()
        {
            // arrange
            var left = PredicateBuilder.True<int>();
            var right = PredicateBuilder.False<int>();
            var or = right.Or(left);
            var func = or.Compile();

            // act
            var result = func(1);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Or_TrueTrue_ReturnsTrue()
        {
            // arrange
            var left = PredicateBuilder.True<int>();
            var right = PredicateBuilder.True<int>();
            var func = left.Or(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Or_FalseFalse_ReturnsFalse()
        {
            // arrange
            var left = PredicateBuilder.False<int>();
            var right = PredicateBuilder.False<int>();
            var func = left.Or(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void And_TrueTrue_ReturnsTrue()
        {
            // arrange
            var left = PredicateBuilder.True<int>();
            var right = PredicateBuilder.True<int>();
            var func = left.And(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void And_TrueFalse_ReturnsFalse()
        {
            // arrange
            var left = PredicateBuilder.True<int>();
            var right = PredicateBuilder.False<int>();
            var func = left.And(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void And_FalseTrue_ReturnsFalse()
        {
            // arrange
            var left = PredicateBuilder.False<int>();
            var right = PredicateBuilder.True<int>();
            var func = left.And(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void And_FalseFalse_ReturnsFalse()
        {
            // arrange
            var left = PredicateBuilder.False<int>();
            var right = PredicateBuilder.False<int>();
            var func = left.And(right).Compile();

            // act
            var result = func(10);

            // assert
            Assert.False(result);
        }
    }
}
