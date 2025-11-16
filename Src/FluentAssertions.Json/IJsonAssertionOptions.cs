using System;
using FluentAssertions.Equivalency;

namespace FluentAssertions.Json;

/// <summary>
/// Provides the run-time details of the <see cref="FluentAssertions.Json.JsonAssertionOptions{T}" /> class.
/// </summary>
public interface IJsonAssertionOptions<T>
{
    /// <summary>
    /// Overrides the comparison of subject and expectation to use provided <paramref name="action" />
    /// when the predicate is met.
    /// </summary>
    /// <param name="action">
    /// The assertion to execute when the predicate is met.
    /// </param>
#pragma warning disable CA1716 // CA1716: Identifiers should not match keywords
    IJsonAssertionRestriction<T, TProperty> Using<TProperty>(Action<IAssertionContext<TProperty>> action);
#pragma warning restore CA1716

    /// <summary>
    /// Configures the JSON assertion to ignore the order of elements in arrays or collections during comparison, allowing for equivalency checks regardless of element sequence.
    /// </summary>
    IJsonAssertionOptions<T> WithoutStrictOrdering();
}
