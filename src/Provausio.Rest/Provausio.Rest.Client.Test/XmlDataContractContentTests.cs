using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Provausio.Rest.Client.ContentType;
using Xunit;

namespace Provausio.Rest.Client.Test
{
    public class XmlDataContractContentTests
    {
        [Fact]
        public void Ctor_WithObject_Initializes()
        {
            // arrange
            var obj = new FakeXmlObject
            {
                FirstName = "Jon",
                LastName = "Snow"
            };

            // act
            var content = new XmlDataContractContent(typeof(FakeXmlObject), obj);

            // assert
            Assert.NotNull(content);
        }

        [Fact]
        public async Task Ctor_WithObject_StreamIsEqual()
        {
            // arrange
            var expectedXml = "<Person xmlns=\"http://schemas.datacontract.org/2004/07/Provausio.Rest.Client.Test\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><FirstName>Jon</FirstName><LastName>Snow</LastName></Person>";
            var obj = new FakeXmlObject {FirstName = "Jon", LastName = "Snow"};
            var asBinary = Encoding.UTF8.GetBytes(expectedXml);

            // act
            var testContent = new XmlDataContractContent(typeof(FakeXmlObject), obj);
            var testContentString = await testContent.ReadAsStringAsync();

            // assert
            Assert.Equal(asBinary, await testContent.ReadAsByteArrayAsync());
            Assert.Equal(expectedXml, testContentString);
        }
    }

    [DataContract(Name = "Person")]
    internal class FakeXmlObject
    {
        [DataMember(Name = "FirstName", Order = 0)]
        public string FirstName { get; set; }

        [DataMember(Name = "LastName", Order = 1)]
        public string LastName { get; set; }
    }
}
