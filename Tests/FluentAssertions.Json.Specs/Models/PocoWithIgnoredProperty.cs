using Newtonsoft.Json;

namespace FluentAssertions.Json.Specs.Models;

public class PocoWithIgnoredProperty
{
    public int Id { get; set; }

    [JsonIgnore]
    public string Name { get; set; }
}
