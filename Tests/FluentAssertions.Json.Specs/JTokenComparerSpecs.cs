using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FluentAssertions.Json.Specs
{
    public class JTokenComparerSpecs
    {
        private static readonly IComparer<JToken> Comparer =
            Type.GetType("FluentAssertions.Json.Common.JTokenExtensions, FluentAssertions.Json")!
                .GetField("Comparer", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
                .GetValue(null) as IComparer<JToken>;

        [Fact]
        public void Should_return_zero_for_same_reference()
        {
            // Arrange
            var token = JToken.Parse(@"{""a"":1}");

            // Act & Assert
            Comparer.Compare(token, token).Should().Be(0);
        }

        [Fact]
        public void Should_handle_nulls()
        {
            // Arrange
            var token = JToken.Parse("1");

            // Act & Assert
            Comparer.Compare(null, token).Should().Be(-1);
            Comparer.Compare(token, null).Should().Be(1);
            Comparer.Compare(null, null).Should().Be(0);
        }

        [Fact]
        public void Should_compare_different_types()
        {
            // Arrange
            var obj = JToken.Parse(@"{""a"":1}");
            var arr = JToken.Parse("[1]");

            // Act & Assert
            Comparer.Compare(obj, arr).Should().NotBe(0);
        }

        [Fact]
        public void Should_compare_jvalues()
        {
            // Arrange
            var v1 = new JValue(1);
            var v2 = new JValue(2);

            // Act & Assert
            Comparer.Compare(v1, v2).Should().Be(-1);
            Comparer.Compare(v2, v1).Should().Be(1);
            Comparer.Compare(v1, new JValue(1)).Should().Be(0);
        }

        [Fact]
        public void Should_compare_jarrays_by_count_and_elements()
        {
            // Arrange
            var arr1 = JArray.Parse("[1,2]");
            var arr2 = JArray.Parse("[1,2,3]");
            var arr3 = JArray.Parse("[1,3]");
            var arr4 = JArray.Parse("[1,2,3]");

            // Act & Assert
            Comparer.Compare(arr1, arr2).Should().Be(-1);
            Comparer.Compare(arr1, arr3).Should().Be(-1);
            Comparer.Compare(arr3, arr1).Should().Be(1);
            Comparer.Compare(arr2, arr4).Should().Be(0);
        }

        [Fact]
        public void Should_compare_jobjects_by_count_and_properties()
        {
            // Arrange
            var obj1 = JObject.Parse(@"{""a"":1}");
            var obj2 = JObject.Parse(@"{""a"":1,""b"":2}");
            var obj3 = JObject.Parse(@"{""a"":2}");
            var obj4 = JObject.Parse(@"{""a"":1,""b"":2}");
            var obj5 = JObject.Parse(@"{""b"":2}");

            // Act & Assert
            Comparer.Compare(obj1, obj2).Should().Be(-1);
            Comparer.Compare(obj1, obj3).Should().Be(-1);
            Comparer.Compare(obj3, obj1).Should().Be(1);
            Comparer.Compare(obj2, obj4).Should().Be(0);
            Comparer.Compare(obj1, obj5).Should().Be(-1);
            Comparer.Compare(obj5, obj1).Should().Be(1);
        }

        [Fact]
        public void Should_compare_jproperties_by_name_and_value()
        {
            // Arrange
            var prop1 = new JProperty("a", 1);
            var prop2 = new JProperty("b", 1);
            var prop3 = new JProperty("a", 2);
            var prop4 = new JProperty("a", 1);

            // Act & Assert
            Comparer.Compare(prop1, prop2).Should().Be(-1);
            Comparer.Compare(prop1, prop3).Should().Be(-1);
            Comparer.Compare(prop3, prop1).Should().Be(1);
            Comparer.Compare(prop4, prop1).Should().Be(0);
            Comparer.Compare(prop2, prop3).Should().Be(1);
            Comparer.Compare(prop3, prop2).Should().Be(-1);
        }
    }
}
