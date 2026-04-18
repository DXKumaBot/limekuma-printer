using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

namespace Limekuma.Prober.Lxns.Models;

public record SimpleRecord
{
    [JsonPropertyName("id")]
    public required int Id
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("song_name")]
    public required string Title { get; init; }

    [JsonPropertyName("level")]
    public required string Level { get; init; }

    [JsonPropertyName("level_index")]
    public required Difficulty Difficulty { get; init; }

    [JsonPropertyName("fc")]
    public CommonComboFlagEnum? ComboFlag { get; init; }

    [JsonPropertyName("fs")]
    public CommonSyncFlagEnum? SyncFlag { get; init; }

    [JsonPropertyName("rate")]
    public CommonAchievementsRankEnum Rank { get; init; }

    [JsonPropertyName("type")]
    public required ChartType Type { get; init; }
}
