using FluentAssertions.Equivalency;

namespace FluentAssertions.Json
{
    public interface IEquivalencyAssertionOptions
    {
        EquivalencyAssertionOptions<object> Instance { get; }
    }

    public sealed class DoubleElementShouldBeApproximately : IEquivalencyAssertionOptions
    {
        public DoubleElementShouldBeApproximately(double precision)
        {
            Instance = new EquivalencyAssertionOptions<object>().Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, precision))
                .WhenTypeIs<double>();
        }
        public EquivalencyAssertionOptions<object> Instance { get; }
    }
}
