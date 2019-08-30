using System;
using System.Collections.Generic;
using FluentAssertions.Formatting;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Json
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable ExpressionIsAlwaysNull
    public class JTokenAssertionsSpecs
    {
        #region (Not)BeEquivalentTo

        [Fact]
        public void When_both_objects_are_null_BeEquivalentTo_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            JToken actual = null;
            JToken expected = null;

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void When_both_objects_are_the_same_or_equal_BeEquivalentTo_should_succeed()
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
            a.Should().BeEquivalentTo(a);
            b.Should().BeEquivalentTo(b);
            a.Should().BeEquivalentTo(b);
        }

        [Fact]
        public void When_objects_differ_BeEquivalentTo_should_fail()
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
                    $"Expected JSON document {Format(actual, true)} " +
                    $"to be equivalent to {Format(expected, true)}, " +
                    "but " + expectedDifference + ".";

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                actual.Should().Invoking(x => x.BeEquivalentTo(expected))
                    .Should().Throw<XunitException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void When_properties_differ_BeEquivalentTo_should_fail()
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
                    $"Expected JSON document {Format(a, true)} " +
                    $"to be equivalent to {Format(b, true)}, " +
                    "but " + testCase.Item3 + ".";

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                a.Should().Invoking(x => x.BeEquivalentTo(b))
                    .Should().Throw<XunitException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void When_both_properties_are_null_BeEquivalentTo_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var actual = JToken.Parse("{ \"id\": null }");
            var expected = JToken.Parse("{ \"id\": null }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void When_arrays_are_equal_BeEquivalentTo_should_succeed()
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
                actual.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void When_only_the_order_of_properties_differ_BeEquivalentTo_should_succeed()
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
                a.Should().BeEquivalentTo(b);
            }
        }

        [Fact]
        public void When_checking_whether_a_JToken_is_equivalent_to_the_string_representation_of_that_token_it_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            string jsonString = @"
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
            
            var actualJSON = JToken.Parse(jsonString);

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actualJSON.Should().BeEquivalentTo(jsonString);
        }
        
        [Fact]
        public void When_specifying_a_reason_why_object_should_be_equivalent_it_should_use_that_in_the_error_message()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ child: { subject: 'foo' } }");
            var expected = JToken.Parse("{ child: { expected: 'bar' } }");

            var expectedMessage =
                $"Expected JSON document {Format(subject, true)} " +
                $"to be equivalent to {Format(expected, true)} " +
                "because we want to test the failure message, " +
                "but it misses property $.child.expected.";

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().Invoking(x => x.BeEquivalentTo(expected, "we want to test the failure {0}", "message"))
                .Should().Throw<XunitException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void When_objects_differ_NotBeEquivalentTo_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var actual = JToken.Parse("{ \"id\": 1 }");
            var expected = JToken.Parse("{ \"id\": 2 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            actual.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public void When_objects_are_equal_NotBeEquivalentTo_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var a = JToken.Parse("{ \"id\": 1 }");
            var b = JToken.Parse("{ \"id\": 1 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            a.Invoking(x => x.Should().NotBeEquivalentTo(b))
                .Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document not to be equivalent to {Format(b)}.");
        }

        [Fact]
        public void When_checking_whether_a_JToken_is_not_equivalent_to_the_string_representation_of_that_token_it_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            string jsonString = "{ \"id\": 1 }";
            var actualJson = JToken.Parse(jsonString);

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action action = () => actualJson.Should().NotBeEquivalentTo(jsonString);

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            action.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document not to be equivalent*");
        }
        
        #endregion (Not)BeEquivalentTo

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

        #region (Not)MatchRegex

        [Fact]
        public void When_json_matches_regex_pattern_MatchRegex_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().MatchRegex("\\d{2}");
        }

        [Fact]
        public void When_json_does_not_match_regex_pattern_MatchRegex_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 'not two digits' }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().Invoking(x => x.MatchRegex("\\d{2}", "because foo"))
                 .Should().Throw<XunitException>()
                 .WithMessage("Expected JSON property \"id\" to match regex pattern \"\\d{2}\" because foo, but found \"not two digits\".");
        }

        [Fact]
        public void When_json_does_not_match_regex_pattern_NotHaveRegexValue_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 'not two digits' }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().NotMatchRegex("\\d{2}");
        }

        [Fact]
        public void When_json_matches_regex_pattern_NotHaveRegexValue_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ 'id': 42 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject["id"].Should().Invoking(x => x.NotMatchRegex("\\d{2}", "because foo"))
                 .Should().Throw<XunitException>()
                 .WithMessage("Did not expect JSON property \"id\" to match regex pattern \"\\d{2}\" because foo.");
        }

        #endregion (Not)MatchRegex

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
            element.Should().BeEquivalentTo(new JProperty("id", 42));
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
                .WithMessage("Expected JSON document <null> to contain a single item because null is not allowed, but found <null>.");
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
                .WithMessage("Expected JSON document * to contain a single item because less is not allowed, but the collection is empty.");
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
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document*id*42*admin*true*to contain a single item because more is not allowed, but found*");
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
            element.Should().BeEquivalentTo(JToken.Parse("{ id: 42 }"));
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
                .WithMessage("Expected JSON document [] to contain a single item because less is not allowed, but the collection is empty.");
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
            and.BeEquivalentTo(subject);
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
                .WithMessage("Expected JSON document <null> to contain 1 item(s) because null is not allowed, but found <null>.");
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
                .WithMessage("Expected JSON document * to contain 1 item(s) because numbers matter, but found 0.");
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
                .WithMessage("Expected JSON document * to contain 3 item(s) because the more the better, but found 2.");
        }

        #endregion HaveCount
        
        #region ContainSubtree

        [Fact]
        public void When_all_expected_subtree_properties_match_ContainSubtree_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', baz: 'baz'} ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { foo: 'foo', baz: 'baz' } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_subtree_properties_are_missing_ContainSubtree_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar' } ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { baz: 'baz' } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_deep_subtree_matches_ContainSubtree_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', child: { x: 1, y: 2, grandchild: { tag: 'abrakadabra' }  }} ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { child: { grandchild: { tag: 'abrakadabra' } } } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_deep_subtree_does_not_match_ContainSubtree_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', child: { x: 1, y: 2, grandchild: { tag: 'abrakadabra' }  }} ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { child: { grandchild: { tag: 'ooops' } } } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_array_elements_are_matching_ContainSubtree_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', items: [ { id: 1 }, { id: 2 }, { id: 3 } ] } ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { items: [ { id: 1 }, { id: 3 } ] } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().NotThrow();
        }

        [Fact]
        public void When_array_elements_are_missing_ContainSubtree_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', items: [ { id: 1 }, { id: 3 }, { id: 5 } ] } ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { items: [ { id: 1 }, { id: 2 } ] } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>();
        }

        [Fact]
        public void When_property_types_dont_match_ContainSubtree_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ foo: '1' } ");

            //-----------------------------------------------------------------------------------------------------------
            // Act
            //-----------------------------------------------------------------------------------------------------------
            Action act = () => subject.Should().ContainSubtree(" { foo: 1 } ");

            //-----------------------------------------------------------------------------------------------------------
            // Assert
            //-----------------------------------------------------------------------------------------------------------
            act.Should().Throw<XunitException>();
        }

        #endregion

        private static string Format(JToken value, bool useLineBreaks = false)
        {
            return new JTokenFormatter().Format(value, new FormattingContext
            {
                UseLineBreaks = useLineBreaks
            }, null);
        }
    }
}