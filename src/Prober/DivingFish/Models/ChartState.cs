using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record ChartState
{
    [JsonPropertyName("cnt")]
    public decimal SampleSize { get; init; }

    [JsonPropertyName("diff")]
    public string? Level { get; init; }

    [JsonPropertyName("fit_diff")]
    public decimal FitLevel { get; init; }

    [JsonPropertyName("avg")]
    public decimal AverageAchievements { get; init; }

    [JsonPropertyName("avg_dx")]
    public decimal AverageDXScore { get; init; }

    [JsonPropertyName("std_dev")]
    public decimal StandardDeviation { get; init; }

    [JsonPropertyName("dist")]
    public List<int>? RankDistribution { get; init; }

    [JsonPropertyName("fc_dist")]
    public List<decimal>? ComboDistribution { get; init; }
}
