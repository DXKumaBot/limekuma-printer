namespace Limekuma.Prober.Common;

public record Record
{
    public required Chart Chart { get; init; }

    public required ComboFlag ComboFlag { get; init; }

    public required SyncFlag SyncFlag { get; init; }

    public required AchievementsRank Rank { get; init; }

    public required decimal Achievements { get; init; }

    public required int DXScore { get; init; }

    public required int DXScoreRank { get; init; }

    public required int DXRating { get; init; }

    public decimal? ExtraInfo { get; internal set; }
}
