using System.Text.Json;

namespace MonteCarlo;

public static class Program
{
    public static int Main(string[] args)
    {
        CliOptions options;
        try
        {
            options = CliParser.Parse(args);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine();
            Console.Error.WriteLine(CliParser.Usage);
            return 1;
        }

        if (options.ShowHelp)
        {
            Console.WriteLine(CliParser.Usage);
            return 0;
        }

        try
        {
            SimulationSummary summary = MonteCarloEngine.Run(options.Config);
            double parityGap = Math.Abs(
                (summary.CallPrice - summary.PutPrice)
                - (options.Config.Spot - options.Config.Strike * Math.Exp(-options.Config.Rate * options.Config.Maturity))
            );

            if (options.Json)
            {
                PrintJson(options.Config, summary, parityGap);
            }
            else
            {
                PrintText(options.Config, summary, parityGap);
            }

            return 0;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Simulation failed: {ex.Message}");
            return 1;
        }
    }

    private static void PrintText(SimulationConfig config, SimulationSummary summary, double parityGap)
    {
        Console.WriteLine("Monte Carlo European Option Pricing");
        Console.WriteLine("==================================");
        Console.WriteLine($"simulations: {config.Simulations}");
        Console.WriteLine($"steps:       {config.Steps}");
        Console.WriteLine($"spot:        {config.Spot:F4}");
        Console.WriteLine($"strike:      {config.Strike:F4}");
        Console.WriteLine($"rate:        {config.Rate:F4}");
        Console.WriteLine($"volatility:  {config.Volatility:F4}");
        Console.WriteLine($"maturity:    {config.Maturity:F4}");
        Console.WriteLine($"seed:        {config.Seed}");
        Console.WriteLine();
        Console.WriteLine($"call_price:  {summary.CallPrice:F6}");
        Console.WriteLine($"call_ci_95:  [{summary.CallCi95.Low:F6}, {summary.CallCi95.High:F6}]");
        Console.WriteLine($"put_price:   {summary.PutPrice:F6}");
        Console.WriteLine($"put_ci_95:   [{summary.PutCi95.Low:F6}, {summary.PutCi95.High:F6}]");
        Console.WriteLine($"prob_itm:    {summary.ProbabilityInTheMoney:F6}");
        Console.WriteLine($"E[S_T]:      {summary.ExpectedTerminalPrice:F6}");
        Console.WriteLine($"E_theory:    {summary.TheoreticalTerminalExpectation:F6}");
        Console.WriteLine();
        Console.WriteLine($"put_call_parity_gap: {parityGap:F6}");
    }

    private static void PrintJson(SimulationConfig config, SimulationSummary summary, double parityGap)
    {
        var payload = new
        {
            simulations = config.Simulations,
            steps = config.Steps,
            spot = config.Spot,
            strike = config.Strike,
            rate = config.Rate,
            volatility = config.Volatility,
            maturity = config.Maturity,
            seed = config.Seed,
            callPrice = summary.CallPrice,
            callCi95 = summary.CallCi95,
            putPrice = summary.PutPrice,
            putCi95 = summary.PutCi95,
            probabilityInTheMoney = summary.ProbabilityInTheMoney,
            expectedTerminalPrice = summary.ExpectedTerminalPrice,
            theoreticalTerminalExpectation = summary.TheoreticalTerminalExpectation,
            putCallParityGap = parityGap
        };

        Console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}
