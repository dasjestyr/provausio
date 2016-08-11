using System.IO;
using System.Text;
using Xunit;

namespace Provausio.Parsing.Test
{
    public class DelimintedStringParserTests
    {
        private readonly string _sourceWithHeaders;
        private readonly string _sourceNoHeaders;

        public DelimintedStringParserTests()
        {
            _sourceWithHeaders = "firstName\tlastName\tage\r\nJon\tSnow\t16";
            _sourceNoHeaders = "Jon\tSnow\t16";
        }

        [Fact]
        public void ReadNext_WithHeaderRow_ReadsFile()
        {
            // arrange
            var stream = GetStream(_sourceWithHeaders);
            var reader = new DelimitedStringParser<FakeUser>(stream, "\t") {FirstRowHeaders = true};
            using (reader)
            {
                // act
                var result = reader.ReadNext();

                // assert
                Assert.True(result);
                Assert.Equal("Jon", reader.CurrentLine.FirstName);
                Assert.Equal("Snow", reader.CurrentLine.LastName);
                Assert.Equal(16, reader.CurrentLine.Age);
            }
        }

        [Fact]
        public void ReadNext_NoHeaderRow_ReadsFile()
        {
            // arrange
            var stream = GetStream(_sourceNoHeaders);

            using (var reader = new DelimitedStringParser<FakeUser>(stream, "\t"))
            {
                reader.Mapper.AddPropertyIndex(0, t => t.FirstName);
                reader.Mapper.AddPropertyIndex(1, t => t.LastName);
                reader.Mapper.AddPropertyIndex(2, t => t.Age);

                // act
                var result = reader.ReadNext();

                // assert
                Assert.True(result);
                Assert.Equal("Jon", reader.CurrentLine.FirstName);
                Assert.Equal("Snow", reader.CurrentLine.LastName);
                Assert.Equal(16, reader.CurrentLine.Age);
            }
        }

        private static Stream GetStream(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var stream = new MemoryStream(bytes);
            return stream;
        }
    }

    public class FakeUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
    }
}
