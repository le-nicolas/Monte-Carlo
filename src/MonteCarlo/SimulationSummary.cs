namespace MonteCarlo;

public readonly record struct ConfidenceInterval(double Low, double High);

public sealed class SimulationSummary
{
    public required double CallPrice { get; init; }
    public required double PutPrice { get; init; }
    public required ConfidenceInterval CallCi95 { get; init; }
    public required ConfidenceInterval PutCi95 { get; init; }
    public required double ProbabilityInTheMoney { get; init; }
    public required double ExpectedTerminalPrice { get; init; }
    public required double TheoreticalTerminalExpectation { get; init; }
}
