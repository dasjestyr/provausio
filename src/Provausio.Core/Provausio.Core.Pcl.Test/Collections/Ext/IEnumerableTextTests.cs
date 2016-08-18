using System;
using System.Collections.Generic;
using System.Linq;
using Provausio.Core.Pcl.Collections.Ext;
using Xunit;

namespace Provausio.Core.Pcl.Test.Collections.Ext
{
    public class EnumerableChunkTests
    {
        [Fact]
        public void Chunk_NullList_Throws()
        {
            // arrange
            List<int> list = null;

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => list.Chunk(100).ToList());

        }

        [Fact]
        public void Chunk_EmptyList_ReturnsSameList()
        {
            // arrange
            var list = new List<int>();

            // act
            var result = list.Chunk(100);

            // assert
            Assert.Equal(0, result.Count());
        }

        [Theory]
        [InlineData(10, 5)]
        [InlineData(11, 5)]
        [InlineData(103, 5)]
        public void Chunk_ValidList_ReturnsExpectedChunks(int listSize, int chunkSize)
        {
            // arrange
            var collection = GenFu.GenFu.ListOf<int>(listSize);
            var expectedChunkCount = (listSize/chunkSize) + (listSize%chunkSize == 0 ? 0 : 1);

            // act
            var chunk = collection.Chunk(chunkSize);

            // assert
            Assert.Equal(expectedChunkCount, chunk.Count());
        }

        [Fact]
        public void Chunk_CanIterate()
        {
            // arrange
            var collection = GenFu.GenFu.ListOf<int>(100);
            var expectedSum = collection.Sum();

            // act
            var accumulator = 0;
            foreach (var chunk in collection.Chunk(5))
            {
                accumulator += chunk.Sum();
            }

            // assert
            Assert.Equal(expectedSum, accumulator);
        }
    }
}
