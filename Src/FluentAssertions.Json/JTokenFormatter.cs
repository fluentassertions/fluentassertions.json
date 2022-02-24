using FluentAssertions.Formatting;
using FluentAssertions.Json.Common;
using Newtonsoft.Json.Linq;

namespace FluentAssertions.Json
{
    /// <summary>
    /// A <see cref="IValueFormatter"/> for <see cref="JToken" />.
    /// </summary>
    public class JTokenFormatter : IValueFormatter
    {
        /// <summary>
        /// Indicates whether the current <see cref="IValueFormatter"/> can handle the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value for which to create a <see cref="string"/>.</param>
        /// <returns>
        /// <c>true</c> if the current <see cref="IValueFormatter"/> can handle the specified value; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(object value)
        {
            return value is JToken;
        }

        public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
        {
            var jToken = value as JToken;

            if (context.UseLineBreaks)
            {
                var result = jToken?.ToString(Newtonsoft.Json.Formatting.Indented);
                if (result is not null)
                {
                    formattedGraph.AddFragmentOnNewLine(result);
                }
                else
                {
                    formattedGraph.AddFragment("<null>");
                }
            }
            else
            {
                formattedGraph.AddFragment(jToken?.ToString().RemoveNewLines() ?? "<null>");
            }
        }
    }
}
