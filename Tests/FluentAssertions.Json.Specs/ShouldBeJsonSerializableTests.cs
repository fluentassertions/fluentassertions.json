using System;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Json;
using FluentAssertions.Json.Specs.Models;
using Xunit;

// NOTE that we are using both namespaces 'FluentAssertions' & 'FluentAssertions.Json' from an external namespace to force compiler disambiguation warnings
// ReSharper disable CheckNamespace
namespace SomeOtherNamespace
// ReSharper restore CheckNamespace
{
    public class ShouldBeJsonSerializableTests
    {
        private readonly Fixture _fixture;

        public ShouldBeJsonSerializableTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Simple_poco_should_be_serializable()
        {
            // arrange
            var target = _fixture.Create<SimplePocoWithPrimitiveTypes>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Complex_poco_should_be_serializable()
        {
            // arrange
            var target = _fixture.Create<PocoWithStructure>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Class_that_does_not_have_default_constructor_should_not_be_serializable()
        {
            // arrange
            const string reasonText = "this is the reason";
            var target = _fixture.Create<PocoWithNoDefaultConstructor>();

            // act
            Action act = () => target.Should().BeJsonSerializable(reasonText);

            // assert
            act.Should().Throw<Xunit.Sdk.XunitException>()
                .Which.Message.Should()
                    .Contain("to be JSON serializable")
                    .And.Contain(reasonText)
                    .And.Contain("but serializing")
                    .And.Contain("failed with");
        }

        [Fact]
        public void Class_that_has_ignored_property_should_not_be_serializable_if_equivalency_options_are_not_configured()
        {
            // arrange
            const string reasonText = "this is the reason";
            var target = _fixture.Create<PocoWithIgnoredProperty>();

            // act
            Action act = () => target.Should().BeJsonSerializable(reasonText);

            // assert
            act.Should().Throw<Xunit.Sdk.XunitException>()
                .Which.Message.Should()
                    .Contain("to be JSON serializable")
                    .And.Contain(reasonText)
                    .And.Contain("but serializing")
                    .And.Contain("failed with");
        }

        [Fact]
        public void Class_that_has_ignored_property_should_be_serializable_when_equivalency_options_are_configured()
        {
            // arrange
            var target = _fixture.Create<PocoWithIgnoredProperty>();

            // act
            Action act = () => target.Should().BeJsonSerializable<PocoWithIgnoredProperty>(opts => opts.Excluding(p => p.Name));

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Should_fail_when_instance_is_null()
        {
            // arrange
            const SimplePocoWithPrimitiveTypes target = null;

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should()
                .Throw<Xunit.Sdk.XunitException>(because:"This is consistent with BeBinarySerializable() and BeDataContractSerializable()")
                .Which.Message
                    .Should().Contain("value is null")
                        .And.Contain("Please provide a value for the assertion");
        }

        [Fact]
        public void Should_fail_when_subject_is_not_same_type_as_the_specified_generic_type()
        {
            // arrange
            var target = new AddressDto();

            // act
            Action act = () => target.Should().BeJsonSerializable<SimplePocoWithPrimitiveTypes>();

            // assert
            act.Should().Throw<Xunit.Sdk.XunitException>(because: "This is consistent with BeBinarySerializable() and BeDataContractSerializable()")
                .Which.Message
                    .Should().Contain("is not assignable to")
                        .And.Contain(nameof(SimplePocoWithPrimitiveTypes));
                ;
        }

    }

}

