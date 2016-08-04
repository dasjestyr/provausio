using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Provausio.Rest.Client.Ext;
using Provausio.Rest.Client.Infrastructure;
using Xunit;

namespace Provausio.Rest.Client.Test

{
    [ExcludeFromCodeCoverage]
    public class HttpresponseMessageExtTests
    {
        [Fact]
        public async Task Deserialize_Json_NoErrors()
        {
            // arrange
            var json = "{\"FirstName\" : \"Jon\", \"Age\" : 16, \"BirthDate\" : \"2/1/1201 12:00:00 AM\"}";
            var content = new StringContent(json);
            var handler = new FakeHandler(HttpStatusCode.OK, true);
            var client = new RestClient();
            client.WithScheme(Scheme.Http).WithHost("www.google.com");
            client.Handler = handler;

            var result = await client.PostAsync(content);

            // act
            var asObj = result.Deserialize<DeserializationTestObject>(BodyFormat.Json);

            // assert
            Assert.Equal("Jon", asObj.FirstName);
            Assert.Equal(16, asObj.Age);
            Assert.Equal(new DateTime(1201, 2, 1), asObj.BirthDate);
        }

        [Fact]
        public async Task Deserialize_UnsupportedFormat_Throws()
        {
            // arrange
            var json = "{\"FirstName\" : \"Jon\", \"Age\" : 16, \"BirthDate\" : \"2/1/1201 12:00:00 AM\"}";
            var content = new StringContent(json);
            var handler = new FakeHandler(HttpStatusCode.OK, true);
            var client = new RestClient();
            client.WithScheme(Scheme.Http).WithHost("www.google.com");
            client.Handler = handler;

            var result = await client.PostAsync(content);

            // act

            // assert
            try
            {
                result.Deserialize<DeserializationTestObject>(BodyFormat.Binary);
            }
            catch (AggregateException ex)
            {
                Assert.True(ex.InnerExceptions.Any(e => e is NotImplementedException));
                return;
            }

            Assert.False(true);
        }

        [Fact]
        public async Task Deserialize_UnspecifiedFormat_Throws()
        {
            // arrange
            var json = "{\"FirstName\" : \"Jon\", \"Age\" : 16, \"BirthDate\" : \"2/1/1201 12:00:00 AM\"}";
            var content = new StringContent(json);
            var handler = new FakeHandler(HttpStatusCode.OK, true);
            var client = new RestClient();
            client.WithScheme(Scheme.Http).WithHost("www.google.com");
            client.Handler = handler;

            var result = await client.PostAsync(content);

            // act

            // assert

            try
            {
                result.Deserialize<DeserializationTestObject>(BodyFormat.Unspecified);
            }
            catch (AggregateException ex)
            {
                Assert.True(ex.InnerExceptions.Any(e => e is ArgumentOutOfRangeException));
                return;
            }

            Assert.False(true);
        }
    }
}
