using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    internal static class JTokenDifferentiator
    {
        public static Difference FindFirstDifference(JToken actual, JToken expected)
        {
            var path = new JPath();
            
            if (actual == expected)
            {
                return null;
            }

            if (actual == null)
            {
                return new Difference(DifferenceKind.ActualIsNull, path);
            }

            if (expected == null)
            {
                return new Difference(DifferenceKind.ExpectedIsNull, path);
            }
            
            return FindFirstDifference(actual, expected, path);
        }

        private static Difference FindFirstDifference(JToken actual, JToken expected, JPath path)
        {
            switch (actual)
            {
                case JArray actualArray:
                    return FindJArrayDifference(actualArray, expected, path);
                case JObject actualObbject:
                    return FindJObjectDifference(actualObbject, expected, path);
                case JProperty actualProperty:
                    return FindJPropertyDifference(actualProperty, expected, path);
                case JValue actualValue:
                    return FindValueDifference(actualValue, expected, path);
                default: 
                    throw new NotSupportedException();
            }
        }

        private static Difference FindJArrayDifference(JArray actualArray, JToken expected, JPath path)
        {
            if (!(expected is JArray expectedArray))
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualArray.Type), Describe(expected.Type));
            }
            
            return CompareItems(actualArray, expectedArray, path);
        }

        private static Difference CompareItems(JArray actual, JArray expected, JPath path)
        {
            JEnumerable<JToken> actualChildren = actual.Children();
            JEnumerable<JToken> expectedChildren = expected.Children();

            if (actualChildren.Count() != expectedChildren.Count())
            {
                return new Difference(DifferenceKind.DifferentLength, path, actualChildren.Count(), expectedChildren.Count());
            }

            for (int i = 0; i < actualChildren.Count(); i++)
            {
                Difference firstDifference = FindFirstDifference(actualChildren.ElementAt(i), expectedChildren.ElementAt(i), 
                    path.AddIndex(i));

                if (firstDifference != null)
                {
                    return firstDifference;
                }
            }

            return null;
        }

        private static Difference FindJObjectDifference(JObject actual, JToken expected, JPath path)
        {
            if (!(expected is JObject expectedObject))
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actual.Type), Describe(expected.Type));
            }

            return CompareProperties(actual?.Properties(), expectedObject.Properties(), path);
        }

        private static Difference CompareProperties(IEnumerable<JProperty> actual, IEnumerable<JProperty> expected, JPath path)
        {
            var actualDictionary = actual?.ToDictionary(p => p.Name, p => p.Value) ?? new Dictionary<string, JToken>();
            var expectedDictionary = expected?.ToDictionary(p => p.Name, p => p.Value) ?? new Dictionary<string, JToken>();

            foreach (KeyValuePair<string, JToken> expectedPair in expectedDictionary)
            {
                if (!actualDictionary.ContainsKey(expectedPair.Key))
                {
                    return new Difference(DifferenceKind.ActualMissesProperty, path.AddProperty(expectedPair.Key));
                }
            }

            foreach (KeyValuePair<string, JToken> actualPair in actualDictionary)
            {
                if (!expectedDictionary.ContainsKey(actualPair.Key))
                {
                    return new Difference(DifferenceKind.ExpectedMissesProperty, path.AddProperty(actualPair.Key));
                }
            }

            foreach (KeyValuePair<string, JToken> expectedPair in expectedDictionary)
            {
                JToken actualValue = actualDictionary[expectedPair.Key];

                Difference firstDifference = FindFirstDifference(actualValue, expectedPair.Value, 
                    path.AddProperty(expectedPair.Key));
                
                if (firstDifference != null)
                {
                    return firstDifference;
                }
            }

            return null;
        }

        private static Difference FindJPropertyDifference(JProperty actualProperty, JToken expected, JPath path)
        {
            if (!(expected is JProperty expectedProperty))
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualProperty.Type), Describe(expected.Type));
            }

            if (actualProperty.Name != expectedProperty.Name)
            {
                return new Difference(DifferenceKind.OtherName, path);
            }
            
            return FindFirstDifference(actualProperty.Value, expectedProperty.Value, path);
        }

        private static Difference FindValueDifference(JValue actualValue, JToken expected, JPath path)
        {
            if (!(expected is JValue expectedValue))
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualValue.Type), Describe(expected.Type));
            }
            
            return CompareValues(actualValue, expectedValue, path);
        }

        private static Difference CompareValues(JValue actual, JValue expected, JPath path)
        {
            if (actual.Type != expected.Type)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actual.Type), Describe(expected.Type));
            }
            
            if (!actual.Equals(expected))
            {
                return new Difference(DifferenceKind.OtherValue, path);
            }
            
            return null;
        }

        private static string Describe(JTokenType jTokenType)
        {
            switch (jTokenType)
            {
                case JTokenType.None:
                    return "type none";
                case JTokenType.Object:
                    return "an object";
                case JTokenType.Array:
                    return "an array";
                case JTokenType.Constructor:
                    return "a constructor";
                case JTokenType.Property:
                    return "a property";
                case JTokenType.Comment:
                    return "a comment";
                case JTokenType.Integer:
                    return "an integer";
                case JTokenType.Float:
                    return "a float";
                case JTokenType.String:
                    return "a string";
                case JTokenType.Boolean:
                    return "a boolean";
                case JTokenType.Null:
                    return "type null";
                case JTokenType.Undefined:
                    return "type undefined";
                case JTokenType.Date:
                    return "a date";
                case JTokenType.Raw:
                    return "type raw";
                case JTokenType.Bytes:
                    return "type bytes";
                case JTokenType.Guid:
                    return "a GUID";
                case JTokenType.Uri:
                    return "a URI";
                case JTokenType.TimeSpan:
                    return "a timespan";
                default:
                    throw new ArgumentOutOfRangeException(nameof(jTokenType), jTokenType, null);
            }
        }
    }

    internal class Difference
    {
        public Difference(DifferenceKind kind, JPath path, object actual, object expected) : this(kind, path)
        {
            Actual = actual;
            Expected = expected;
        }

        public Difference(DifferenceKind kind, JPath path)
        {
            Kind = kind;
            Path = path;
        }

        private DifferenceKind Kind { get; }
        private JPath Path { get; }
        private object Actual { get; }
        private object Expected { get; }

        public override string ToString()
        {
            switch (Kind)
            {
                case DifferenceKind.ActualIsNull:
                    return "is null";
                case DifferenceKind.ExpectedIsNull:
                    return "is not null";
                case DifferenceKind.OtherType:
                    return $"has {Actual} instead of {Expected} at {Path}";
                case DifferenceKind.OtherName:
                    return $"has a different name at {Path}";
                case DifferenceKind.OtherValue:
                    return $"has a different value at {Path}";
                case DifferenceKind.DifferentLength:
                    return $"has {Actual} elements instead of {Expected} at {Path}";
                case DifferenceKind.ActualMissesProperty:
                    return $"misses property {Path}";
                case DifferenceKind.ExpectedMissesProperty:
                    return $"has extra property {Path}";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal class JPath
    {
        private readonly List<string> nodes = new List<string>();
        
        public JPath()
        {
            nodes.Add("$");            
        }

        private JPath(JPath existingPath, string extraNode)
        {
            nodes.AddRange(existingPath.nodes);
            nodes.Add(extraNode);
        }

        public JPath AddProperty(string name)
        {
            return new JPath(this, $".{name}");
        }

        public JPath AddIndex(int index)
        {
            return new JPath(this, $"[{index}]");
        }

        public override string ToString()
        {
            return string.Join("", nodes);
        }
    }

    internal enum DifferenceKind
    {
        ActualIsNull,
        ExpectedIsNull,
        OtherType,
        OtherName,
        OtherValue,
        DifferentLength,
        ActualMissesProperty,
        ExpectedMissesProperty
    }
}