namespace MonteCarlo;

public static class MonteCarloEngine
{
    public static SimulationSummary Run(SimulationConfig config)
    {
        ValidateConfig(config);

        double n = config.Simulations;
        double dt = config.Maturity / config.Steps;
        double drift = (config.Rate - 0.5 * config.Volatility * config.Volatility) * dt;
        double diffusion = config.Volatility * Math.Sqrt(dt);
        double discount = Math.Exp(-config.Rate * config.Maturity);

        var normals = new NormalSampler(config.Seed);

        double callSum = 0.0;
        double callSqSum = 0.0;
        double putSum = 0.0;
        double putSqSum = 0.0;
        double terminalSum = 0.0;
        int itmCount = 0;

        for (int path = 0; path < config.Simulations; path++)
        {
            double price = config.Spot;
            for (int step = 0; step < config.Steps; step++)
            {
                double z = normals.NextStandardNormal();
                price *= Math.Exp(drift + diffusion * z);
            }

            terminalSum += price;

            double callPayoff = Math.Max(price - config.Strike, 0.0);
            double putPayoff = Math.Max(config.Strike - price, 0.0);

            callSum += callPayoff;
            callSqSum += callPayoff * callPayoff;
            putSum += putPayoff;
            putSqSum += putPayoff * putPayoff;

            if (price > config.Strike)
            {
                itmCount++;
            }
        }

        double meanCallPayoff = callSum / n;
        double meanPutPayoff = putSum / n;

        double callPrice = discount * meanCallPayoff;
        double putPrice = discount * meanPutPayoff;

        double callVar = Math.Max(callSqSum / n - meanCallPayoff * meanCallPayoff, 0.0);
        double putVar = Math.Max(putSqSum / n - meanPutPayoff * meanPutPayoff, 0.0);

        double callStdErr = discount * Math.Sqrt(callVar / n);
        double putStdErr = discount * Math.Sqrt(putVar / n);

        const double z95 = 1.96;

        return new SimulationSummary
        {
            CallPrice = callPrice,
            PutPrice = putPrice,
            CallCi95 = new ConfidenceInterval(callPrice - z95 * callStdErr, callPrice + z95 * callStdErr),
            PutCi95 = new ConfidenceInterval(putPrice - z95 * putStdErr, putPrice + z95 * putStdErr),
            ProbabilityInTheMoney = itmCount / n,
            ExpectedTerminalPrice = terminalSum / n,
            TheoreticalTerminalExpectation = config.Spot * Math.Exp(config.Rate * config.Maturity)
        };
    }

    private static void ValidateConfig(SimulationConfig config)
    {
        if (config.Simulations <= 0)
        {
            throw new ArgumentException("simulations must be greater than 0");
        }

        if (config.Steps <= 0)
        {
            throw new ArgumentException("steps must be greater than 0");
        }

        if (config.Spot <= 0.0)
        {
            throw new ArgumentException("spot must be greater than 0");
        }

        if (config.Strike <= 0.0)
        {
            throw new ArgumentException("strike must be greater than 0");
        }

        if (config.Volatility <= 0.0)
        {
            throw new ArgumentException("volatility must be greater than 0");
        }

        if (config.Maturity <= 0.0)
        {
            throw new ArgumentException("maturity must be greater than 0");
        }
    }

    private sealed class NormalSampler
    {
        private readonly XorShift64Star _rng;
        private double? _cached;

        public NormalSampler(ulong seed)
        {
            _rng = new XorShift64Star(seed);
        }

        public double NextStandardNormal()
        {
            if (_cached.HasValue)
            {
                double cached = _cached.Value;
                _cached = null;
                return cached;
            }

            double u1 = _rng.NextUnitOpen();
            double u2 = _rng.NextUnitOpen();

            double radius = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = 2.0 * Math.PI * u2;

            double z0 = radius * Math.Cos(theta);
            double z1 = radius * Math.Sin(theta);
            _cached = z1;
            return z0;
        }
    }

    private sealed class XorShift64Star
    {
        private ulong _state;

        public XorShift64Star(ulong seed)
        {
            _state = seed == 0 ? 0x9E37_79B9_7F4A_7C15UL : seed;
        }

        private ulong NextUInt64()
        {
            ulong x = _state;
            x ^= x >> 12;
            x ^= x << 25;
            x ^= x >> 27;
            _state = x;
            return x * 0x2545_F491_4F6C_DD1DUL;
        }

        public double NextUnitOpen()
        {
            ulong value = NextUInt64() >> 11;
            return (value + 0.5) * (1.0 / (1UL << 53));
        }
    }
}
