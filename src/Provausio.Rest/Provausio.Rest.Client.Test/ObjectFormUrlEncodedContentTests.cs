using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FormUrlEncodedContent = Provausio.Rest.Client.ContentType.FormUrlEncodedContent;

namespace Provausio.Rest.Client.Test
{
    public class ObjectFormUrlEncodedContentTests
    {
        private readonly object _testObject;

        public ObjectFormUrlEncodedContentTests()
        {
            _testObject = new { FirstName = "Jon", LastName = "Snow" };
        }

        [Fact]
        public void Ctor_Initializes()
        {
            // arrange

            // act
            var content = new FormUrlEncodedContent(_testObject);

            // assert
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Ctor_StreamsAreEqual()
        {
            // arrange
            var values = new Dictionary<string, string> {{"FirstName", "Jon"}, {"LastName", "Snow"}};
            var formContent = new FormUrlEncodedContent(values);
            
            // act
            var content = new FormUrlEncodedContent(_testObject);
            var formString = await formContent.ReadAsStringAsync();
            var objectFormString = await content.ReadAsStringAsync();

            // assert
            Assert.Equal(formString, objectFormString);
        }
    }
}
