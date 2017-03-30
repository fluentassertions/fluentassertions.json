using System;
using FluentAssertions.Equivalency;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json;

namespace FluentAssertions.Json
{
    /// <summary>
    ///     Contains extension methods for JSON serialization assertion methods
    /// </summary>
    public static class ObjectAssertionsExtensions
    {
        /// <summary>
        /// Asserts that an object can be serialized and deserialized using the JSON serializer and that it stills retains
        /// the values of all members.
        /// </summary>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        public static AndConstraint<ObjectAssertions> BeJsonSerializable<T>(this ObjectAssertions assertions, string because = "",
            params object[] becauseArgs)
        {
            return BeJsonSerializable<T>(assertions, options => options, because, becauseArgs);
        }

        /// <summary>
        /// Asserts that an object can be serialized and deserialized using the JSON serializer and that it stills retains
        /// the values of all members.
        /// </summary>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="because" />.
        /// </param>
        public static AndConstraint<ObjectAssertions> BeJsonSerializable<T>(this ObjectAssertions assertions,
            Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> options, string because = "",
            params object[] becauseArgs)
        {
            try
            {
                object deserializedObject = CreateCloneUsingJsonSerializer<T>(assertions.Subject);

                EquivalencyAssertionOptions<T> defaultOptions = AssertionOptions.CloneDefaults<T>()
                    .RespectingRuntimeTypes().IncludingFields().IncludingProperties();

                ((T)deserializedObject).ShouldBeEquivalentTo(assertions.Subject, _ => options(defaultOptions));
            }
            catch (Exception exc)
            {
                Execute.Assertion
                    .BecauseOf(because, becauseArgs)
                    .FailWith("Expected {0} to be JSON serializable{reason}, but serialization failed with:\r\n\r\n{1}",
                        assertions.Subject,
                        exc.Message);
            }

            return new AndConstraint<ObjectAssertions>(assertions);
        }

        private static object CreateCloneUsingJsonSerializer<T>(object subject)
        {
            var serializedObject = JsonConvert.SerializeObject(subject);
            var cloneUsingJsonSerializer = JsonConvert.DeserializeObject<T>(serializedObject);
            return cloneUsingJsonSerializer; ;
        }
    }
}
