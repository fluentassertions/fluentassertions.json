using System.Diagnostics;
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
        protected override string Context => nameof(JToken);

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is equivalent to the parsed <paramref name="expected" /> JSON,
        ///     using an equivalent of <see cref="JToken.DeepEquals(JToken, JToken)" />.
        /// </summary>
        /// <param name="expected">The string representation of the expected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> BeEquivalentTo(string expected, string because = "",
            params object[] becauseArgs)
        {
            JToken parsedExpected = JToken.Parse(expected);
            return BeEquivalentTo(parsedExpected, because, becauseArgs);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is equivalent to the <paramref name="expected" /> element,
        ///     using an equivalent of <see cref="JToken.DeepEquals(JToken, JToken)" />.
        /// </summary>
        /// <param name="expected">The expected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> BeEquivalentTo(JToken expected, string because = "",
            params object[] becauseArgs)
        {
            Difference difference = JTokenDifferentiator.FindFirstDifference(Subject, expected);
            JTokenFormatter formatter = new JTokenFormatter();

            var message = $"Expected JSON document {formatter.ToString(Subject, true).Replace("{", "{{").Replace("}", "}}")}" +
                          $" to be equivalent to {formatter.ToString(expected, true).Replace("{", "{{").Replace("}", "}}")}" +
                          $"{{reason}}, but {difference}.";
            
            Execute.Assertion
                .ForCondition(difference == null)
                .BecauseOf(because, becauseArgs)
                .FailWith(message);

            return new AndConstraint<JTokenAssertions>(this);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is not equivalent to the parsed <paramref name="unexpected" /> JSON,
        ///     using an equivalent of <see cref="JToken.DeepEquals(JToken, JToken)" />.
        /// </summary>
        /// <param name="unexpected">The string representation of the unexpected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> NotBeEquivalentTo(string unexpected, string because = "", params object[] becauseArgs)
        {
            JToken parsedExpected = JToken.Parse(unexpected);
            return NotBeEquivalentTo(parsedExpected, because, becauseArgs);
        }

        /// <summary>
        ///     Asserts that the current <see cref="JToken" /> is not equivalent to the <paramref name="unexpected" /> element,
        ///     using an equivalent of <see cref="JToken.DeepEquals(JToken, JToken)" />.
        /// </summary>
        /// <param name="unexpected">The unexpected element</param>
        /// <param name="because">
        ///     A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion
        ///     is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        ///     Zero or more objects to format using the placeholders in <see paramref="because" />.
        /// </param>
        public AndConstraint<JTokenAssertions> NotBeEquivalentTo(JToken unexpected, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition((ReferenceEquals(Subject, null) && !ReferenceEquals(unexpected, null)) ||
                              !JToken.DeepEquals(Subject, unexpected))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected JSON document not to be equivalent to {0}{reason}.", unexpected);

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
    }
}