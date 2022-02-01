using System;
using System.Collections.Generic;
using FluentAssertions.Equivalency;

namespace FluentAssertions.Json
{
    public interface IJsonAssertionOptions<T>
    {
        SelfReferenceEquivalencyAssertionOptions<EquivalencyAssertionOptions<T>>.Restriction<TProperty> Using<TProperty>(Action<IAssertionContext<TProperty>> action);
    }
}
