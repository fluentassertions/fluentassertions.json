using System;

namespace FluentAssertions.Json;

internal sealed class Difference
{
    public Difference(DifferenceKind kind, JPath path, object actual, object expected)
        : this(kind, path)
    {
        Actual = actual;
        Expected = expected;
    }

    public Difference(DifferenceKind kind, JPath path)
    {
        Kind = kind;
        Path = path;
    }

    private DifferenceKind Kind { get; }

    private JPath Path { get; }

    private object Actual { get; }

    private object Expected { get; }

    public override string ToString()
    {
        return Kind switch
        {
            DifferenceKind.ActualIsNull => "is null",
            DifferenceKind.ExpectedIsNull => "is not null",
            DifferenceKind.OtherType => $"has {Actual} instead of {Expected} at {Path}",
            DifferenceKind.OtherName => $"has a different name at {Path}",
            DifferenceKind.OtherValue => $"has a different value at {Path}",
            DifferenceKind.DifferentLength => $"has {Actual} elements instead of {Expected} at {Path}",
            DifferenceKind.ActualMissesProperty => $"misses property {Path}",
            DifferenceKind.ExpectedMissesProperty => $"has extra property {Path}",
            DifferenceKind.ActualMissesElement => $"misses expected element {Path}",
            DifferenceKind.WrongOrder => $"has expected element {Path} in the wrong order",
#pragma warning disable MA0015 // Specify the parameter name in ArgumentException
            _ => throw new ArgumentOutOfRangeException(),
#pragma warning restore MA0015 // Specify the parameter name in ArgumentException
        };
    }
}
