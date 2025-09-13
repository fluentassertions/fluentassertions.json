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
        [MemberData(nameof(Should_HandleJToken_WhenNeedToIgnoreOrdering_SampleData))]
        public void Should_HandleJToken_WhenNeedToIgnoreOrdering(string json1, string json2)
        {
            // Arrange
            var j1 = JToken.Parse(json1);
            var j2 = JToken.Parse(json2);

            // Act
            j1.Should().BeEquivalentTo(j2, opt => opt.WithoutStrictOrdering());

            // Assert
        }

        [Theory]
        [MemberData(nameof(Should_DoNotHandleJToken_WhenNoNeedToIgnoreOrdering_SampleData))]
        public void Should_DoNotHandleJToken_WhenNoNeedToIgnoreOrdering(string json1, string json2)
        {
            // Arrange
            var j1 = JToken.Parse(json1);
            var j2 = JToken.Parse(json2);

            // Act
            var action = new Func<AndConstraint<JTokenAssertions>>(() => j1.Should().BeEquivalentTo(j2));

            // Assert
            action.Should().Throw<XunitException>();
        }

        public static IEnumerable<object[]> Should_DoNotHandleJToken_WhenNoNeedToIgnoreOrdering_SampleData()
        {
            yield return new object[] { @"{""ids"":[1,2,3]}", @"{""ids"":[3,2,1]}" };
            yield return new object[] { @"{""names"":[""a"",""b""]}", @"{""names"":[""b"",""a""]}" };
            yield return new object[]
            {
                @"{""vals"":[{""type"":1,""name"":""a""},{""name"":""b"",""type"":2}]}",
                @"{""vals"":[{""type"":2,""name"":""b""},{""name"":""a"",""type"":1}]}"
            };
        }

        public static IEnumerable<object[]> Should_HandleJToken_WhenNeedToIgnoreOrdering_SampleData()
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
    }
}
