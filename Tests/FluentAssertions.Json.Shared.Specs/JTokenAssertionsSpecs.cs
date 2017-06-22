using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    [TestClass]
    // ReSharper disable InconsistentNaming
    public class JTokenAssertionsSpecs
    {

        private static readonly JTokenFormatter _formatter = new JTokenFormatter();

        #region (Not)Be

        [TestMethod]
        public void When_both_values_are_the_same_or_equal_Be_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var a = JToken.Parse("{ \"id\": 1 }");
            var b = JToken.Parse("{ \"id\": 1 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            a.Should().Be(a);
            b.Should().Be(b);
            a.Should().Be(b);
        }

        [TestMethod]
        public void When_values_differ_Be_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new Dictionary<string, string>
            {
                {
                    "{ id: 1 }",
                    "{ id: 2 }"
                },
                {
                    "{ id: 1, admin: true }",
                    "{ id: 1, admin: false }"
                }
            };

            foreach (var testCase in testCases)
            {
                var actualJson = testCase.Key;
                var expectedJson = testCase.Value;

                var a = JToken.Parse(actualJson);
                var b = JToken.Parse(expectedJson);

                var expectedMessage =
                    $"Expected JSON document to be {_formatter.ToString(b)}, but found {_formatter.ToString(a)}.";

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                a.Should().Invoking(x => x.Be(b))
                    .ShouldThrow<AssertFailedException>()
                    .WithMessage(expectedMessage);
            }
        }

        [TestMethod]
        public void When_values_differ_NotBe_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var a = JToken.Parse("{ \"id\": 1 }");
            var b = JToken.Parse("{ \"id\": 2 }");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            a.Should().NotBeNull();
            a.Should().NotBe(null);
            a.Should().NotBe(b);
        }

        [TestMethod]
        public void When_values_are_equal_or_equivalent_NotBe_should_fail()
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
                .ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document not to be {_formatter.ToString(b)}.");
        }

        #endregion (Not)Be

        #region (Not)BeEquivalentTo

        [TestMethod]
        public void When_both_values_are_equal_BeEquivalentTo_should_succeed()
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
                    "{ admin: true, id: 2}" }
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

        [TestMethod]
        public void When_values_differ_BeEquivalentTo_should_fail()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new Dictionary<string, string>
            {
                {
                    "{ id: 1, admin: true }",
                    "{ id: 1, admin: false }"
                }
            };

            foreach (var testCase in testCases)
            {
                var actualJson = testCase.Key;
                var expectedJson = testCase.Value;

                var a = JToken.Parse(actualJson);
                var b = JToken.Parse(expectedJson);

                var expectedMessage = GetNotEquivalentMessage(a, b);

                //-----------------------------------------------------------------------------------------------------------
                // Act & Assert
                //-----------------------------------------------------------------------------------------------------------
                a.Should().Invoking(x => x.BeEquivalentTo(b))
                    .ShouldThrow<AssertFailedException>()
                    .WithMessage(expectedMessage);
            }
        }

        [TestMethod]
        public void When_values_differ_NotBeEquivalentTo_should_succeed()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var testCases = new Dictionary<string, string>
            {
                {
                    "{ id: 1, admin: true }",
                    "{ id: 1, admin: false }"
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
                a.Should().NotBeEquivalentTo(b);
            }
        }

        [TestMethod]
        public void Fail_with_descriptive_message_when_child_element_differs()
        {
            //-----------------------------------------------------------------------------------------------------------
            // Arrange
            //-----------------------------------------------------------------------------------------------------------
            var subject = JToken.Parse("{ child: { subject: 'foo' } }");
            var expected = JToken.Parse("{ child: { expected: 'bar' } }");

            var expectedMessage = GetNotEquivalentMessage(subject, expected, "we want to test the failure {0}", "message");

            //-----------------------------------------------------------------------------------------------------------
            // Act & Assert
            //-----------------------------------------------------------------------------------------------------------
            subject.Should().Invoking(x => x.BeEquivalentTo(expected, "we want to test the failure {0}", "message"))
                .ShouldThrow<AssertFailedException>()
                .WithMessage(expectedMessage);
        }

        private static string GetNotEquivalentMessage(JToken actual, JToken expected,
            string reason = null, params object[] reasonArgs)
        {
            var diff = ObjectDiffPatch.GenerateDiff(actual, expected);
            var key = diff.NewValues?.First ?? diff.OldValues?.First;

            var because = string.Empty;
            if (!string.IsNullOrWhiteSpace(reason))
                because = " because " + string.Format(reason, reasonArgs);

            var expectedMessage = $"Expected JSON document {_formatter.ToString(actual, true)}" +
                                  $" to be equivalent to {_formatter.ToString(expected, true)}" +
                                  $"{because}, but differs at {key}.";
            return expectedMessage;
        }

        #endregion (Not)BeEquivalentTo

        #region (Not)HaveValue

        [TestMethod]
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

        [TestMethod]
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
                .ShouldThrow<AssertFailedException>()
                .WithMessage("Expected JSON property \"id\" to have value \"43\" because foo, but found \"42\".");
        }

        [TestMethod]
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

        [TestMethod]
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
                .ShouldThrow<AssertFailedException>()
                .WithMessage("Did not expect JSON property \"id\" to have value \"42\" because foo.");
        }

        #endregion (Not)HaveValue

        #region (Not)HaveElement

        [TestMethod]
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

        [TestMethod]
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
                .ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document {_formatter.ToString(subject)} to have element \"name\" because foo, but no such element was found.");
        }

        [TestMethod]
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

        [TestMethod]
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
                .ShouldThrow<AssertFailedException>()
                .WithMessage($"Did not expect JSON document {_formatter.ToString(subject)} to have element \"id\" because foo.");
        }

        #endregion (Not)HaveElement

        #region ContainSingleItem

        [TestMethod]
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
            act.ShouldNotThrow();
        }

        [TestMethod]
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

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document <null> to contain a single item because null is not allowed, but found <null>.");
        }

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document * to contain a single item because less is not allowed, but the collection is empty.");
        }

        [TestMethod]
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
            string formattedSubject = _formatter.ToString(subject);

            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document {formattedSubject} to contain a single item because more is not allowed, but found {formattedSubject}.");
        }

        [TestMethod]
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
            act.ShouldNotThrow();
        }

        [TestMethod]
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

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document [] to contain a single item because less is not allowed, but the collection is empty.");
        }

        [TestMethod]
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
            string formattedSubject = _formatter.ToString(subject);

            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document {formattedSubject} to contain a single item because more is not allowed, but found {formattedSubject}.");
        }

        #endregion ContainSingleItem

        #region HaveCount

        [TestMethod]
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
            act.ShouldNotThrow();
        }

        [TestMethod]
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

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document <null> to contain 1 item(s) because null is not allowed, but found <null>.");
        }

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document * to contain 1 item(s) because numbers matter, but found 0.");
        }

        [TestMethod]
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
            act.ShouldNotThrow();
        }

        [TestMethod]
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
            act.ShouldThrow<AssertFailedException>()
                .WithMessage($"Expected JSON document * to contain 3 item(s) because the more the better, but found 2.");
        }

        #endregion HaveCount
    }
}