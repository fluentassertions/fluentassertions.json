using System;
using FluentAssertions.Equivalency;

namespace FluentAssertions.Json
{
    public interface IJsonAssertionOptions<T>
    {
        EquivalencyAssertionOptions<T>.Restriction<TProperty> Using<TProperty>(Action<IAssertionContext<TProperty>> action);
    }
}
