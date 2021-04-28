using System;
using AutoFixture;
using FluentAssertions;
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
        public void Class_is_simple_poco_should_be_serializable()
        {
            // arrange
            var target = _fixture.Create<SimplePocoWithPrimitiveTypes>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Class_is_complex_poco_should_be_serializable()
        {
            // arrange
            var target = _fixture.Create<PocoWithStructure>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Class_does_not_have_default_constructor()
        {
            // arrange
            var target = _fixture.Create<PocoWithNoDefaultConstructor>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void Class_has_ignored_property()
        {
            // arrange
            var target = _fixture.Create<PocoWithIgnoredProperty>();

            // act
            Action act = () => target.Should().BeJsonSerializable();

            // assert
            act.Should().Throw<Xunit.Sdk.XunitException>();
        }

        [Fact]
        public void Class_has_ignored_property_equivalency_options_are_configured()
        {
            // arrange
            var target = _fixture.Create<PocoWithIgnoredProperty>();

            // act
            Action act = () => target.Should().BeJsonSerializable<PocoWithIgnoredProperty>(opts => opts.Excluding(p => p.Name));

            // assert
            act.Should().NotThrow();
        }

    }

}

