using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Provausio.Rest.Client.Infrastructure;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    public class RestClientTests
    {
        [Fact]
        public void Ctor_Default_BuilderNotNull()
        {
            // arrange
            var client = new RestClient();

            // act
            client
                .WithScheme(Scheme.Http)
                .WithHost("www.google.com");

            // assert

            // if the builder was null, it would have thrown
            Assert.True(true);
        }

        [Fact]
        public void Ctor_WithSchemeAndHost_Initializes()
        {
            // arrange
            
            // act
            var client = new RestClient(Scheme.Https, "www.google.com");
            var builder = client.BuildUri();

            // assert
            Assert.Equal("https://www.google.com/", builder.ToString());
        }

        [Fact]
        public async Task Get_BuilderIsCalled()
        {
            // arrange
            var mockBuilder = new Mock<IResourceBuilder>();
            mockBuilder.Setup(m => m.BuildUri()).Returns(new Uri("http://www.google.com"));
            var client = new RestClient(mockBuilder.Object) {Handler = GetHandler(HttpStatusCode.OK)};

            // act
            var result = await client.GetAsync();

            // assert
            mockBuilder.Verify(m => m.BuildUri(), Times.Once);
        }

        [Fact]
        public async Task Get_MadeRequestToCorrectUrl()
        {
            // arrange
            var client = GetBaseClient();

            // act
            var result = await client.GetAsync();

            // assert
            Assert.Equal("GET http://www.google.com", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Delete_MadeRequestToCorrectUrl()
        {
            // arrange
            var client = GetBaseClient();

            // act
            var result = await client.DeleteAsync();
            // assert
            Assert.Equal("DELETE http://www.google.com", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Post_MadeRequestToCorrectUrl()
        {
            // arrange
            var client = GetBaseClient();

            // act
            var result = await client.PostAsync();

            // assert
            Assert.Equal("POST http://www.google.com", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Put_MadeREquestToCorrectUrl()
        {
            // arrange
            var client = GetBaseClient();

            // act
            var result = await client.PutAsync();

            // assert
            Assert.Equal("PUT http://www.google.com", await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Post_WithBody_BodyIsAttached()
        {
            // arrange
            var message = "Hello world";
            var content = new StringContent(message, Encoding.UTF8, "application/json");
            var client = GetBaseClient();
            client.Handler = new FakeHandler(HttpStatusCode.OK, true);

            // act
            var result = await client.PostAsync(content);

            // assert
            Assert.Equal(message, await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task Put_WithBody_BodyIsAttached()
        {
            // arrange
            var message = "Hello world";
            var content = new StringContent(message);
            var client = GetBaseClient();
            client.Handler = new FakeHandler(HttpStatusCode.OK, true);

            // act
            var result = await client.PutAsync(content);

            // assert
            Assert.Equal(message, await result.Content.ReadAsStringAsync());
        }

        [Fact]
        public void WithScheme_BuilderIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithScheme(It.IsAny<Scheme>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithScheme(Scheme.Http);

            // assert
            builder.Verify(m => m.WithScheme(It.IsAny<Scheme>()), Times.Once);
        }

        [Fact]
        public void WithHost_BuilderIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithHost(It.IsAny<string>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithHost("www.google.com");

            // assert
            builder.Verify(m => m.WithHost(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void WithPort_BuilderIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithPort(It.IsAny<uint>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithPort(1234);

            // assert
            builder.Verify(m => m.WithPort(It.IsAny<uint>()), Times.Once);
        }

        [Fact]
        public void WithPath_BuilderIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithPath(It.IsAny<string>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithPath("/this/path");

            // assert
            builder.Verify(m => m.WithPath(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void WithQueryParameters_Object_BuilderIsCalled()
        {
            // arrange
            var parameters = new {FirstName = "Jeremy"};
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithQueryParameters(It.IsAny<object>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithQueryParameters(parameters);

            // assert
            builder.Verify(m => m.WithQueryParameters(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void WithQueryParameters_ListOfKvp_BuilderIsCalled()
        {
            // arrange
            var parameters = new List<KeyValuePair<string, string>>();
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithQueryParameters(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithQueryParameters(parameters);

            // assert
            builder.Verify(m => m.WithQueryParameters(It.IsAny<IEnumerable<KeyValuePair<string, string>>>()), Times.Once);
        }

        [Fact]
        public void WithSegmentPair_BuilderIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.WithSegmentPair(It.IsAny<string>(), It.IsAny<string>()));
            var client = new RestClient(builder.Object);

            // arrange
            client.WithSegmentPair("Foo", "bar");

            // assert
            builder.Verify(m => m.WithSegmentPair(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void BuildUri_BuidlerIsCalled()
        {
            // arrange
            var builder = new Mock<IResourceBuilder>();
            builder.Setup(m => m.BuildUri());
            var client = new RestClient(builder.Object);

            // act
            var result = client.BuildUri();

            // assert
            builder.Verify(m => m.BuildUri(), Times.Once);

        }

        private static RestClient GetBaseClient()
        {
            var client = new RestClient();
            client.Handler = GetHandler(HttpStatusCode.OK);
            client.WithScheme(Scheme.Http).WithHost("www.google.com");
            return client;
        }

        private static HttpMessageHandler GetHandler(HttpStatusCode requestedCode)
        {
            return new FakeHandler(requestedCode);
        }
    }

    internal class FakeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _returnCode;
        private readonly bool _useProvidedBody;

        public FakeHandler(HttpStatusCode returnCode)
        {
            _returnCode = returnCode;
        }

        public FakeHandler(HttpStatusCode returnCode, bool useProvidedBody)
        {
            _returnCode = returnCode;
            _useProvidedBody = useProvidedBody;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = new HttpResponseMessage(_returnCode);

            var returnBody = _useProvidedBody
                ? new StringContent(await request.Content.ReadAsStringAsync())
                : new StringContent($"{request.Method} {request.RequestUri.ToString().Trim('/')}");

            result.Content = returnBody;

            return result;
        }
    }
}
