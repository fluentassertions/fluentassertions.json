using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using Xunit;

// NOTE that we are using both namespaces 'FluentAssertions' & 'FluentAssertions.Json' from an external namespace to force compiler disambiguation warnings
namespace SomeOtherNamespace
{
    // ReSharper disable InconsistentNaming
    public class JsonAssertionExtensionsSpecs
    {
        [Fact]
        public void Should_Provide_Unambiguos_JTokenAssertions()
        {
            // Arrange
            var assertions = new[]
            {
                JToken.Parse("{\"token\":\"value\"}").Should()
                , new JProperty("property","value").Should()
                , new JObject(new JProperty("object", "value")).Should()
                , new JArray(new[] { 42, 43 }).Should()
                , new JConstructor("property","value").Should()
                , new JValue("value").Should()
                , new JRaw("value").Should()
            };

            // Act & Assert
            foreach (var sut in assertions)
            {
                ((object)sut).Should().BeOfType<JTokenAssertions>("extensions should provide assertions for all JSon primitives, i.e. JObject, JToken and JProperty");
            }
        }
    }
}
