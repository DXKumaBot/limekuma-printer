using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonCourseRankEnum = Limekuma.Prober.Common.CourseRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonTrophyColorEnum = Limekuma.Prober.Common.TrophyColor;

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
    public required CourseRank CourseRank { get; set; }

    [JsonPropertyName("class_rank")]
    public required CommonClassRankEnum ClassRank { get; set; }

    [JsonPropertyName("star")]
    public required int Star { get; set; }

    [JsonPropertyName("trophy")]
    public Trophy? Trophy { get; set; }

    [JsonPropertyName("icon")]
    public Icon? Icon { get; set; }

    [JsonPropertyName("name_plate")]
    public NamePlate? NamePlate { get; set; }

    [JsonPropertyName("frame")]
    public Frame? Frame { get; set; }

    [JsonPropertyName("upload_time")]
    public DateTimeOffset? UploadTime { get; set; }

    private static CommonCourseRankEnum MapCourseRank(CourseRank courseRank) => courseRank switch
    {
        CourseRank.Shoshinsha => CommonCourseRankEnum.Shoshinsha,
        CourseRank.Shodan => CommonCourseRankEnum.Shodan,
        CourseRank.Nidan => CommonCourseRankEnum.Nidan,
        CourseRank.Sandan => CommonCourseRankEnum.Sandan,
        CourseRank.Yondan => CommonCourseRankEnum.Yondan,
        CourseRank.Godan => CommonCourseRankEnum.Godan,
        CourseRank.Rokudan => CommonCourseRankEnum.Rokudan,
        CourseRank.Shichidan => CommonCourseRankEnum.Shichidan,
        CourseRank.Hachidan => CommonCourseRankEnum.Hachidan,
        CourseRank.Kyudan => CommonCourseRankEnum.Kyudan,
        CourseRank.Judan => CommonCourseRankEnum.Judan,
        CourseRank.Shinshodan => CommonCourseRankEnum.Shinshodan,
        CourseRank.Shinnidan => CommonCourseRankEnum.Shinnidan,
        CourseRank.Shinsandan => CommonCourseRankEnum.Shinsandan,
        CourseRank.Shinyondan => CommonCourseRankEnum.Shinyondan,
        CourseRank.Shingodan => CommonCourseRankEnum.Shingodan,
        CourseRank.Shinrokudan => CommonCourseRankEnum.Shinrokudan,
        CourseRank.Shinshichidan => CommonCourseRankEnum.Shinshichidan,
        CourseRank.Shinhachidan => CommonCourseRankEnum.Shinhachidan,
        CourseRank.Shinkyudan => CommonCourseRankEnum.Shinkyudan,
        CourseRank.Shinjudan => CommonCourseRankEnum.Shinjudan,
        CourseRank.Shinkaiden => CommonCourseRankEnum.Shinkaiden,
        CourseRank.Urakaiden => CommonCourseRankEnum.Urakaiden,
        _ => throw new ArgumentOutOfRangeException(nameof(courseRank), courseRank, null)
    };

    public static implicit operator CommonPlayer(Player player) =>
        new()
        {
            Name = player.Name,
            DXRating = player.DXRating,
            TrophyColor = player.Trophy?.Color ?? CommonTrophyColorEnum.Normal,
            TrophyText = player.Trophy?.Name ?? "なかよしmai友～！",
            ClassRank = player.ClassRank,
            CourseRank = MapCourseRank(player.CourseRank),
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
