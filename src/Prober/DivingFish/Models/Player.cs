using System.Text.Json.Serialization;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonCourseRankEnum = Limekuma.Prober.Common.CourseRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonTrophyColorEnum = Limekuma.Prober.Common.TrophyColor;

namespace Limekuma.Prober.DivingFish.Models;

public record Player
{
    [JsonPropertyName("username")]
    public required string AccountName { get; set; }

    [JsonPropertyName("rating")]
    public required int DXRating { get; set; }

    [JsonPropertyName("additional_rating")]
    public required CommonCourseRankEnum CourseRank { get; set; }

    [JsonPropertyName("nickname")]
    public required string Name { get; set; }

    [JsonPropertyName("plate")]
    public required string PlateName { get; set; }

    [JsonPropertyName("charts")]
    public required Bests Bests { get; set; }

    [JsonPropertyName("user_general_data")]
    public required object? UserGeneralData { get; set; }

    public static implicit operator CommonPlayer(Player player) =>
        new()
        {
            Name = player.Name,
            DXRating = player.DXRating,
            TrophyColor = CommonTrophyColorEnum.Normal,
            TrophyText = "なかよしmai友～！",
            ClassRank = CommonClassRankEnum.B5,
            CourseRank = player.CourseRank,
            IconId = 458001,
            FrameId = 558001,
            PlateId = 458001
        };
}
