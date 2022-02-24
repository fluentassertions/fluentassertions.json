using System;

namespace FluentAssertions.Json.Common
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Finds the first index at which the <paramref name="value"/> does not match the <paramref name="expected"/>
        /// string anymore, accounting for the specified <paramref name="stringComparison"/>.
        /// </summary>
        public static int IndexOfFirstMismatch(this string value, string expected, StringComparison stringComparison)
        {
            for (int index = 0; index < value.Length; index++)
            {
                if ((index >= expected.Length) || !value[index].ToString().Equals(expected[index].ToString(), stringComparison))
                {
                    return index;
                }
            }

            return -1;
        }

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
}
