using System;
using System.Net.Http;
using Moq;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    public class WebRequestTests
    {
        [Fact]
        public void Ctor_MethodOnly_Initializes()
        {
            // arrange

            // act
            var x = new Mock<WebRequest>(MockBehavior.Loose, HttpMethod.Get);

            // assert
            Assert.NotNull(x.Object);
        }

        [Fact]
        public void Ctor_MethodAndBuilder_Initializes()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();

            // act
            var x = new Mock<WebRequest>(MockBehavior.Loose, HttpMethod.Get, builder.Object);

            // assert
            Assert.NotNull(x.Object);
        }

        [Fact]
        public void GetHttpRequest_GetRequest_BuildsRequest()
        {
            // arrange
            var testUri = new Uri("http://www.google.com");
            var resourceMock = new Mock<IResourceBuilder>();
            resourceMock.Setup(m => m.BuildUri()).Returns(testUri);

            var request = new Mock<WebRequest>(MockBehavior.Loose, HttpMethod.Get, resourceMock.Object);
            request.Setup(m => m.GetHttpRequest()).CallBase();

            // act
            var x = request.Object.GetHttpRequest();

            // assert
            Assert.Equal(testUri, x.RequestUri);
            Assert.Equal(HttpMethod.Get, x.Method);
        }

        [Fact]
        public void GetHttpRequest_PostWithBody_AttachesBody()
        {
            // arrange
            var testContent = new StringContent("foo bar");
            var testUri = new Uri("http://www.google.com");

            var resourceMock = new Mock<IResourceBuilder>();
            resourceMock.Setup(m => m.BuildUri()).Returns(testUri);

            var requestMock = new Mock<WebRequest>(MockBehavior.Loose, HttpMethod.Post, resourceMock.Object);
            requestMock.Setup(m => m.GetContent()).Returns(testContent);
            requestMock.Setup(m => m.GetHttpRequest()).CallBase();

            // act
            var x = requestMock.Object.GetHttpRequest();

            // assert
            Assert.Equal(testContent, x.Content);
        }

        [Fact]
        public void GetHttpRequest_PostWithNoBody_NullContent()
        {
            // arrange
            var testUri = new Uri("http://www.google.com");

            var resourceMock = new Mock<IResourceBuilder>();
            resourceMock.Setup(m => m.BuildUri()).Returns(testUri);

            var requestMock = new Mock<WebRequest>(MockBehavior.Loose, HttpMethod.Post, resourceMock.Object);
            requestMock.Setup(m => m.GetContent()).CallBase();
            requestMock.Setup(m => m.GetHttpRequest()).CallBase();

            // act
            var x = requestMock.Object.GetHttpRequest();

            // assert
            Assert.Null(x.Content);
        }
    }
}
