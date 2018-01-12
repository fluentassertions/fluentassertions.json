using System.Diagnostics;
using System.Linq;
using FluentAssertions.Collections;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    /// <summary>
    ///     Contains a number of methods to assert that an <see cref="JToken" /> is in the expected state.
    /// </summary>
    [DebuggerNonUserCode]
    public class JTokenAssertions : ReferenceTypeAssertions<JToken, JTokenAssertions>
    {
        private GenericCollectionAssertions<JToken> EnumerableSubject { get; }

        static JTokenAssertions()
        {
            Formatter.AddFormatter(new JTokenFormatter());
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JTokenAssertions" /> class.
        /// </summary>
        /// <param name="subject">The subject</param>
        public JTokenAssertions(JToken subject)
        {
            Subject = subject;
            EnumerableSubject = new GenericCollectionAssertions<JToken>(subject);
        }

        /// <summary>
        ///     Returns the type of the subject the assertion applies on.
        /// </summary>
#if !PORTABLE && !CORE_CLR
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        protected override string Context => nameof(JToken);

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> equals the <paramref name="expected" /> element.
        /// </summary>
        /// <param name="expected">The expected element</param>
        public AndConstraint<JTokenAssertions> Be(JToken expected)
        {
            return Be(expected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> equals the <paramref name="expected" /> element.
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> Be(JToken expected, string because, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(JToken.DeepEquals(Subject, expected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON document to be {0}{reason}, but found {1}.", expected, Subject);

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> does not equal the <paramref name="unexpected" /> element,
        ///     using its <see cref="object.Equals(object)" /> implementation.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        public AndConstraint<JTokenAssertions> NotBe(JToken unexpected)
        {
            return NotBe(unexpected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> does not equal the <paramref name="unexpected" /> element,
        ///     using its <see cref="object.Equals(object)" /> implementation.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> NotBe(JToken unexpected, string because, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition((ReferenceEquals(Subject, null) && !ReferenceEquals(unexpected, null)) ||
                              !JToken.DeepEquals(Subject, unexpected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON document not to be {0}{reason}.", unexpected);

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is equivalent to the <paramref name="expected" /> element,
        ///     using its <see cref="JToken.DeepEquals(JToken, JToken)" /> implementation.
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <remarks>
        /// Json tokens are compared by inspecting all properties. They are considered equal
        /// when all property names and values match (independent of their order).
        /// When not equal, the first mismatching property or value is included in the assertion failure message.
        /// </remarks>
        public AndConstraint<JTokenAssertions> BeEquivalentTo(JToken expected)
        {
            return BeEquivalentTo(expected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is equivalent to the <paramref name="expected" /> element,
        ///     using its <see cref="JToken.DeepEquals(JToken, JToken)" /> implementation.
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> BeEquivalentTo(JToken expected, string because,
            params object[] becauseArgs)
        {
            ObjectDiffPatchResult diff = ObjectDiffPatch.GenerateDiff(Subject, expected);
            JToken firstDifferingToken = diff.NewValues?.First ?? diff.OldValues?.First;
            JTokenFormatter formatter = new JTokenFormatter();

            Execute.Assertion
                .ForCondition(diff.AreEqual)
                .BecauseOf(because, becauseArgs)
                .FailWith($"Expected JSON document {formatter.ToString(Subject, true)}" +
                    $" to be equivalent to {formatter.ToString(expected, true)}" +
                    $"{{reason}}, but differs at {firstDifferingToken}.");

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is not equivalent to the <paramref name="unexpected" /> element,
        ///     using its <see cref="JToken.DeepEquals(JToken, JToken)" /> implementation.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        public AndConstraint<JTokenAssertions> NotBeEquivalentTo(JToken unexpected)
        {
            return NotBeEquivalentTo(unexpected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is not equivalent to the <paramref name="unexpected" /> element,
        ///     using its <see cref="JToken.DeepEquals(JToken, JToken)" /> implementation.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> NotBeEquivalentTo(JToken unexpected, string because,
            params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(!JToken.DeepEquals(Subject, unexpected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Did not expect JSON element {0} to be equivalent to {1}{reason}.",
                    Subject, unexpected);

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> has the specified <paramref name="expected" /> value.
        /// </summary>
        /// <param name="expected">The expected value</param>
        public AndConstraint<JTokenAssertions> HaveValue(string expected)
        {
            return HaveValue(expected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> has the specified <paramref name="expected" /> value.
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> HaveValue(string expected, string because, params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Value<string>() == expected)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON property {0} to have value {1}{reason}, but found {2}.",
                    Subject.Path, expected, Subject.Value<string>());

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> does not have the specified <paramref name="unexpected" /> value.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> NotHaveValue(string unexpected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Value<string>() != unexpected)
                .BecauseOf(because, becauseArgs)
                .FailWith("Did not expect JSON property {0} to have value {1}{reason}.",
                    Subject.Path, unexpected, Subject.Value<string>());

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> has a direct child element with the specified
        ///     <paramref name="expected" /> name.
        /// </summary>
        /// <param name="expected">The name of the expected child element</param>
        public AndWhichConstraint<JTokenAssertions, JToken> HaveElement(string expected)
        {
            return HaveElement(expected, string.Empty);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> has a direct child element with the specified
        ///     <paramref name="expected" /> name.
        /// </summary>
        /// <param name="expected">The name of the expected child element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndWhichConstraint<JTokenAssertions, JToken> HaveElement(string expected, string because,
            params object[] becauseArgs)
        {
            JToken jToken = Subject[expected];
            Execute.Assertion
                .ForCondition(jToken != null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON document {0} to have element \"" + expected.Escape(true) + "\"{reason}" +
                          ", but no such element was found.", Subject);

            return new AndWhichConstraint<JTokenAssertions, JToken>(this, jToken);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> does not have a direct child element with the specified
        ///     <paramref name="unexpected" /> name.
        /// </summary>
        /// <param name="unexpected">The name of the not expected child element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndWhichConstraint<JTokenAssertions, JToken> NotHaveElement(string unexpected, string because = "",
            params object[] becauseArgs)
        {
            JToken jToken = Subject[unexpected];
            Execute.Assertion
                .ForCondition(jToken == null)
                .BecauseOf(because, becauseArgs)
                .FailWith("Did not expect JSON document {0} to have element \"" + unexpected.Escape(true) + "\"{reason}.", Subject);

            return new AndWhichConstraint<JTokenAssertions, JToken>(this, jToken);
        }

        /// <summary>
        /// Expects the current <see cref="JToken" /> to contain only a single item.
        /// </summary>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        public AndWhichConstraint<JTokenAssertions, JToken> ContainSingleItem(string because = "", params object[] becauseArgs)
        {
            var formatter = new JTokenFormatter();
            string formattedDocument = formatter.ToString(Subject).Replace("{", "{{").Replace("}", "}}");

            using (new AssertionScope("JSON document " + formattedDocument))
            {
                var constraint = EnumerableSubject.ContainSingle(because, becauseArgs);
                return new AndWhichConstraint<JTokenAssertions, JToken>(this, constraint.Which);
            }
        }

        /// <summary>
        /// Asserts that the number of items in the current <see cref="JToken" /> matches the supplied <paramref name="expected" /> amount.
        /// </summary>
        /// <param name="expected">The expected number of items in the current <see cref="JToken" />.</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> HaveCount(int expected, string because = "", params object[] becauseArgs)
        {
            var formatter = new JTokenFormatter();
            string formattedDocument = formatter.ToString(Subject).Replace("{", "{{").Replace("}", "}}");

            using (new AssertionScope("JSON document " + formattedDocument))
            {
                EnumerableSubject.HaveCount(expected, because, becauseArgs);
                return new AndConstraint<JTokenAssertions>(this);
            }
        }

        /// <summary>
        /// Recursively asserts that the current <see cref="JToken"/> contains at least the properties or elements of the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="subtree">The subtree to search for</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        /// <remarks>Use this method to match the current <see cref="JToken"/> against an arbitrary subtree, 
        /// permitting it to contain any additional properties or elements. This way we can test multiple properties on a <see cref="JObject"/> at once,
        /// or test if a <see cref="JArray"/> contains any items that match a set of properties, assert that a JSON document has a given shape, etc. </remarks>
        /// <example>
        /// This example asserts the values of multiple properties of a child object within a JSON document.
        /// <code>
        /// var json = JToken.Parse("{ success: true, data: { id: 123, type: 'my-type', name: 'Noone' } }");
        /// json.Should().ContainSubtree(JToken.Parse("{ success: true, data: { type: 'my-type', name: 'Noone' } }"));
        /// </code>
        /// </example>
        /// <example>This example asserts that a <see cref="JArray"/> within a <see cref="JObject"/> has at least one element with at least the given properties</example>
        /// <code>
        /// var json = JToken.Parse("{ id: 1, items: [ { id: 2, type: 'my-type', name: 'Alpha' }, { id: 3, type: 'other-type', name: 'Bravo' } ] }");
        /// json.Should().ContainSubtree(JToken.Parse("{ items: [ { type: 'my-type', name: 'Alpha' } ] }"));
        /// </code>
        public AndConstraint<JTokenAssertions> ContainSubtree(JToken subtree, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(JTokenContainsSubtree(Subject, subtree))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON document to contain subtree {0} {reason}, but some elements were missing.", subtree); // todo: report exact cause of failure, eg. name of the missing property, etc.

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        /// Recursively asserts that the current <see cref="JToken"/> contains at least the properties or elements of the specified <see cref="JToken"/>.
        /// </summary>
        /// <param name="subtree">The subtree to search for</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        /// <remarks>Use this method to match the current <see cref="JToken"/> against an arbitrary subtree, 
        /// permitting it to contain any additional properties or elements. This way we can test multiple properties on a <see cref="JObject"/> at once,
        /// or test if a <see cref="JArray"/> contains any items that match a set of properties, assert that a JSON document has a given shape, etc. </remarks>
        /// <example>
        /// This example asserts the values of multiple properties of a child object within a JSON document.
        /// <code>
        /// var json = JToken.Parse("{ success: true, data: { id: 123, type: 'my-type', name: 'Noone' } }");
        /// json.Should().ContainSubtree(JToken.Parse("{ success: true, data: { type: 'my-type', name: 'Noone' } }"));
        /// </code>
        /// </example>
        /// <example>This example asserts that a <see cref="JArray"/> within a <see cref="JObject"/> has at least one element with at least the given properties</example>
        /// <code>
        /// var json = JToken.Parse("{ id: 1, items: [ { id: 2, type: 'my-type', name: 'Alpha' }, { id: 3, type: 'other-type', name: 'Bravo' } ] }");
        /// json.Should().ContainSubtree(JToken.Parse("{ items: [ { type: 'my-type', name: 'Alpha' } ] }"));
        /// </code>
        public AndConstraint<JTokenAssertions> ContainSubtree(string subtree, string because = "", params object[] becauseArgs)
        {
            return ContainSubtree(JToken.Parse(subtree), because, becauseArgs);
        }

        private bool JTokenContainsSubtree(JToken token, JToken subtree)
        {
            switch (subtree.Type)
            {
                case JTokenType.Object:
                {
                    var sub = (JObject)subtree;
                    var obj = token as JObject;
                    if (obj == null)
                        return false;
                    foreach (var subProp in sub.Properties())
                    {
                        var prop = obj.Property(subProp.Name);
                        if (prop == null)
                            return false;
                        if (!JTokenContainsSubtree(prop.Value, subProp.Value))
                            return false;
                    }
                    return true;
                }
                case JTokenType.Array:
                {
                    var sub = (JArray)subtree;
                    var arr = token as JArray;
                    if (arr == null)
                        return false;
                    foreach (var subItem in sub)
                    {
                        if (!arr.Any(item => JTokenContainsSubtree(item, subItem)))
                            return false;
                    }
                    return true;
                }
                default:
                    return JToken.DeepEquals(token, subtree);

            }
        }
    }
}