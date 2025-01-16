using System;
using System.Collections.Generic;
using FluentAssertions.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Sdk;

namespace FluentAssertions.Json.Specs
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable ExpressionIsAlwaysNull
    public class JTokenAssertionsSpecs
    {
        #region (Not)BeEquivalentTo

        [Fact]
        public void When_both_tokens_are_null_they_should_be_treated_as_equivalent()
        {
            // Arrange
            JToken actual = null;
            JToken expected = null;

            // Act & Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void When_both_tokens_represent_the_same_json_content_they_should_be_treated_as_equivalent()
        {
            // Arrange
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

            // Act & Assert
            a.Should().BeEquivalentTo(a);
            b.Should().BeEquivalentTo(b);
            a.Should().BeEquivalentTo(b);
        }

        public static TheoryData<string, string, string> FailingBeEquivalentCases => new()
        {
            {
                null,
                "{ id: 2 }",
                "is null"
            },
            {
                "{ id: 1 }",
                null,
                "is not null"
            },
            {
                "{ items: [] }",
                "{ items: 2 }",
                "has an array instead of an integer at $.items"
            },
            {
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "{ items: [ \"fork\", \"knife\" ] }",
                "has 3 elements instead of 2 at $.items"
            },
            {
                "{ items: [ \"fork\", \"knife\" ] }",
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "has 2 elements instead of 3 at $.items"
            },
            {
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "{ items: [ \"fork\", \"spoon\", \"knife\" ] }",
                "has a different value at $.items[1]"
            },
            {
                "{ tree: { } }",
                "{ tree: \"oak\" }",
                "has an object instead of a string at $.tree"
            },
            {
                "{ tree: { leaves: 10} }",
                "{ tree: { branches: 5, leaves: 10 } }",
                "misses property $.tree.branches"
            },
            {
                "{ tree: { branches: 5, leaves: 10 } }",
                "{ tree: { leaves: 10} }",
                "has extra property $.tree.branches"
            },
            {
                "{ tree: { leaves: 5 } }",
                "{ tree: { leaves: 10} }",
                "has a different value at $.tree.leaves"
            },
            {
                "{ eyes: \"blue\" }",
                "{ eyes: [] }",
                "has a string instead of an array at $.eyes"
            },
            {
                "{ eyes: \"blue\" }",
                "{ eyes: 2 }",
                "has a string instead of an integer at $.eyes"
            },
            {
                "{ id: 1 }",
                "{ id: 2 }",
                "has a different value at $.id"
            }
        };

        [Theory]
        [MemberData(nameof(FailingBeEquivalentCases))]
        public void When_both_tokens_are_not_equivalent_it_should_throw_and_mention_the_difference(
            string actualJson, string expectedJson, string expectedDifference)
        {
            // Arrange
            var actual = (actualJson != null) ? JToken.Parse(actualJson) : null;
            var expected = (expectedJson != null) ? JToken.Parse(expectedJson) : null;

            var expectedMessage =
                $"JSON document {expectedDifference}." +
                "Actual document" +
                $"{Format(actual, true)}" +
                "was expected to be equivalent to" +
                $"{Format(expected, true)}.";

            // Act & Assert
            actual.Should().Invoking(x => x.BeEquivalentTo(expected))
                .Should().Throw<XunitException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void When_properties_differ_between_two_tokens_it_should_not_treat_them_as_equivalent()
        {
            // Arrange
            var testCases = new[]
            {
                Tuple.Create<JToken, JToken, string>(
                    new JProperty("eyes", "blue"),
                    new JArray(),
                    "has a property instead of an array at $")
                ,
                Tuple.Create<JToken, JToken, string>(
                    new JProperty("eyes", "blue"),
                    new JProperty("hair", "black"),
                    "has a different name at $")
                ,
            };

            foreach (var testCase in testCases)
            {
                var actual = testCase.Item1;
                var expected = testCase.Item2;
                var expectedDifference = testCase.Item3;

                var expectedMessage =
                    $"JSON document {expectedDifference}." +
                    "Actual document" +
                    $"{Format(actual, true)}" +
                    "was expected to be equivalent to" +
                    $"{Format(expected, true)}.";

                // Act & Assert
                actual.Should().Invoking(x => x.BeEquivalentTo(expected))
                    .Should().Throw<XunitException>()
                    .WithMessage(expectedMessage);
            }
        }

        [Fact]
        public void When_both_property_values_are_null_it_should_treat_them_as_equivalent()
        {
            // Arrange
            var actual = JToken.Parse("{ \"id\": null }");
            var expected = JToken.Parse("{ \"id\": null }");

            // Act & Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void When_two_json_arrays_have_the_same_properties_in_the_same_order_they_should_be_treated_as_equivalent()
        {
            // Arrange
            var testCases = new[]
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

                // Act & Assert
                actual.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void When_only_the_order_of_properties_differ_they_should_be_treated_as_equivalent()
        {
            // Arrange
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

                // Act & Assert
                a.Should().BeEquivalentTo(b);
            }
        }

        [Fact]
        public void When_a_token_is_compared_to_its_string_representation_they_should_be_treated_as_equivalent()
        {
            // Arrange
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

            // Act & Assert
            actualJSON.Should().BeEquivalentTo(jsonString);
        }

        [Fact]
        public void When_checking_non_equivalency_with_an_invalid_expected_string_it_should_provide_a_clear_error_message()
        {
            // Arrange
            var actualJson = JToken.Parse("{ \"id\": null }");
            var expectedString = "{ invalid JSON }";

            // Act & Assert
            actualJson.Should().Invoking(x => x.BeEquivalentTo(expectedString))
                .Should().Throw<ArgumentException>()
                .WithMessage($"Unable to parse expected JSON string:{expectedString}*")
                .WithInnerException<JsonReaderException>();
        }

        [Fact]
        public void When_checking_for_non_equivalency_with_an_unparseable_string_it_should_provide_a_clear_error_message()
        {
            // Arrange
            var actualJson = JToken.Parse("{ \"id\": null }");
            var unexpectedString = "{ invalid JSON }";

            // Act & Assert
            actualJson.Should().Invoking(x => x.NotBeEquivalentTo(unexpectedString))
                .Should().Throw<ArgumentException>()
                .WithMessage($"Unable to parse unexpected JSON string:{unexpectedString}*")
                .WithInnerException<JsonReaderException>();
        }

        [Fact]
        public void When_specifying_a_reason_why_a_token_should_be_equivalent_it_should_use_that_in_the_error_message()
        {
            // Arrange
            var subject = JToken.Parse("{ child: { subject: 'foo' } }");
            var expected = JToken.Parse("{ child: { expected: 'bar' } }");

            var expectedMessage =
                "JSON document misses property $.child.expected." +
                "Actual document" +
                $"{Format(subject, true)}" +
                "was expected to be equivalent to" +
                $"{Format(expected, true)} " +
                "because we want to test the failure message.";

            // Act & Assert
            subject.Should().Invoking(x => x.BeEquivalentTo(expected, "we want to test the failure {0}", "message"))
                .Should().Throw<XunitException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void When_property_values_differ_a_non_equivalency_check_should_succeed()
        {
            // Arrange
            var actual = JToken.Parse("{ \"id\": 1 }");
            var expected = JToken.Parse("{ \"id\": 2 }");

            // Act & Assert
            actual.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public void When_two_tokens_are_the_same_the_non_equivalency_check_should_throw()
        {
            // Arrange
            var a = JToken.Parse("{ \"id\": 1 }");
            var b = JToken.Parse("{ \"id\": 1 }");

            // Act & Assert
            a.Invoking(x => x.Should().NotBeEquivalentTo(b))
                .Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document not to be equivalent to {Format(b)}.");
        }

        [Fact]
        public void When_a_token_is_equal_to_its_string_representation_the_non_equivalency_check_should_throw()
        {
            // Arrange
            string jsonString = "{ \"id\": 1 }";
            var actualJson = JToken.Parse(jsonString);

            // Act
            Action action = () => actualJson.Should().NotBeEquivalentTo(jsonString);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document not to be equivalent*");
        }

        [Fact]
        public void When_a_float_is_within_approximation_check_should_succeed()
        {
            // Arrange
            var actual = JToken.Parse("{ \"id\": 1.1232 }");
            var expected = JToken.Parse("{ \"id\": 1.1235 }");

            // Act & Assert
            actual.Should().BeEquivalentTo(expected, options => options
                .Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, 1e-3))
                .WhenTypeIs<double>());
        }

        [Fact]
        public void When_a_float_is_not_within_approximation_check_should_throw()
        {
            // Arrange
            var actual = JToken.Parse("{ \"id\": 1.1232 }");
            var expected = JToken.Parse("{ \"id\": 1.1235 }");

            // Act & Assert
            actual.Should().
                Invoking(x => x.BeEquivalentTo(expected, options => options
                    .Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, 1e-5))
                    .WhenTypeIs<double>()))
                .Should().Throw<XunitException>()
                .WithMessage("JSON document has a different value at $.id.*");
        }

        [Fact]
        public void When_the_value_of_a_property_contains_curly_braces_the_equivalency_check_should_not_choke_on_them()
        {
            // Arrange
            var actual = JToken.Parse(@"{ ""{a1}"": {b: 1 }}");
            var expected = JToken.Parse(@"{ ""{a1}"": {b: 2 }}");

            // Act & Assert
            var expectedMessage =
                "JSON document has a different value at $.{a1}.b." +
                "Actual document" +
                $"{Format(actual, true)}" +
                "was expected to be equivalent to" +
                $"{Format(expected, true)}.";

            actual.Should().Invoking(x => x.BeEquivalentTo(expected))
                .Should().Throw<XunitException>()
                .WithMessage(expectedMessage);
        }

        #endregion (Not)BeEquivalentTo

        #region (Not)HaveValue

        [Fact]
        public void When_the_token_has_the_expected_value_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject["id"].Should().HaveValue("42");
        }

        [Fact]
        public void When_the_token_is_null_then_asserting_on_a_value_expectation_should_throw()
        {
            // Arrange
            JToken subject = null;

            // Act
            Action act = () => subject.Should().HaveValue("foo");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Expected*foo*was*null*");
        }

        [Fact]
        public void When_the_token_has_another_value_than_expected_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject["id"].Should().Invoking(x => x.HaveValue("43", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage("Expected JSON property \"id\" to have value \"43\" because foo, but found \"42\".");
        }

        [Fact]
        public void When_the_token_does_not_have_the_unexpected_value_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 43 }");

            // Act & Assert
            subject["id"].Should().NotHaveValue("42");
        }

        [Fact]
        public void When_the_token_is_null_assertions_on_not_having_a_value_should_throw()
        {
            // Arrange
            JToken subject = null;

            // Act
            Action act = () => subject.Should().NotHaveValue("foo");

            // Assert
            act.Should().Throw<XunitException>().WithMessage("Did not expect*foo*was*null*");
        }

        [Fact]
        public void When_the_token_has_a_value_that_it_was_not_supposed_to_have_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject["id"].Should().Invoking(x => x.NotHaveValue("42", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage("Did not expect JSON property \"id\" to have value \"42\" because foo.");
        }

        #endregion (Not)HaveValue

        #region (Not)MatchRegex

        [Fact]
        public void When_a_tokens_value_matches_the_regex_pattern_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject["id"].Should().MatchRegex("\\d{2}");
        }

        [Fact]
        public void When_a_tokens_value_does_not_match_the_regex_pattern_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 'not two digits' }");

            // Act & Assert
            subject["id"].Should().Invoking(x => x.MatchRegex("\\d{2}", "because foo"))
                 .Should().Throw<XunitException>()
                 .WithMessage("Expected JSON property \"id\" to match regex pattern \"\\d{2}\" because foo, but found \"not two digits\".");
        }

        [Fact]
        public void When_a_tokens_value_does_not_match_the_regex_pattern_and_that_is_expected_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 'not two digits' }");

            // Act & Assert
            subject["id"].Should().NotMatchRegex("\\d{2}");
        }

        [Fact]
        public void When_a_tokens_value_matches_the_regex_pattern_unexpectedly_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject["id"].Should().Invoking(x => x.NotMatchRegex("\\d{2}", "because foo"))
                 .Should().Throw<XunitException>()
                 .WithMessage("Did not expect JSON property \"id\" to match regex pattern \"\\d{2}\" because foo.");
        }

        #endregion (Not)MatchRegex

        #region (Not)HaveElement

        [Fact]
        public void When_the_token_has_a_property_with_the_specified_key_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject.Should().HaveElement("id");
        }

        [Fact]
        public void When_the_token_does_not_have_the_specified_property_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject.Should().Invoking(x => x.HaveElement("name", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document {Format(subject)} to have element \"name\" because foo, but no such element was found.");
        }

        [Fact]
        public void When_the_token_does_not_have_the_specified_element_and_that_was_expected_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject.Should().NotHaveElement("name");
        }

        [Fact]
        public void When_the_token_has_an_unexpected_element_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ 'id': 42 }");

            // Act & Assert
            subject.Should().Invoking(x => x.NotHaveElement("id", "because foo"))
                .Should().Throw<XunitException>()
                .WithMessage($"Did not expect JSON document {Format(subject)} to have element \"id\" because foo.");
        }

        #endregion (Not)HaveElement

        #region ContainSingleItem

        [Fact]
        public void When_the_token_has_a_single_child_and_that_was_expected_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ id: 42 }");

            // Act
            Action act = () => subject.Should().ContainSingleItem();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_the_token_has_a_single_child_it_should_return_that_child_for_chaining()
        {
            // Arrange
            var subject = JToken.Parse("{ id: 42 }");

            // Act
            var element = subject.Should().ContainSingleItem().Which;

            // Assert
            element.Should().BeEquivalentTo(new JProperty("id", 42));
        }

        [Fact]
        public void When_the_token_is_null_then_asserting_a_single_child_should_throw_with_a_clear_failure()
        {
            // Arrange
            JToken subject = null;

            // Act
            Action act = () => subject.Should().ContainSingleItem("null is not allowed");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document <null> to contain a single item because null is not allowed, but found <null>.");
        }

        [Fact]
        public void When_the_token_is_an_empty_object_then_the_assertion_on_a_single_item_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ }");

            // Act
            Action act = () => subject.Should().ContainSingleItem("less is not allowed");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document * to contain a single item because less is not allowed, but the collection is empty.");
        }

        [Fact]
        public void When_the_token_contains_multiple_properties_then_the_single_item_assertion_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ id: 42, admin: true }");

            // Act
            Action act = () => subject.Should().ContainSingleItem("more is not allowed");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document*id*42*admin*true*to contain a single item because more is not allowed, but found*");
        }

        [Fact]
        public void When_the_token_is_an_array_with_a_single_property_then_that_should_satisfy_the_single_item_assertion()
        {
            // Arrange
            var subject = JToken.Parse("[{ id: 42 }]");

            // Act
            Action act = () => subject.Should().ContainSingleItem();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_the_token_is_an_array_with_a_single_property_the_single_item_assertion_should_return_that_item_for_chaining()
        {
            // Arrange
            var subject = JToken.Parse("[{ id: 42 }]");

            // Act
            var element = subject.Should().ContainSingleItem().Which;

            // Assert
            element.Should().BeEquivalentTo(JToken.Parse("{ id: 42 }"));
        }

        [Fact]
        public void When_the_token_is_an_empty_array_then_an_assertion_for_a_single_item_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("[]");

            // Act
            Action act = () => subject.Should().ContainSingleItem("less is not allowed");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document [] to contain a single item because less is not allowed, but the collection is empty.");
        }

        [Fact]
        public void When_the_token_is_an_array_with_multiple_items_asserting_for_a_single_item_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("[1, 2]");

            // Act
            Action act = () => subject.Should().ContainSingleItem("more is not allowed");

            // Assert
            string formattedSubject = Format(subject);

            act.Should().Throw<XunitException>()
                .WithMessage($"Expected JSON document {formattedSubject} to contain a single item because more is not allowed, but found {formattedSubject}.");
        }

        #endregion ContainSingleItem

        #region HaveCount

        [Fact]
        public void When_the_number_of_items_match_the_expectation_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ id: 42, admin: true }");

            // Act
            Action act = () => subject.Should().HaveCount(2);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_the_number_of_items_match_the_expectation_it_should_allow_chaining_more_assertions()
        {
            // Arrange
            var subject = JToken.Parse("{ id: 42 }");

            // Act
            JTokenAssertions and = subject.Should().HaveCount(1).And;

            // Assert
            and.BeEquivalentTo(subject);
        }

        [Fact]
        public void When_the_token_is_null_then_an_assertion_on_the_count_should_throw()
        {
            // Arrange
            JToken subject = null;

            // Act
            Action act = () => subject.Should().HaveCount(1, "null is not allowed");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document <null> to contain 1 item(s) because null is not allowed, but found <null>.");
        }

        [Fact]
        public void When_expecting_a_different_number_of_elements_than_the_actual_number_it_should_throw()
        {
            // Arrange
            var subject = JToken.Parse("{ }");

            // Act
            Action act = () => subject.Should().HaveCount(1, "numbers matter");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document * to contain 1 item(s) because numbers matter, but found 0*");
        }

        [Fact]
        public void When_expecting_the_actual_number_of_array_items_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("[ 'Hello', 'World!' ]");

            // Act
            Action act = () => subject.Should().HaveCount(2);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_expecting_a_different_number_of_array_items_than_the_actual_number_it_should_fail()
        {
            // Arrange
            var subject = JToken.Parse("[ 'Hello', 'World!' ]");

            // Act
            Action act = () => subject.Should().HaveCount(3, "the more the better");

            // Assert
            act.Should().Throw<XunitException>()
                .WithMessage("Expected JSON document * to contain 3 item(s) because the more the better, but found 2*");
        }

        #endregion HaveCount

        #region ContainSubtree

        [Fact]
        public void When_all_expected_subtree_properties_match_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', baz: 'baz'} ");

            // Act
            Action act = () => subject.Should().ContainSubtree(" { foo: 'foo', baz: 'baz' } ");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_deep_subtree_matches_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', child: { x: 1, y: 2, grandchild: { tag: 'abrakadabra' }  }} ");

            // Act
            Action act = () => subject.Should().ContainSubtree(" { child: { grandchild: { tag: 'abrakadabra' } } } ");

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void When_array_elements_are_matching_within_a_nested_structure_it_should_succeed()
        {
            // Arrange
            var subject = JToken.Parse("{ foo: 'foo', bar: 'bar', items: [ { id: 1 }, { id: 2 }, { id: 3 } ] } ");

            // Act
            Action act = () => subject.Should().ContainSubtree(" { items: [ { id: 1 }, { id: 3 } ] } ");

            // Assert
            act.Should().NotThrow();
        }

        public static TheoryData<string, string, string> FailingContainSubtreeCases => new()
        {
            {
                null,
                "{ id: 2 }",
                "is null"
            },
            {
                "{ id: 1 }",
                null,
                "is not null"
            },
            {
                "{ foo: 'foo', bar: 'bar' }",
                "{ baz: 'baz' }",
                "misses property $.baz"
            },
            {
                "{ items: [] }",
                "{ items: 2 }",
                "has an array instead of an integer at $.items"
            },
            {
                "{ items: [ \"fork\", \"knife\" ] }",
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "misses expected element $.items[2]"
            },
            {
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "{ items: [ \"fork\", \"spoon\", \"knife\" ] }",
                "has expected element $.items[2] in the wrong order"
            },
            {
                "{ items: [ \"fork\", \"knife\" , \"spoon\" ] }",
                "{ items: [ \"fork\", \"fork\" ] }",
                "has a different value at $.items[1]"
            },
            {
                "{ tree: { } }",
                "{ tree: \"oak\" }",
                "has an object instead of a string at $.tree"
            },
            {
                "{ tree: { leaves: 10} }",
                "{ tree: { branches: 5, leaves: 10 } }",
                "misses property $.tree.branches"
            },
            {
                "{ tree: { leaves: 5 } }",
                "{ tree: { leaves: 10} }",
                "has a different value at $.tree.leaves"
            },
            {
                "{ eyes: \"blue\" }",
                "{ eyes: [] }",
                "has a string instead of an array at $.eyes"
            },
            {
                "{ eyes: \"blue\" }",
                "{ eyes: 2 }",
                "has a string instead of an integer at $.eyes"
            },
            {
                "{ id: 1 }",
                "{ id: 2 }",
                "has a different value at $.id"
            },
            {
                "{ items: [ { id: 1 }, { id: 3 }, { id: 5 } ] }",
                "{ items: [ { id: 1 }, { id: 2 } ] }",
                "has a different value at $.items[1].id"
            },
            {
                "{ foo: '1' }",
                "{ foo: 1 }",
                "has a string instead of an integer at $.foo"
            },
            {
                "{ foo: 'foo', bar: 'bar', child: { x: 1, y: 2, grandchild: { tag: 'abrakadabra' } } }",
                "{ child: { grandchild: { tag: 'ooops' } } }",
                "has a different value at $.child.grandchild.tag"
            }
        };

        [Theory]
        [MemberData(nameof(FailingContainSubtreeCases))]
        public void When_some_JSON_does_not_contain_all_elements_from_a_subtree_it_should_throw(
            string actualJson, string expectedJson, string expectedDifference)
        {
            // Arrange
            var actual = (actualJson != null) ? JToken.Parse(actualJson) : null;
            var expected = (expectedJson != null) ? JToken.Parse(expectedJson) : null;

            // Act
            Action action = () => actual.Should().ContainSubtree(expected);

            // Assert
            action.Should().Throw<XunitException>()
                .WithMessage(
                    $"JSON document {expectedDifference}.{Environment.NewLine}" +
                    $"Actual document{Environment.NewLine}" +
                    $"{Format(actual, true)}{Environment.NewLine}" +
                    $"was expected to contain{Environment.NewLine}" +
                    $"{Format(expected, true)}.{Environment.NewLine}");
        }

        [Fact]
        public void When_checking_subtree_with_an_invalid_expected_string_it_should_provide_a_clear_error_message()
        {
            // Arrange
            var actualJson = JToken.Parse("{ \"id\": null }");
            var invalidSubtree = "{ invalid JSON }";

            // Act & Assert
            actualJson.Should().Invoking(x => x.ContainSubtree(invalidSubtree))
                .Should().Throw<ArgumentException>()
                .WithMessage($"Unable to parse expected JSON string:{invalidSubtree}*")
                .WithInnerException<JsonReaderException>();
        }

        #endregion

        private static string Format(JToken value, bool useLineBreaks = false)
        {
            var output = new FormattedObjectGraph(100);

            new JTokenFormatter().Format(value, output, new FormattingContext
            {
                UseLineBreaks = useLineBreaks
            }, null);

            return output.ToString();
        }
    }
}
