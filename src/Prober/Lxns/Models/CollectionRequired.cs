using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

namespace Limekuma.Prober.Lxns.Models;

public record CollectionRequired
{
    [JsonPropertyName("difficulties")]
    public List<Difficulty>? Difficulties { get; init; }

    [JsonPropertyName("rate")]
    public CommonAchievementsRankEnum? Rank { get; init; }

    [JsonPropertyName("fc")]
    public CommonComboFlagEnum? ComboFlag { get; init; }

    [JsonPropertyName("fs")]
    public CommonSyncFlagEnum? SyncFlag { get; init; }

    [JsonPropertyName("songs")]
    public List<CollectionRequiredSong>? Songs { get; init; }

    [JsonPropertyName("completed")]
    public bool? Completed { get; init; }
}
