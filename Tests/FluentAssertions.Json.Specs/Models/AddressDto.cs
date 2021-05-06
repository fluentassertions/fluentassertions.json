using System;
using Newtonsoft.Json;

namespace FluentAssertions.Json.Specs.Models
{
    // ReSharper disable UnusedMember.Global

    public class AddressDto
    {
        public string AddressLine1{ get; set; }
        public string AddressLine2{ get; set; }
        public string AddressLine3{ get; set; }
    }

    public class DerivedFromAddressDto:AddressDto
    {
        [JsonIgnore]
        public DateTime LastUpdated{ get; set; }
    }
    // ReSharper restore UnusedMember.Global

}
