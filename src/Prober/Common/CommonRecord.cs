namespace Limekuma.Prober.Common;

public record CommonRecord
{
    public required CommonChart Chart { get; init; }

    public required ComboFlags ComboFlag { get; init; }

    public required SyncFlags SyncFlag { get; init; }

    public required Ranks Rank { get; init; }

    public required decimal Achievements { get; init; }

    public required int DXScore { get; init; }

    public required int DXScoreRank { get; init; }

    public required int DXRating { get; init; }

    public decimal ExtraInfo { get; internal set; }
}
