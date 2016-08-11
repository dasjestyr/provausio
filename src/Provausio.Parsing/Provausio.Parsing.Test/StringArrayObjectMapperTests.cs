using Xunit;

namespace Provausio.Parsing.Test
{
    public class StringArrayObjectMapperTests
    {
        [Fact]
        public void MapObject_DecoratedClass__NoExplicitMapping_PropertiesMapped()
        {
            // arrange
            var testArray = new[] {"Hello", "World"};
            var parser = new StringArrayObjectMapper<FakeDecorated>();

            // act
            var result = parser.MapObject(testArray);

            // assert
            Assert.Equal("Hello", result.Property1);
            Assert.Equal("World", result.Property2);
            Assert.Equal(false, result.Property3); // default; not set
        }

        [Fact]
        public void MapObject_DecoratedClass_WithExplicitMapping_PropertiesMapped()
        {
            // arrange
            var testArray = new[] {"Hello", "World", "Y"};
            var parser = new StringArrayObjectMapper<FakeDecorated>();
            parser.IndexMapper.AddPropertyIndex(2, t => t.Property3, v => v == "Y");

            // act
            var result = parser.MapObject(testArray);

            // assert
            Assert.Equal("Hello", result.Property1);
            Assert.Equal("World", result.Property2);
            Assert.Equal(true, result.Property3); 
        }

        [Fact]
        public void MapObject_WithHeadersAndExplicit_PropertiesMapped()
        {
            // arrange
            var headers = new[] {"Property1", "Property2", "Property3"};
            var source = new[] {"Hello", "World", "Y"};
            var parser = new StringArrayObjectMapper<FakeUndecorated>();
            parser.IndexMapper.AddPropertyIndex(2, t => t.Property3, v => v == "Y");

            // act
            var result = parser.MapObject(source, headers);

            // assert
            Assert.Equal("Hello", result.Property1);
            Assert.Equal("World", result.Property2);
            Assert.Equal(true, result.Property3);
        }
    }

    internal class FakeUndecorated
    {
        public string Property1 { get; set; }

        public string Property2 { get; set; }

        public bool Property3 { get; set; }
    }

    internal class FakeDecorated
    {
        [ArrayProperty(0)]
        public string Property1 { get; set; }

        [ArrayProperty(1)]
        public string Property2 { get; set; }
        
        // do not decorate if setting explicit
        public bool Property3 { get; set; }
    }
}
