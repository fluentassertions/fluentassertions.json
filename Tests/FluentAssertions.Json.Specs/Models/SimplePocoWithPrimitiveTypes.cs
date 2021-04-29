using System;

namespace FluentAssertions.Json.Specs.Models
{
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

        public byte[] Image { get; set; }
        public char Category { get; set; }
    }
    // ReSharper restore UnusedMember.Global
}
