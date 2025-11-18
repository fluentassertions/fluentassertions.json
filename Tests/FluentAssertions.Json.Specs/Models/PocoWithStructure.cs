namespace FluentAssertions.Json.Specs.Models;

// ReSharper disable UnusedMember.Global
public class PocoWithStructure
{
    public int Id { get; set; }

    public AddressDto Address { get; set; }

    public EmploymentDto Employment { get; set; }
}

// ReSharper restore UnusedMember.Global
