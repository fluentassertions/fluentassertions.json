using FluentAssertions.Equivalency;

namespace FluentAssertions.Json
{
    public sealed class JSonAssertionOption<T> : EquivalencyAssertionOptions<T> , IJsonAssertionOptions<T>
    {
    }
}