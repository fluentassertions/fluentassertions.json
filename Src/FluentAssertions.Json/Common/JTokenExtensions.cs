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
                JObject obj => new JObject(obj.Properties().OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => new JProperty(p.Name, Normalize(p.Value)))),
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
                    JConstructor c => Compare(c, (JConstructor)y),
                    _ => string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal)
                };
            }

            private static int Compare(JValue x, JValue y) => Comparer<object>.Default.Compare(x.Value, y.Value);

            private static int Compare(JConstructor x, JConstructor y)
            {
                var nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                return nameComparison != 0 ? nameComparison : Compare(x, (JContainer)y);
            }

            private static int Compare(JContainer x, JContainer y)
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

                return x.Properties()
                    .OrderBy(p => p.Name, StringComparer.Ordinal)
                    .Zip(y.Properties().OrderBy(p => p.Name, StringComparer.Ordinal), (px, py) => Compare(px, py))
                    .FirstOrDefault(itemComparison => itemComparison != 0);
            }

            private static int Compare(JProperty x, JProperty y)
            {
                var nameComparison = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
                return nameComparison != 0 ? nameComparison : Comparer.Compare(x.Value, y.Value);
            }
        }
    }
}
