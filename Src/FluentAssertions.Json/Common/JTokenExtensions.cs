using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json.Common
{
    internal static class JTokenExtensions
    {
        private static readonly JTokenComparer Comparer = new();

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
                JArray array => new JArray(array.Select(Normalize).OrderBy(x => x, Comparer)),
                _ => token
            };
        }

        private sealed class JTokenComparer : IComparer<JToken>
        {
            public int Compare(JToken x, JToken y)
            {
                if (ReferenceEquals(x, y))
                    return 0;

                if (x is null)
                    return -1;

                if (y is null)
                    return 1;

                var typeComparison = x.Type.CompareTo(y.Type);
                if (typeComparison != 0)
                    return typeComparison;

                return x switch
                {
                    JArray a => Compare(a, (JArray)y),
                    JObject o => Compare(o, (JObject)y),
                    JProperty p => Compare(p, (JProperty)y),
                    JValue v => Compare(v, (JValue)y),
                    _ => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal)
                };
            }

            private static int Compare(JValue x, JValue y) => Comparer<object>.Default.Compare(x.Value, y.Value);

            private static int Compare(JArray x, JArray y)
            {
                var countComparison = x.Count.CompareTo(y.Count);
                if (countComparison != 0)
                    return countComparison;

                return x
                    .Select((t, i) => Comparer.Compare(t, y[i]))
                    .FirstOrDefault(itemComparison => itemComparison != 0);
            }

            private static int Compare(JObject x, JObject y)
            {
                var countComparison = x.Count.CompareTo(y.Count);
                if (countComparison != 0)
                    return countComparison;

                var xProperties = x.Properties().OrderBy(p => p.Name).ToArray();
                var yProperties = y.Properties().OrderBy(p => p.Name).ToArray();

                for (var i = 0; i < xProperties.Length; i++)
                {
                    var nameComparison = string.Compare(xProperties[i].Name, yProperties[i].Name, StringComparison.Ordinal);
                    if (nameComparison != 0)
                        return nameComparison;

                    var valueComparison = Comparer.Compare(xProperties[i].Value, yProperties[i].Value);
                    if (valueComparison != 0)
                        return valueComparison;
                }

                return 0;
            }

            private static int Compare(JProperty x, JProperty y)
            {
                var nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                return nameComparison != 0 ? nameComparison : Comparer.Compare(x.Value, y.Value);
            }
        }
    }
}
