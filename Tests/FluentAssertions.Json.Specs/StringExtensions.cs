namespace FluentAssertions.Json.Specs
{
    internal static class StringExtensions
    {
        public static string RemoveNewLines(this string @this)
        {
            return @this.Replace("\n", "").Replace("\r", "").Replace("\\r\\n", "");
        }
    }
}
