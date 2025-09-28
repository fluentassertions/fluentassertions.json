using System.Linq;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json.Common
{
    internal static class JTokenExtensions
    {
        /// <summary>
        /// Recursively sorts the properties of JObject instances by name and
        /// the elements of JArray instances by their string representation,
        /// producing a normalized JToken for consistent comparison
        /// </summary>
        public static JToken Normalize(this JToken token)
        {
            return token switch
            {
                JObject obj => new JObject(obj.Properties().OrderBy(p => p.Name).Select(p => new JProperty(p.Name, Normalize(p.Value)))),
                JArray array => new JArray(array.Select(Normalize).OrderBy(x => x.ToString(Newtonsoft.Json.Formatting.None))),
                _ => token
            };
        }
    }
}
