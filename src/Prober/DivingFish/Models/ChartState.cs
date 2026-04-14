using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record ChartState
{
    [JsonPropertyName("cnt")]
    public double SampleSize { get; init; }

    [JsonPropertyName("diff")]
    public string? Level { get; init; }

    [JsonPropertyName("fit_diff")]
    public double FitLevel { get; init; }

    [JsonPropertyName("avg")]
    public double AverageAchievements { get; init; }

    [JsonPropertyName("avg_dx")]
    public double AverageDXScore { get; init; }

    [JsonPropertyName("std_dev")]
    public double StandardDeviation { get; init; }

    [JsonPropertyName("dist")]
    public List<int>? RankDistribution { get; init; }

    [JsonPropertyName("fc_dist")]
    public List<double>? ComboDistribution { get; init; }
}
