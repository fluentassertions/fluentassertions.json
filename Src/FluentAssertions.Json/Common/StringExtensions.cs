namespace FluentAssertions.Json.Common;

internal static class StringExtensions
{
    /// <summary>
    /// Replaces all characters that might conflict with formatting placeholders with their escaped counterparts.
    /// </summary>
    public static string EscapePlaceholders(this string value) =>
        value.Replace("{", "{{").Replace("}", "}}");

    public static string RemoveNewLines(this string @this)
    {
        return @this.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\\r\\n", string.Empty);
    }
}
