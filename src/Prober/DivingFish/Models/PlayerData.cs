using System.Text.Json.Serialization;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonGradeRankEnum = Limekuma.Prober.Common.GradeRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonTitleColorEnum = Limekuma.Prober.Common.TitleColor;

namespace Limekuma.Prober.DivingFish.Models;

public record PlayerData
{
    [JsonPropertyName("additional_rating")]
    public required CommonGradeRankEnum GradeRank { get; set; }

    [JsonPropertyName("nickname")]
    public required string Name { get; set; }

    [JsonPropertyName("plate")]
    public required string PlateName { get; set; }

    [JsonPropertyName("rating")]
    public required int DXRating { get; set; }

    [JsonPropertyName("records")]
    public required List<Record> Records { get; set; }

    [JsonPropertyName("username")]
    public required string AccountName { get; set; }

    public static implicit operator CommonPlayer(PlayerData player) =>
        new()
        {
            Name = player.Name,
            DXRating = player.DXRating,
            TitleColor = CommonTitleColorEnum.Normal,
            Title = "なかよしmai友～！",
            ClassRank = CommonClassRankEnum.B5,
            GradeRank = player.GradeRank,
            IconId = 458001,
            FrameId = 558001,
            PlateId = 458001
        };
}
