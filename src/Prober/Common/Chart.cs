using Fractions;

namespace Limekuma.Prober.Common;

public class Chart
{
    public required Song Song { get; init; }

    public required Difficulty Difficulty { get; init; }

    public required int TotalDXScore { get; init; }

    public required string Level { get; init; }

    public required Fraction LevelValue { get; init; }

    public required Notes Notes { get; init; }
}
