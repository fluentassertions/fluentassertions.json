namespace FluentAssertions.Json.Specs.Models
{
    public class PocoWithNoDefaultConstructor
    {
        public int Id { get; }

        /// <summary>
        /// Newtonsoft.Json will deserialise this successfully if the parameter name id the same as the property
        /// </summary>
        /// <param name="value">DO NOT CHANGE THE NAME OF THIS PARAMETER</param>
        public PocoWithNoDefaultConstructor(int value)
        {
            Id = value;
        }
    }
}