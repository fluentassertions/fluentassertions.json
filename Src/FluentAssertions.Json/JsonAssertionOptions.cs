using System;
using FluentAssertions.Equivalency;

namespace FluentAssertions.Json;

/// <summary>
/// Represents the run-time type-specific behavior of a JSON structural equivalency assertion. It is the equivalent of <see cref="FluentAssertions.Equivalency.EquivalencyOptions{T}"/>
/// </summary>
public sealed class JsonAssertionOptions<T> : EquivalencyOptions<T>, IJsonAssertionOptions<T>
{
    internal JsonAssertionOptions()
    {
    }

    public JsonAssertionOptions(EquivalencyOptions<T> equivalencyAssertionOptions)
        : base(equivalencyAssertionOptions)
    {
    }

    internal bool IsStrictlyOrdered { get; private set; } = true;

    public new IJsonAssertionRestriction<T, TProperty> Using<TProperty>(Action<IAssertionContext<TProperty>> action)
    {
        return new JsonAssertionRestriction<T, TProperty>(base.Using(action));
    }

    public new IJsonAssertionOptions<T> WithoutStrictOrdering()
    {
        IsStrictlyOrdered = false;
        return this;
    }
}
