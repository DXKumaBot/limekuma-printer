using Fractions;
using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record LevelState
{
    [JsonPropertyName("achievements")]
    public required Fraction AverageAchievements { get; init; }

    [JsonPropertyName("dist")]
    public required List<Fraction> RankDistribution { get; init; }

    [JsonPropertyName("fc_dist")]
    public required List<Fraction> ComboDistribution { get; init; }
}
