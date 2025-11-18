namespace FluentAssertions.Json;

public sealed class JsonAssertionRestriction<T, TProperty> : IJsonAssertionRestriction<T, TProperty>
{
    private readonly JsonAssertionOptions<T>.Restriction<TProperty> restriction;

    internal JsonAssertionRestriction(JsonAssertionOptions<T>.Restriction<TProperty> restriction)
    {
        this.restriction = restriction;
    }

    public IJsonAssertionOptions<T> WhenTypeIs<TMemberType>()
        where TMemberType : TProperty
    {
        return (JsonAssertionOptions<T>)restriction.WhenTypeIs<TMemberType>();
    }
}
