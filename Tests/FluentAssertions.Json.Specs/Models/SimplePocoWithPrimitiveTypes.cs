using System;

namespace FluentAssertions.Json.Specs.Models;

// ReSharper disable UnusedMember.Global
public class SimplePocoWithPrimitiveTypes
{
    public int Id { get; set; }

    public Guid GlobalId { get; set; }

    public string Name { get; set; }

    public DateTime DateOfBirth { get; set; }

    public decimal Height { get; set; }

    public double Weight { get; set; }

    public float ShoeSize { get; set; }

    public bool IsActive { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
    public byte[] Image { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

    public char Category { get; set; }
}

// ReSharper restore UnusedMember.Global
