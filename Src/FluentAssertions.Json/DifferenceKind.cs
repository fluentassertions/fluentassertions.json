namespace FluentAssertions.Json;

internal enum DifferenceKind
{
    ActualIsNull,
    ExpectedIsNull,
    OtherType,
    OtherName,
    OtherValue,
    DifferentLength,
    ActualMissesProperty,
    ExpectedMissesProperty,
    ActualMissesElement,
    WrongOrder
}
