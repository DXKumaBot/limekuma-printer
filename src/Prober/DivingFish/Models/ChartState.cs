using Fractions;
using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record ChartState
{
    [JsonPropertyName("cnt")]
    public Fraction SampleSize { get; init; }

    [JsonPropertyName("diff")]
    public string? Level { get; init; }

    [JsonPropertyName("fit_diff")]
    public Fraction FitLevel { get; init; }

    [JsonPropertyName("avg")]
    public Fraction AverageAchievements { get; init; }

    [JsonPropertyName("avg_dx")]
    public Fraction AverageDXScore { get; init; }

    [JsonPropertyName("std_dev")]
    public Fraction StandardDeviation { get; init; }

    [JsonPropertyName("dist")]
    public List<int>? RankDistribution { get; init; }

    [JsonPropertyName("fc_dist")]
    public List<Fraction>? ComboDistribution { get; init; }
}
