using System;
using FluentAssertions.Equivalency;

namespace FluentAssertions.Json
{
    /// <summary>
    /// Represents the run-time type-specific behavior of a json structural equivalency assertion. It is the equivalent of the class EquivalencyAssertionOptions in FluentAssertion
    /// </summary>
    public sealed class JsonAssertionOptions<T> : EquivalencyAssertionOptions<T> , IJsonAssertionOptions<T>
    {
        public new IJsonAssertionRestriction<T, TProperty> Using<TProperty>(Action<IAssertionContext<TProperty>> action)
        {
            return new JsonAssertionRestriction<T, TProperty>(base.Using(action));
        }
    }
}