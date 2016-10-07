using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Provausio.Core.Pcl.Serialization.JsonDotNet;
using Xunit;

namespace Provausio.Core.Pcl.Test.Serialization.JsonDotNet
{
    public class SelectiveContractResolverTests
    {
        [Fact]
        public void Ctor_Initializes()
        {
            // arrange
            
            // act
            var resolver = new SelectiveContractResolver("field1");

            // assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void Ctor_NullFields_DoesNotThrow()
        {
            // arrange

            // act
            var resolver = new SelectiveContractResolver(null);

            // assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void Ctor_EmptyFields_DoesNotThrow()
        {
            // arrange

            // act
            var resolver = new SelectiveContractResolver("");

            // assert
            Assert.NotNull(resolver);
        }

        [Fact]
        public void CreateProperty_SingleProperty_IsReturned()
        {
            // arrange
            var settings = GetSettings("property1");
            var testObj = new FakeClass {Property1 = "prop1", Property2 = "prop2"};
            
            // act
            var result = JsonConvert.SerializeObject(testObj, Formatting.None, settings);
            var reversed = JsonConvert.DeserializeObject<FakeClass>(result);

            // assert
            Assert.NotNull(reversed.Property1);
            Assert.Null(reversed.Property2); // should be null since it wasn't included in the original serialization
        }

        [Fact]
        public void CreateProperty_MultipleProperties_AreReturned()
        {
            // arrange
            var settings = GetSettings("property1, property2");
            var testObj = new FakeClass { Property1 = "prop1", Property2 = "prop2" };

            // act
            var result = JsonConvert.SerializeObject(testObj, Formatting.None, settings);
            var reversed = JsonConvert.DeserializeObject<FakeClass>(result);

            // assert
            Assert.NotNull(reversed.Property1);
            Assert.NotNull(reversed.Property2); 
        }

        private JsonSerializerSettings GetSettings(string fields)
        {
            var resolver = new SelectiveContractResolver(fields);
            var settings = new JsonSerializerSettings {ContractResolver = resolver};
            return settings;
        }

        private class FakeClass
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }
        }
    }
}
