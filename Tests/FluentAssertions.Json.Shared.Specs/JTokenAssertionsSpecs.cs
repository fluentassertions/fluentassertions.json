using System;
using System.Collections.Generic;
using FluentAssertions.Formatting;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Json
{
    // ReSharper disable InconsistentNaming
    public class JTokenAssertionsSpecs
    {
        #region (Not)Be

        [Fact]
        public void When_both_objects_are_null_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            JToken actual = null;
            JToken expected = null;

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().Be(expected);
        }

        [Fact]
        public void When_both_objects_are_the_same_or_equal_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            string json = @"
                {
                    friends:
                    [{
                            id: 123,
                            name: ""John Doe""
                        }, {
                            id: 456,
                            name: ""Jane Doe"",
                            kids:
                            [
                                ""Jimmy"",
                                ""James""
                            ]
                        }
                    ]
                }
                ";
            
            var a = JToken.Parse(json);
            var b = JToken.Parse(json);

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            a.Should().Be(a);
            b.Should().Be(b);
            a.Should().Be(b);
        }

        [Fact]
        public void When_objects_differ_Be_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new []
            {
                Tuple.Create(
                    (string)null,
                    "{ id: 2 }",
                    "it is null")
                ,
                Tuple.Create(
                    "{ id: 1 }",
                    (string)null,
                    "it is not null")
                ,
                Tuple.Create(
                    "{ items: [] }",
                    "{ items: 2 }",
                    "the type is different at $.items")
                ,
                Tuple.Create(
                    "{ items: [ \"fork\", \"knife\" , \"spoon\" ]}",
                    "{ items: [ \"fork\", \"knife\" ]}",
                    "the length is different at $.items")
                ,
                Tuple.Create(
                    "{ items: [ \"fork\", \"knife\" , \"spoon\" ]}",
                    "{ items: [ \"fork\", \"knife\" ]}",
                    "the length is different at $.items")
                ,
                Tuple.Create(
                    "{ items: [ \"fork\", \"knife\" , \"spoon\" ]}",
                    "{ items: [ \"fork\", \"spoon\", \"knife\" ]}",
                    "the value is different at $.items[1]")
                ,
                Tuple.Create(
                    "{ tree: { } }",
                    "{ tree: \"oak\" }",
                    "the type is different at $.tree")
                ,
                Tuple.Create(
                    "{ tree: { leaves: 10} }",
                    "{ tree: { branches: 5, leaves: 10 } }",
                    "it misses property $.tree.branches")
                ,
                Tuple.Create(
                    "{ tree: { branches: 5, leaves: 10 } }",
                    "{ tree: { leaves: 10} }",
                    "it has extra property $.tree.branches")
                ,
                Tuple.Create(
                    "{ tree: { leaves: 5 } }",
                    "{ tree: { leaves: 10} }",
                    "the value is different at $.tree.leaves")
                ,
                Tuple.Create(
                    "{ eyes: \"blue\" }",
                    "{ eyes: [] }",
                    "the type is different at $.eyes")
                ,
                Tuple.Create(
                    "{ eyes: \"blue\" }",
                    "{ eyes: 2 }",
                    "the type is different at $.eyes")
                ,
                Tuple.Create(
                    "{ id: 1 }",
                    "{ id: 2 }",
                    "the value is different at $.id")
            };

            foreach (var testCase in testCases)
            {
                string actualJson = testCase.Item1;
                string expectedJson = testCase.Item2;
                string expectedDifference = testCase.Item3;

                var actual = (actualJson != null) ? JToken.Parse(actualJson) : null;
                var expected = (expectedJson != null) ? JToken.Parse(expectedJson) : null;

                var expectedMessage =
                    $"Expected JSON document {_formatter.ToString(actual, true)} to be {_formatter.ToString(expected, true)}, " +
                    "but " + expectedDifference + ".";

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                actual.Should().Invoking(x => x.Be(expected))
                    .ShouldThrow<XunitException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void When_properties_differ_Be_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new []
            {
                Tuple.Create<JToken, JToken, string>(
                    new JProperty("eyes", "blue"),
                    new JArray(),
                    "the type is different at $")
                ,
                Tuple.Create<JToken, JToken, string>(
                    new JProperty("eyes", "blue"),
                    new JProperty("hair", "black"),
                    "the name is different at $")
                ,
            };

            foreach (var testCase in testCases)
            {
                var a = testCase.Item1;
                var b = testCase.Item2;
                
                var expectedMessage =
                    $"Expected JSON document {_formatter.ToString(a, true)} to be {_formatter.ToString(b, true)}, " +
                    "but " + testCase.Item3 + ".";

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                a.Should().Invoking(x => x.Be(b))
                    .ShouldThrow<XunitException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void When_both_properties_are_null_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var actual = JToken.Parse("{ \"id\": null }");
            var expected = JToken.Parse("{ \"id\": null }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().Be(expected);
        }

        [Fact]
        public void When_arrays_are_equal_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new []
            {
                Tuple.Create(
                    new JArray(1, 2, 3),
                    new JArray(1, 2, 3))
                ,
                Tuple.Create(
                    new JArray("blue", "green"),
                    new JArray("blue", "green"))
                ,
                Tuple.Create(
                    new JArray(JToken.Parse("{ car: { color: \"blue\" }}"), JToken.Parse("{ flower: { color: \"red\" }}")),
                    new JArray(JToken.Parse("{ car: { color: \"blue\" }}"), JToken.Parse("{ flower: { color: \"red\" }}")))
            };

            foreach (var testCase in testCases)
            {
                var actual = testCase.Item1;
                var expected = testCase.Item2;
                
                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                actual.Should().Be(expected);
            }
        }

        [Fact]
        public void When_only_the_order_of_properties_differ_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new Dictionary<string, string>
            {
                {
                    "{ friends: [{ id: 123, name: \"Corby Page\" }, { id: 456, name: \"Carter Page\" }] }",
                    "{ friends: [{ name: \"Corby Page\", id: 123 }, { id: 456, name: \"Carter Page\" }] }"
                },
                {
                    "{ id: 2, admin: true }",
                    "{ admin: true, id: 2}"
                }
            };

            foreach (var testCase in testCases)
            {
                var actualJson = testCase.Key;
                var expectedJson = testCase.Value;
                var a = JToken.Parse(actualJson);
                var b = JToken.Parse(expectedJson);

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                a.Should().Be(b);
            }
        }

        [Fact]
        public void When_specifying_a_reason_why_object_should_be_equal_it_should_use_that_in_the_error_message()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ child: { subject: 'foo' } }");
            var expected = JToken.Parse("{ child: { expected: 'bar' } }");

            var expectedMessage =
                $"Expected JSON document {_formatter.ToString(subject, true)} to be {_formatter.ToString(expected, true)} " +
                "because we want to test the failure message, " +
                "but it misses property $.child.expected.";

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().Invoking(x => x.Be(expected, "we want to test the failure {0}", "message"))
                .ShouldThrow<XunitException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void When_objects_differ_NotBe_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var actual = JToken.Parse("{ \"id\": 1 }");
            var expected = JToken.Parse("{ \"id\": 2 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().NotBe(expected);
        }

        [Fact]
        public void When_objects_are_equal_NotBe_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var a = JToken.Parse("{ \"id\": 1 }");
            var b = JToken.Parse("{ \"id\": 1 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            a.Invoking(x => x.Should().NotBe(b))
                .ShouldThrow<XunitException>()
                .WithMessage($"Expected JSON document not to be {_formatter.ToString(b)}.");
        }

        #endregion (Not)Be

        #region (Not)HaveValue

        [Fact]
        public void When_jtoken_has_value_HaveValue_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().HaveValue("42");
        }

        [Fact]
        public void When_jtoken_not_has_value_HaveValue_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().Invoking(x => x.HaveValue("43", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage("Expected JSON property \"id\" to have value \"43\" because foo, but found \"42\".");
        }

        [Fact]
        public void When_jtoken_does_not_have_value_NotHaveValue_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 43 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().NotHaveValue("42");
        }

        [Fact]
        public void When_jtoken_does_have_value_NotHaveValue_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().Invoking(x => x.NotHaveValue("42", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage("Did not expect JSON property \"id\" to have value \"42\" because foo.");
        }

        #endregion (Not)HaveValue

        #region (Not)HaveElement

        [Fact]
        public void When_jtoken_has_element_HaveElement_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().HaveElement("id");
        }

        [Fact]
        public void When_jtoken_not_has_element_HaveElement_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().Invoking(x => x.HaveElement("name", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document {Format(subject)} to have element \"name\" because foo, but no such element was found.");
        }

        [Fact]
        public void When_jtoken_does_not_have_element_NotHaveElement_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().NotHaveElement("name");
        }

        [Fact]
        public void When_jtoken_does_have_element_NotHaveElement_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().Invoking(x => x.NotHaveElement("id", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage($"Did not expect JSON document {Format(subject)} to have element \"id\" because foo.");
        }

        #endregion (Not)HaveElement

        #region ContainSingleItem

        [Fact]
        public void When_jtoken_has_a_single_element_ContainSingleItem_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ id: 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_jtoken_has_a_single_element_ContainSingleItem_should_return_which_element_it_is()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ id: 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            var element = subject.Should().ContainSingleItem().Which;

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            element.Should().Be(new JProperty("id", 42));
        }

        [Fact]
        public void When_jtoken_is_null_ContainSingleItem_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            JToken subject = null;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem("null is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document <null> to contain a single item because null is not allowed, but found <null>.");
        }

        [Fact]
        public void When_jtoken_is_an_empty_object_ContainSingleItem_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem("less is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document * to contain a single item because less is not allowed, but the collection is empty.");
        }

        [Fact]
        public void When_jtoken_has_multiple_elements_ContainSingleItem_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ id: 42, admin: true }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem("more is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            string formattedSubject = Format(subject);

            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document*id*42*admin*true*to contain a single item because more is not allowed, but found*");
        }

        [Fact]
        public void When_jtoken_is_array_with_a_single_item_ContainSingleItem_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[{ id: 42 }]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem();

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_jtoken_is_an_array_with_a_single_item_ContainSingleItem_should_return_which_element_it_is()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[{ id: 42 }]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            var element = subject.Should().ContainSingleItem().Which;

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            element.Should().Be(JToken.Parse("{ id: 42 }"));
        }

        [Fact]
        public void When_jtoken_is_an_empty_array_ContainSingleItem_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem("less is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document [] to contain a single item because less is not allowed, but the collection is empty.");
        }

        [Fact]
        public void When_jtoken_is_an_array_with_multiple_items_ContainSingleItem_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[1, 2]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSingleItem("more is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            string formattedSubject = Format(subject);

            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document {formattedSubject} to contain a single item because more is not allowed, but found {formattedSubject}.");
        }

        #endregion ContainSingleItem

        #region HaveCount

        [Fact]
        public void When_expecting_the_actual_number_of_elements_HaveCount_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ id: 42, admin: true }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().HaveCount(2);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_expecting_the_actual_number_of_elements_HaveCount_should_enable_consecutive_assertions()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ id: 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            JTokenAssertions and = subject.Should().HaveCount(1).And;

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            and.Be(subject);
        }

        [Fact]
        public void When_jtoken_is_null_HaveCount_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            JToken subject = null;

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().HaveCount(1, "null is not allowed");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document <null> to contain 1 item(s) because null is not allowed, but found <null>.");
        }

        [Fact]
        public void When_expecting_a_different_number_of_elements_than_the_actual_number_HaveCount_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ }");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().HaveCount(1, "numbers matter");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document * to contain 1 item(s) because numbers matter, but found 0.");
        }

        [Fact]
        public void When_expecting_the_actual_number_of_array_items_HaveCount_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[ 'Hello', 'World!' ]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().HaveCount(2);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_expecting_a_different_number_of_array_items_than_the_actual_number_HaveCount_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("[ 'Hello', 'World!' ]");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().HaveCount(3, "the more the better");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document * to contain 3 item(s) because the more the better, but found 2.");
        }

        #endregion HaveCount
        
        public static string Format(JToken value, bool useLineBreaks = false)
        {
            return new JTokenFormatter().Format(value, new FormattingContext
            {
                UseLineBreaks = useLineBreaks
            }, null);
        }

    }
}