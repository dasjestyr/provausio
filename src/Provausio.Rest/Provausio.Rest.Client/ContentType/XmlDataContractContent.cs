using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;

namespace Provausio.Rest.Client.ContentType
{
    /// <summary>
    /// Serializes a properly <see cref="DataContractAttribute"/>-decorated object to xml and adds it to the content stream.
    /// </summary>
    /// <seealso cref=".ByteArrayContent" />
    public class XmlDataContractContent : ByteArrayContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataContractContent"/> class.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="content">The content.</param>
        public XmlDataContractContent(Type objectType, object content)
            : base(GetContentBytes(objectType, content))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataContractContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
        public XmlDataContractContent(byte[] content) 
            : base(content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataContractContent"/> class.
        /// </summary>
        /// <param name="content">The content used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
        /// <param name="offset">The offset, in bytes, in the <paramref name="content" />  parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
        /// <param name="count">The number of bytes in the <paramref name="content" /> starting from the <paramref name="offset" /> parameter used to initialize the <see cref="T:System.Net.Http.ByteArrayContent" />.</param>
        public XmlDataContractContent(byte[] content, int offset, int count) 
            : base(content, offset, count)
        {
        }

        private static byte[] GetContentBytes(Type objectType, object content)
        {
            var xmlString = GetXmlContent(objectType, content);
            var asBytes = Encoding.UTF8.GetBytes(xmlString);
            return asBytes;
        }

        private static string GetXmlContent(Type objectType, object content)
        {
            var serializer = new DataContractSerializer(objectType);
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, content);
                using (var sr = new StreamReader(ms))
                {
                    ms.Position = 0;
                    var xmlString = sr.ReadToEnd();
                    return xmlString;
                }
            }
        }
    }
}
