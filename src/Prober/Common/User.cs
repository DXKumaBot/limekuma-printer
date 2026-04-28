namespace Limekuma.Prober.Common;

public record User
{
    public required ProberType Prober { get; init; }

    public required string Name { get; init; }

    public required int DXRating { get; init; }

    public required TitleColor TitleColor { get; set; }

    public required string Title { get; set; }

    public required GradeRank GradeRank { get; init; }

    public required ClassRank ClassRank { get; set; }

    public required int IconId { get; set; }

    public required int PlateId { get; set; }

    public required int FrameId { get; set; }

    public bool MayMasked { get; internal set; } = false;

    public decimal? ExtraInfo { get; internal set; }

    public DXRatingRank DXRatingRank => DXRating switch
    {
        < 1000 => DXRatingRank.White,
        < 2000 => DXRatingRank.Blue,
        < 4000 => DXRatingRank.Green,
        < 7000 => DXRatingRank.Yellow,
        < 10000 => DXRatingRank.Red,
        < 12000 => DXRatingRank.Purple,
        < 13000 => DXRatingRank.Bronze,
        < 14000 => DXRatingRank.Silver,
        < 14500 => DXRatingRank.Gold,
        < 15000 => DXRatingRank.Platinum,
        > 14999 => DXRatingRank.Rainbow
    };

    public string IconUrl => $"https://assets2.lxns.net/maimai/icon/{IconId}.png";

    public string PlateUrl => $"https://assets2.lxns.net/maimai/plate/{PlateId}.png";

    public string FrameUrl => $"https://assets2.lxns.net/maimai/frame/{FrameId}.png";
}
