using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Json.Specs
{
    public class WithoutStrictOrderingSpecs
    {
        [Theory]
        [MemberData(nameof(When_ignoring_ordering_BeEquivalentTo_should_succeed_sample_data))]
        public void When_ignoring_ordering_BeEquivalentTo_should_succeed(string subject, string expectation)
        {
            // Arrange
            var subjectJToken = JToken.Parse(subject);
            var expectationJToken = JToken.Parse(expectation);

            // Act
            subjectJToken.Should().BeEquivalentTo(expectationJToken, opt => opt.WithoutStrictOrdering());

            // Assert
        }

        public static IEnumerable<object[]> When_ignoring_ordering_BeEquivalentTo_should_succeed_sample_data()
        {
            yield return new object[] { @"{""ids"":[1,2,3]}", @"{""ids"":[3,2,1]}" };
            yield return new object[] { @"{""ids"":[1,2,3]}", @"{""ids"":[1,2,3]}" };
            yield return new object[] { @"{""type"":2,""name"":""b""}", @"{""name"":""b"",""type"":2}" };
            yield return new object[] { @"{""names"":[""a"",""b""]}", @"{""names"":[""b"",""a""]}" };
            yield return new object[]
            {
                @"{""vals"":[{""type"":1,""name"":""a""},{""name"":""b"",""type"":2}]}",
                @"{""vals"":[{""type"":2,""name"":""b""},{""name"":""a"",""type"":1}]}"
            };
            yield return new object[]
            {
                @"{""vals"":[{""type"":1,""name"":""a""},{""name"":""b"",""type"":2}]}",
                @"{""vals"":[{""name"":""a"",""type"":1},{""type"":2,""name"":""b""}]}"
            };
        }

        [Theory]
        [MemberData(nameof(When_not_ignoring_ordering_BeEquivalentTo_should_throw_sample_data))]
        public void When_not_ignoring_ordering_BeEquivalentTo_should_throw(string subject, string expectation)
        {
            // Arrange
            var subjectJToken = JToken.Parse(subject);
            var expectationJToken = JToken.Parse(expectation);

            // Act
            var action = new Func<AndConstraint<JTokenAssertions>>(() => subjectJToken.Should().BeEquivalentTo(expectationJToken));

            // Assert
            action.Should().Throw<XunitException>();
        }

        public static IEnumerable<object[]> When_not_ignoring_ordering_BeEquivalentTo_should_throw_sample_data()
        {
            yield return new object[] { @"{""ids"":[1,2,3]}", @"{""ids"":[3,2,1]}" };
            yield return new object[] { @"{""names"":[""a"",""b""]}", @"{""names"":[""b"",""a""]}" };
            yield return new object[]
            {
                @"{""vals"":[{""type"":1,""name"":""a""},{""name"":""b"",""type"":2}]}",
                @"{""vals"":[{""type"":2,""name"":""b""},{""name"":""a"",""type"":1}]}"
            };
        }
    }
}
