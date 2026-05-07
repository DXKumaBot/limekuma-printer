using Fractions;
using Limekuma.Prober.DivingFish.Enums;
using Limekuma.Utils;
using System.Text.Json.Serialization;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

namespace Limekuma.Prober.DivingFish.Models;

public class SimpleRecord
{
    [JsonPropertyName("achievements")]
    public required Fraction Achievements { get; init; }

    [JsonPropertyName("fc")]
    public required Union<CommonComboFlagEnum, string> ComboFlag { get; init; }

    [JsonPropertyName("fs")]
    public required Union<CommonSyncFlagEnum, string> SyncFlag { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("level")]
    public required string Level { get; init; }

    [JsonPropertyName("level_index")]
    public required int DifficultyIndex { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("type")]
    public required ChartType Type { get; init; }
}
