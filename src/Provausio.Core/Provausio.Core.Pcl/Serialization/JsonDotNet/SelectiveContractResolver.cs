using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Provausio.Core.Pcl.Serialization.JsonDotNet
{
    public class SelectiveContractResolver : DefaultContractResolver
    {
        private readonly string[] _fields;

        public SelectiveContractResolver(string fields)
        {
            if (string.IsNullOrEmpty(fields))
            {
                _fields = new string[0];
                return;
            }

            var fieldColl = fields.Split(',');
            _fields = fieldColl
                .Select(f => f.ToLower().Trim())
                .ToArray();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = o => _fields.Contains(member.Name.ToLower());

            return property;
        }
    }
}
