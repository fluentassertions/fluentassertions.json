namespace FluentAssertions.Json
{
    /// <summary>
    /// Defines additional overrides when used with <see cref="FluentAssertions.Json.JsonAssertionRestriction{T, TProperty}" />
    /// </summary>
    public interface IJsonAssertionRestriction<T, TMember>
    {
        /// <summary>
        /// Allows overriding the way structural equality is applied to (nested) objects of type
        /// <typeparamref name="TMemberType" />
        /// </summary>
        public IJsonAssertionOptions<T> WhenTypeIs<TMemberType>() where TMemberType : TMember;
    }
}
