using MonteCarlo;

namespace MonteCarlo.Tests;

public class MonteCarloEngineTests
{
    [Fact]
    public void SameSeedProducesDeterministicResults()
    {
        var config = new SimulationConfig
        {
            Simulations = 20_000,
            Steps = 64,
            Seed = 123_456
        };

        SimulationSummary first = MonteCarloEngine.Run(config);
        SimulationSummary second = MonteCarloEngine.Run(config);

        Assert.Equal(first.CallPrice, second.CallPrice);
        Assert.Equal(first.PutPrice, second.PutPrice);
        Assert.Equal(first.ProbabilityInTheMoney, second.ProbabilityInTheMoney);
        Assert.Equal(first.ExpectedTerminalPrice, second.ExpectedTerminalPrice);
    }

    [Fact]
    public void PutCallParityIsReasonablyClose()
    {
        var config = new SimulationConfig
        {
            Simulations = 80_000,
            Steps = 128,
            Seed = 999
        };

        SimulationSummary summary = MonteCarloEngine.Run(config);
        double lhs = summary.CallPrice - summary.PutPrice;
        double rhs = config.Spot - config.Strike * Math.Exp(-config.Rate * config.Maturity);

        Assert.True(Math.Abs(lhs - rhs) < 0.25);
    }

    [Fact]
    public void InvalidInputThrows()
    {
        var config = new SimulationConfig
        {
            Simulations = 0
        };

        ArgumentException ex = Assert.Throws<ArgumentException>(() => MonteCarloEngine.Run(config));
        Assert.Contains("simulations", ex.Message);
    }
}
