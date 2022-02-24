using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    // REFACTOR: Change to non-static and make parameters fields
    internal static class JTokenDifferentiator
    {
        public static Difference FindFirstDifference(JToken actual, JToken expected, bool ignoreExtraProperties, Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
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

            return FindFirstDifference(actual, expected, path, ignoreExtraProperties, config);
        }

        private static Difference FindFirstDifference(JToken actual, JToken expected, JPath path, bool ignoreExtraProperties, Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            return actual switch
            {
                JArray actualArray => FindJArrayDifference(actualArray, expected, path, ignoreExtraProperties, config),
                JObject actualObject => FindJObjectDifference(actualObject, expected, path, ignoreExtraProperties, config),
                JProperty actualProperty => FindJPropertyDifference(actualProperty, expected, path, ignoreExtraProperties, config),
                JValue actualValue => FindValueDifference(actualValue, expected, path, config),
                _ => throw new NotSupportedException(),
            };
        }

        private static Difference FindJArrayDifference(JArray actualArray, JToken expected, JPath path,
            bool ignoreExtraProperties,
            Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            if (expected is not JArray expectedArray)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualArray.Type), Describe(expected.Type));
            }

            if (ignoreExtraProperties)
            {
                return CompareExpectedItems(actualArray, expectedArray, path, config);
            }
            else
            {
                return CompareItems(actualArray, expectedArray, path, config);
            }
        }

        private static Difference CompareExpectedItems(JArray actual, JArray expected, JPath path, Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            JToken[] actualChildren = actual.Children().ToArray();
            JToken[] expectedChildren = expected.Children().ToArray();

            int matchingIndex = 0;
            for (int expectedIndex = 0; expectedIndex < expectedChildren.Length; expectedIndex++)
            {
                var expectedChild = expectedChildren[expectedIndex];
                bool match = false;
                for (int actualIndex = matchingIndex; actualIndex < actualChildren.Length; actualIndex++)
                {
                    var difference = FindFirstDifference(actualChildren[actualIndex], expectedChild, true, config);

                    if (difference == null)
                    {
                        match = true;
                        matchingIndex = actualIndex + 1;
                        break;
                    }
                }

                if (!match)
                {
                    if (matchingIndex >= actualChildren.Length)
                    {
                        if (actualChildren.Any(actualChild => FindFirstDifference(actualChild, expectedChild, true, config) == null))
                        {
                            return new Difference(DifferenceKind.WrongOrder, path.AddIndex(expectedIndex));
                        }

                        return new Difference(DifferenceKind.ActualMissesElement, path.AddIndex(expectedIndex));
                    }

                    return FindFirstDifference(actualChildren[matchingIndex], expectedChild,
                        path.AddIndex(expectedIndex), true, config);
                }
            }

            return null;
        }

        private static Difference CompareItems(JArray actual, JArray expected, JPath path,
            Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            JToken[] actualChildren = actual.Children().ToArray();
            JToken[] expectedChildren = expected.Children().ToArray();

            if (actualChildren.Length != expectedChildren.Length)
            {
                return new Difference(DifferenceKind.DifferentLength, path, actualChildren.Length, expectedChildren.Length);
            }

            for (int i = 0; i < actualChildren.Length; i++)
            {
                Difference firstDifference = FindFirstDifference(actualChildren[i], expectedChildren[i],
                    path.AddIndex(i), false, config);

                if (firstDifference != null)
                {
                    return firstDifference;
                }
            }

            return null;
        }

        private static Difference FindJObjectDifference(JObject actual, JToken expected, JPath path, bool ignoreExtraProperties,
            Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            if (expected is not JObject expectedObject)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actual.Type), Describe(expected.Type));
            }

            return CompareProperties(actual?.Properties(), expectedObject.Properties(), path, ignoreExtraProperties, config);
        }

        private static Difference CompareProperties(IEnumerable<JProperty> actual, IEnumerable<JProperty> expected, JPath path,
            bool ignoreExtraProperties,
            Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
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
                if (!ignoreExtraProperties && !expectedDictionary.ContainsKey(actualPair.Key))
                {
                    return new Difference(DifferenceKind.ExpectedMissesProperty, path.AddProperty(actualPair.Key));
                }
            }

            foreach (KeyValuePair<string, JToken> expectedPair in expectedDictionary)
            {
                JToken actualValue = actualDictionary[expectedPair.Key];

                Difference firstDifference = FindFirstDifference(actualValue, expectedPair.Value,
                    path.AddProperty(expectedPair.Key), ignoreExtraProperties, config);

                if (firstDifference != null)
                {
                    return firstDifference;
                }
            }

            return null;
        }

        private static Difference FindJPropertyDifference(JProperty actualProperty, JToken expected, JPath path,
            bool ignoreExtraProperties,
            Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            if (expected is not JProperty expectedProperty)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualProperty.Type), Describe(expected.Type));
            }

            if (actualProperty.Name != expectedProperty.Name)
            {
                return new Difference(DifferenceKind.OtherName, path);
            }

            return FindFirstDifference(actualProperty.Value, expectedProperty.Value, path, ignoreExtraProperties, config);
        }

        private static Difference FindValueDifference(JValue actualValue, JToken expected, JPath path, Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            if (expected is not JValue expectedValue)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actualValue.Type), Describe(expected.Type));
            }

            return CompareValues(actualValue, expectedValue, path, config);
        }

        private static Difference CompareValues(JValue actual, JValue expected, JPath path, Func<IJsonAssertionOptions<object>, IJsonAssertionOptions<object>> config)
        {
            if (actual.Type != expected.Type)
            {
                return new Difference(DifferenceKind.OtherType, path, Describe(actual.Type), Describe(expected.Type));
            }

            bool hasMismatches;
            using (var scope = new AssertionScope())
            {
                actual.Value.Should().BeEquivalentTo(expected.Value, options => (JsonAssertionOptions<object>)config.Invoke(new JsonAssertionOptions<object>(options)));
                hasMismatches = scope.Discard().Length > 0;
            }

            if (hasMismatches)
            {
                return new Difference(DifferenceKind.OtherValue, path);
            }

            return null;
        }
        private static string Describe(JTokenType jTokenType)
        {
            return jTokenType switch
            {
                JTokenType.None => "type none",
                JTokenType.Object => "an object",
                JTokenType.Array => "an array",
                JTokenType.Constructor => "a constructor",
                JTokenType.Property => "a property",
                JTokenType.Comment => "a comment",
                JTokenType.Integer => "an integer",
                JTokenType.Float => "a float",
                JTokenType.String => "a string",
                JTokenType.Boolean => "a boolean",
                JTokenType.Null => "type null",
                JTokenType.Undefined => "type undefined",
                JTokenType.Date => "a date",
                JTokenType.Raw => "type raw",
                JTokenType.Bytes => "type bytes",
                JTokenType.Guid => "a GUID",
                JTokenType.Uri => "a URI",
                JTokenType.TimeSpan => "a timespan",
                _ => throw new ArgumentOutOfRangeException(nameof(jTokenType), jTokenType, null),
            };
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
            return Kind switch
            {
                DifferenceKind.ActualIsNull => "is null",
                DifferenceKind.ExpectedIsNull => "is not null",
                DifferenceKind.OtherType => $"has {Actual} instead of {Expected} at {Path}",
                DifferenceKind.OtherName => $"has a different name at {Path}",
                DifferenceKind.OtherValue => $"has a different value at {Path}",
                DifferenceKind.DifferentLength => $"has {Actual} elements instead of {Expected} at {Path}",
                DifferenceKind.ActualMissesProperty => $"misses property {Path}",
                DifferenceKind.ExpectedMissesProperty => $"has extra property {Path}",
                DifferenceKind.ActualMissesElement => $"misses expected element {Path}",
                DifferenceKind.WrongOrder => $"has expected element {Path} in the wrong order",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    internal class JPath
    {
        private readonly List<string> nodes = new();

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
            return string.Concat(nodes);
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
        ExpectedMissesProperty,
        ActualMissesElement,
        WrongOrder
    }
}
