namespace MonteCarlo;

public sealed record SimulationConfig
{
    public int Simulations { get; init; } = 100_000;
    public int Steps { get; init; } = 252;
    public double Spot { get; init; } = 100.0;
    public double Strike { get; init; } = 100.0;
    public double Rate { get; init; } = 0.05;
    public double Volatility { get; init; } = 0.20;
    public double Maturity { get; init; } = 1.0;
    public ulong Seed { get; init; } = 42;
}
