using System.Globalization;

namespace MonteCarlo;

public sealed class CliOptions
{
    public required SimulationConfig Config { get; init; }
    public required bool Json { get; init; }
    public required bool ShowHelp { get; init; }
}

public static class CliParser
{
    public const string Usage = """
Monte Carlo Simulator (.NET)

Usage:
  dotnet run --project src/MonteCarlo -- [options]

Options:
  --simulations <n>   Number of Monte Carlo paths (default: 100000)
  --steps <n>         Time steps per path (default: 252)
  --spot <x>          Initial asset price (default: 100.0)
  --strike <x>        Option strike price (default: 100.0)
  --rate <x>          Risk-free annual rate as decimal (default: 0.05)
  --volatility <x>    Annual volatility as decimal (default: 0.20)
  --maturity <x>      Time to maturity in years (default: 1.0)
  --seed <n>          RNG seed for deterministic runs (default: 42)
  --json              Print machine-readable JSON
  -h, --help          Show this help
""";

    public static CliOptions Parse(string[] args)
    {
        var config = new SimulationConfig();
        bool json = false;
        bool help = false;

        int i = 0;
        while (i < args.Length)
        {
            switch (args[i])
            {
                case "-h":
                case "--help":
                    help = true;
                    break;
                case "--json":
                    json = true;
                    break;
                case "--simulations":
                    config = config with { Simulations = ParseIntValue(args, ref i, "--simulations") };
                    break;
                case "--steps":
                    config = config with { Steps = ParseIntValue(args, ref i, "--steps") };
                    break;
                case "--spot":
                    config = config with { Spot = ParseDoubleValue(args, ref i, "--spot") };
                    break;
                case "--strike":
                    config = config with { Strike = ParseDoubleValue(args, ref i, "--strike") };
                    break;
                case "--rate":
                    config = config with { Rate = ParseDoubleValue(args, ref i, "--rate") };
                    break;
                case "--volatility":
                    config = config with { Volatility = ParseDoubleValue(args, ref i, "--volatility") };
                    break;
                case "--maturity":
                    config = config with { Maturity = ParseDoubleValue(args, ref i, "--maturity") };
                    break;
                case "--seed":
                    config = config with { Seed = ParseUlongValue(args, ref i, "--seed") };
                    break;
                default:
                    throw new ArgumentException($"unknown option: {args[i]}");
            }

            i++;
        }

        return new CliOptions
        {
            Config = config,
            Json = json,
            ShowHelp = help
        };
    }

    private static int ParseIntValue(string[] args, ref int index, string flag)
    {
        index++;
        EnsureValueExists(args, index, flag);
        if (!int.TryParse(args[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
        {
            throw new ArgumentException($"invalid value for {flag}: '{args[index]}'");
        }
        return parsed;
    }

    private static ulong ParseUlongValue(string[] args, ref int index, string flag)
    {
        index++;
        EnsureValueExists(args, index, flag);
        if (!ulong.TryParse(args[index], NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong parsed))
        {
            throw new ArgumentException($"invalid value for {flag}: '{args[index]}'");
        }
        return parsed;
    }

    private static double ParseDoubleValue(string[] args, ref int index, string flag)
    {
        index++;
        EnsureValueExists(args, index, flag);
        if (!double.TryParse(args[index], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double parsed))
        {
            throw new ArgumentException($"invalid value for {flag}: '{args[index]}'");
        }
        return parsed;
    }

    private static void EnsureValueExists(string[] args, int index, string flag)
    {
        if (index >= args.Length)
        {
            throw new ArgumentException($"missing value for {flag}");
        }
    }
}
