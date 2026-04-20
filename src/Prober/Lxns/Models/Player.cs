using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonGradeRankEnum = Limekuma.Prober.Common.GradeRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonProberTypeEnum = Limekuma.Prober.Common.ProberType;
using CommonTitleColorEnum = Limekuma.Prober.Common.TitleColor;

namespace Limekuma.Prober.Lxns.Models;

public record Player
{
    [JsonIgnore]
    public LxnsDeveloperClient? Client { get; internal set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("rating")]
    public required int DXRating { get; set; }

    [JsonPropertyName("friend_code")]
    public required long FriendCode { get; set; }

    [JsonPropertyName("course_rank")]
    public required GradeRank GradeRank { get; set; }

    [JsonPropertyName("class_rank")]
    public required CommonClassRankEnum ClassRank { get; set; }

    [JsonPropertyName("star")]
    public required int Star { get; set; }

    [JsonPropertyName("trophy")]
    public Title? Title { get; set; }

    [JsonPropertyName("icon")]
    public Icon? Icon { get; set; }

    [JsonPropertyName("name_plate")]
    public NamePlate? NamePlate { get; set; }

    [JsonPropertyName("frame")]
    public Frame? Frame { get; set; }

    [JsonPropertyName("upload_time")]
    public DateTimeOffset? UploadTime { get; set; }

    private static CommonGradeRankEnum MapGradeRank(GradeRank gradeRank) => gradeRank switch
    {
        GradeRank.Beginner => CommonGradeRankEnum.Beginner,
        GradeRank.FirstDan => CommonGradeRankEnum.FirstDan,
        GradeRank.SecondDan => CommonGradeRankEnum.SecondDan,
        GradeRank.ThirdDan => CommonGradeRankEnum.ThirdDan,
        GradeRank.FourthDan => CommonGradeRankEnum.FourthDan,
        GradeRank.FifthDan => CommonGradeRankEnum.FifthDan,
        GradeRank.SixthDan => CommonGradeRankEnum.SixthDan,
        GradeRank.SeventhDan => CommonGradeRankEnum.SeventhDan,
        GradeRank.EighthDan => CommonGradeRankEnum.EighthDan,
        GradeRank.NinthDan => CommonGradeRankEnum.NinthDan,
        GradeRank.TenthDan => CommonGradeRankEnum.TenthDan,
        GradeRank.TrueFirstDan => CommonGradeRankEnum.TrueFirstDan,
        GradeRank.TrueSecondDan => CommonGradeRankEnum.TrueSecondDan,
        GradeRank.TrueThirdDan => CommonGradeRankEnum.TrueThirdDan,
        GradeRank.TrueFourthDan => CommonGradeRankEnum.TrueFourthDan,
        GradeRank.TrueFifthDan => CommonGradeRankEnum.TrueFifthDan,
        GradeRank.TrueSixthDan => CommonGradeRankEnum.TrueSixthDan,
        GradeRank.TrueSeventhDan => CommonGradeRankEnum.TrueSeventhDan,
        GradeRank.TrueEighthDan => CommonGradeRankEnum.TrueEighthDan,
        GradeRank.TrueNinthDan => CommonGradeRankEnum.TrueNinthDan,
        GradeRank.TrueTenthDan => CommonGradeRankEnum.TrueTenthDan,
        GradeRank.TrueKaiden => CommonGradeRankEnum.TrueKaiden,
        GradeRank.HiddenKaiden => CommonGradeRankEnum.HiddenKaiden,
        _ => (CommonGradeRankEnum)gradeRank
    };

    public static implicit operator CommonPlayer(Player player) =>
        new()
        {
            Prober = CommonProberTypeEnum.Lxns,
            Name = player.Name,
            DXRating = player.DXRating,
            TitleColor = player.Title?.Color ?? CommonTitleColorEnum.Normal,
            Title = player.Title?.Name ?? "なかよしmai友～！",
            ClassRank = player.ClassRank,
            GradeRank = MapGradeRank(player.GradeRank),
            IconId = player.Icon?.Id ?? 458001,
            FrameId = player.Frame?.Id ?? 558001,
            PlateId = player.NamePlate?.Id ?? 458001
        };

    public async Task<Record> GetBestAsync(int id, Difficulty difficulty, ChartType type,
        CancellationToken cancellationToken = default) =>
        await Client!.GetBestAsync(FriendCode, id, difficulty, type, cancellationToken);

    public async Task<Record> GetBestAsync(string title, Difficulty difficulty, ChartType type,
        CancellationToken cancellationToken = default) =>
        await Client!.GetBestAsync(FriendCode, title, difficulty, type, cancellationToken);

    public async Task<Bests> GetBestsAsync(CancellationToken cancellationToken = default) =>
        await Client!.GetBestsAsync(FriendCode, cancellationToken);

    public async Task<Bests> GetAllPerfectBestsAsync(CancellationToken cancellationToken = default) =>
        await Client!.GetAllPerfectBestsAsync(FriendCode, cancellationToken);

    public async Task<List<Record>> GetRecordsAsync(int id, ChartType type,
        CancellationToken cancellationToken = default) =>
        await Client!.GetRecordsAsync(FriendCode, id, type, cancellationToken);

    public async Task<List<Record>> GetRecordsAsync(string title, ChartType type,
        CancellationToken cancellationToken = default) =>
        await Client!.GetRecordsAsync(FriendCode, title, type, cancellationToken);

    public async Task UploadRecordsAsync(List<Record> records, CancellationToken cancellationToken = default) =>
        await Client!.UploadRecordsAsync(FriendCode, records, cancellationToken);

    public async Task<List<Record>> GetRecentsAsync(CancellationToken cancellationToken = default) =>
        await Client!.GetRecentsAsync(FriendCode, cancellationToken);

    public async Task<List<SimpleRecord>> GetAllRecordsAsync(CancellationToken cancellationToken = default) =>
        await Client!.GetAllRecordsAsync(FriendCode, cancellationToken);

    public async Task<Dictionary<string, int>> GetHeatmapAsync(CancellationToken cancellationToken = default) =>
        await Client!.GetHeatmapAsync(FriendCode, cancellationToken);

    public async Task<List<RatingTrend>> GetDXRatingTrendAsync(int? version = null,
        CancellationToken cancellationToken = default) =>
        await Client!.GetDXRatingTrendAsync(FriendCode, version, cancellationToken);

    public async Task<List<Record>> GetHistoryAsync(int id, ChartType type, Difficulty difficulty,
        CancellationToken cancellationToken = default) =>
        await Client!.GetHistoryAsync(FriendCode, id, type, difficulty, cancellationToken);

    public async Task<NamePlate> GetNamePlateProgressAsync(int id, CancellationToken cancellationToken = default) =>
        await Client!.GetNamePlateProgressAsync(FriendCode, id, cancellationToken);

    public async Task UploadFromHtmlAsync(string html, CancellationToken cancellationToken = default) =>
        await Client!.UploadFromHtmlAsync(FriendCode, html, cancellationToken);
}
