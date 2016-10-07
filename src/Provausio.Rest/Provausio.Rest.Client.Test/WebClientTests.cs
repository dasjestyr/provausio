using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    public class WebClientTests
    {
        [Fact]
        public void Ctor_Default_Initializes()
        {
            // arrange

            // act
            var client = new WebClient();

            // assert
            Assert.NotNull(client);
        }

        [Fact]
        public void Ctor_CustomHandler_Initializes()
        {
            // arrange
            var handler = new Mock<HttpMessageHandler>();

            // act
            var client = new WebClient(handler.Object);

            // assert
            Assert.NotNull(client);
        }

        [Fact]
        public async Task SendAsync_ReturnsResponse()
        {
            // arrange
            var handler = new FakeHandler(HttpStatusCode.OK);
            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);

            // act
            var response = await client.SendAsync(request);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SendAsyncT_NonSuccess_ReturnNullObject()
        {
            // arrange
            var errorBody = new StringContent("Test error");
            var handler = new FakeHandler(HttpStatusCode.BadRequest, errorBody);

            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);

            // act
            var result = await client.SendAsync<FakeClass1>(request);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SendAsyncT_SuccessWithBody_DeserializesBody()
        {
            // arrange
            var responseBody = "{}";
            var handler = new FakeHandler(HttpStatusCode.OK, new StringContent(responseBody));
            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);

            // act
            var result = await client.SendAsync<FakeClass1>(request);

            // assert
            Assert.NotNull(result);
            Assert.IsType(typeof(FakeClass1), result);
        }

        [Fact]
        public async Task SendAsyncT_SuccessNullBody_ReturnsNull()
        {
            // arrange
            var responseBody = "";
            var handler = new FakeHandler(HttpStatusCode.OK, new StringContent(responseBody));
            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);

            // act
            var result = await client.SendAsync<FakeClass1>(request);

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SendAsyncT_WithMapperDelegate_RunsDelegate()
        {
            // arrange
            var returnBody = new StringContent("{}");
            var handler = new FakeHandler(HttpStatusCode.OK, returnBody);
            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);
            var wasRun = false;

            // act
            var result = await client.SendAsync(request, (input) =>
            {
                wasRun = true;
                return new FakeClass1();
            });

            // assert
            Assert.NotNull(result);
            Assert.True(wasRun);
        }

        [Fact]
        public async Task SendAsyncT_WithMapperDelegate_NonSuccess_ReturnsNull()
        {
            // arrange
            var testError = new StringContent("Test error");
            var handler = new FakeHandler(HttpStatusCode.BadRequest, testError);
            var client = new WebClient(handler);
            var request = GetWebRequest(HttpMethod.Get, false);

            // act
            var result = await client.SendAsync(request, (input) => new FakeClass1());

            // assert
            Assert.Null(result);
        }

        private static WebRequest GetWebRequest(HttpMethod method, bool includeBody)
        {
            var httpRequest = new HttpRequestMessage(method, "http://www.vendi.com");

            if (includeBody)
            {
                var content = new StringContent("Foo bar");
                httpRequest.Content = content;
            }

            var mockRequest = new Mock<WebRequest>(MockBehavior.Loose, method);
            mockRequest.Setup(m => m.GetHttpRequest()).Returns(httpRequest);

            return mockRequest.Object;
        }

        private class FakeHandler : DelegatingHandler
        {
            private readonly HttpStatusCode _repsonseStatus;
            private readonly HttpContent _responseBody;

            public FakeHandler(HttpStatusCode repsonseStatus, HttpContent responseBody = null)
            {
                _repsonseStatus = repsonseStatus;
                _responseBody = responseBody;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_repsonseStatus);
                if (_responseBody != null)
                    response.Content = _responseBody;

                return Task.FromResult(response);
            }
        }

        private class FakeClass1
        {
        }
    }
}
