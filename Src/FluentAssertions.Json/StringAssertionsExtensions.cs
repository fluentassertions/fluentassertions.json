using System;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    public static class StringAssertionsExtensions
    {
        [CustomAssertionAttribute]
        public static AndWhichConstraint<StringAssertions, JToken> BeValidJson(
            this StringAssertions stringAssertions,
            string because = "",
            params object[] becauseArgs)
        {
            JToken json = null;

            try
            {
                json = JToken.Parse(stringAssertions.Subject);
            }
#pragma warning disable CA1031 // Ignore catching general exception
            catch (Exception ex)
#pragma warning restore CA1031 // Ignore catching general exception
            {
                Execute.Assertion.BecauseOf(because, becauseArgs)
                    .FailWith("Expected {context:string} to be valid JSON{reason}, but parsing failed with {0}.", ex.Message);
            }

            return new AndWhichConstraint<StringAssertions, JToken>(stringAssertions, json);
        }
    }
}
