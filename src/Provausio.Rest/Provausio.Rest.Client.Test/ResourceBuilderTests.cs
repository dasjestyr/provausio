using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Provausio.Rest.Client.Infrastructure;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    [ExcludeFromCodeCoverage]
    public class ResourceBuilderTests
    {
        [Fact]
        public void Ctor_Default_Initializes()
        {
            // arrange

            // act
            var builder = new ResourceBuilder();

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void Ctor_WithParameters_Initializes()
        {
            // arrange

            // act
            var builder = new ResourceBuilder(Scheme.Https, "www.google.com");

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void Ctor_UnspecifiedScheme_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new ResourceBuilder(Scheme.Unspecified, "www.google.com"));
        }

        [Fact]
        public void Ctor_NullHost_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new ResourceBuilder(Scheme.Http, null));
        }

        [Fact]
        public void Ctor_EmptyHost_Throws()
        {
            // arrange

            // act

            // assert
            Assert.Throws<ArgumentException>(() => new ResourceBuilder(Scheme.Http, string.Empty));
        }

        [Fact]
        public void ToString_NoScheme_Throws()
        {
            // arrange
            var builder = new ResourceBuilder();
            builder.WithHost("www.google.com");

            // act
            
            // assert
            Assert.Throws<InvalidOperationException>(() => builder.ToString());
        }

        [Fact]
        public void ToString_NullHost_Throws()
        {
            // arrange
            var builder = new ResourceBuilder();
            builder.WithScheme(Scheme.Http);

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => builder.ToString());
        }

        [Fact]
        public void ToString_EmptyHost_Throws()
        {
            // arrange
            var builder = new ResourceBuilder();
            builder.WithScheme(Scheme.Http);

            // act

            // assert
            Assert.Throws<InvalidOperationException>(() => builder.ToString());
        }

        [Fact]
        public void ToString_WithPort_ExpectedOutput()
        {
            // arrange
            var builder = GetBaseBuilder();
            builder.WithPort(1234);

            // act
            var result = builder.ToString();

            // assert
            Assert.Equal("http://www.google.com:1234", result);
        }

        [Fact]
        public void ToString_WithSegments_ExpectedOutput()
        {
            // arrange
            var builder = GetBaseBuilder();
            builder.WithSegmentPair("FirstName", "Jon");
            builder.WithSegmentPair("LastName", "Snow");

            // act
            var result = builder.ToString();

            // assert
            Assert.Equal("http://www.google.com/FirstName/Jon/LastName/Snow", result);
        }

        [Fact]
        public void ToString_WithQueryParameters_ExpectedOutput()
        {
            // arrange
            var builder = GetBaseBuilder();
            var query = new {Location = "Castle Black", Position = "Lord Commander"};
            builder.WithQueryParameters(query);

            // act
            var result = builder.ToString();

            // assert
            Assert.Equal("http://www.google.com?Location=Castle+Black&Position=Lord+Commander", result);
        }

        [Fact]
        public void ToString_FullUsage_ExpectedOutput()
        {
            // arrange
            var builder = new ResourceBuilder()
                .WithScheme(Scheme.Http)
                .WithHost("www.google.com")
                .WithPort(1234)
                .WithSegmentPair("FirstName", "Jon")
                .WithSegmentPair("LastName", "Snow")
                .WithQueryParameters(new { Location = "Castle Black", Position = "Lord Commander"});

            // act
            var result = builder.BuildUri();

            // assert
            Assert.Equal("http://www.google.com:1234/FirstName/Jon/LastName/Snow?Location=Castle+Black&Position=Lord+Commander", result.ToString());
        }

        [Fact]
        public void BuildUri_ValidUri_ExpectedOutput()
        {
            // arrange
            var builder = new ResourceBuilder()
                .WithScheme(Scheme.Http)
                .WithHost("www.google.com")
                .WithPort(1234)
                .WithSegmentPair("FirstName", "Jon")
                .WithSegmentPair("LastName", "Snow")
                .WithQueryParameters(new { Location = "Castle Black", Position = "Lord Commander" });

            // act
            var result = builder.ToString();

            // assert
            Assert.Equal("http://www.google.com:1234/FirstName/Jon/LastName/Snow?Location=Castle+Black&Position=Lord+Commander", result);
        }

        [Fact]
        public void ToString_WithSchemeAndHost_ExpectedOutput()
        {
            // arrange
            var builder = new ResourceBuilder(Scheme.Http, "www.google.com");

            // act
            var result = builder.ToString();

            // assert
            Assert.Equal("http://www.google.com", result);
        }

        [Fact]
        public void WithHost_EmptyHost_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();
            
            // act
            builder.WithHost(string.Empty);

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void WithHost_NullHost_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithHost(null);

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void WithPath_SingleSegment_IsExpected()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithPath("test");

            // assert
            Assert.Equal("http://www.google.com/test", builder.ToString());
        }

        [Fact]
        public void WithPath_MultiSegmentLeadingSlash_IsExpected()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithPath("/this/test/path");

            // assert
            Assert.Equal("http://www.google.com/this/test/path", builder.ToString());
        }

        [Fact]
        public void WithPath_MultiSegmentLeadAndTrailingSlash_IsExpected()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithPath("/this/test/path/");

            // assert
            Assert.Equal("http://www.google.com/this/test/path", builder.ToString());
        }

        [Fact]
        public void WithPort_OutOfRange_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            
            // assert
            Assert.Throws<ArgumentException>(() => builder.WithPort(65536));
        }

        [Fact]
        public void WithPort_InRange_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithPort(81);

            // assert
            Assert.NotNull(builder);

        }

        [Fact]
        public void WithQueryParameters_NullObject_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithQueryParameters((object) null));
        }

        [Fact]
        public void WithQueryParameters_ValidObject_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();
            var parameters = new {FirstName = "Jon", LastName = "Snow"};

            // act
            builder.WithQueryParameters(builder);

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void WithQueryParameters_NullCollection_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithQueryParameters(null)); // the cast is implicit, apparently
        }

        [Fact]
        public void WithQueryParameters_EmptyCollection_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();
            var parameters = new List<KeyValuePair<string, string>>();

            // act
            
            // assert
            Assert.Throws<ArgumentException>(() => builder.WithQueryParameters(parameters));
        }

        [Fact]
        public void WithQueryParameters_ValidParametersCollection_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();
            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("FirstName", "Jon"));

            // act
            builder.WithQueryParameters(parameters);

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void WithSegmentPair_NullName_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithSegmentPair(null, "Jon"));
        }

        [Fact]
        public void WithSegmentPair_EmptyName_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithSegmentPair(string.Empty, "Jon"));
        }

        [Fact]
        public void WithSegmentPair_NullValue_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithSegmentPair("FirstName", null));
        }

        [Fact]
        public void WithSegmentPair_EmptyValue_Throws()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => builder.WithSegmentPair("FirstName", string.Empty));
        }

        [Fact]
        public void WithSegmentPair_ValidPair_DoesNotThrow()
        {
            // arrange
            var builder = GetBaseBuilder();

            // act
            builder.WithSegmentPair("Jon", "Snow");

            // assert
            Assert.NotNull(builder);
        }

        [Fact]
        public void AsClient_ReturnsAttachedClient()
        {
            // arrange
            var client = new RestClient();

            // act
            var builtClient = new ResourceBuilder(client)
                .WithScheme(Scheme.Ftp)
                .WithHost("www.google.com")
                .AsClient();

            // assert
            Assert.Equal(client, builtClient);
        }

        private static ResourceBuilder GetBaseBuilder()
        {
            return new ResourceBuilder(Scheme.Http, "www.google.com");
        }
    }
}
