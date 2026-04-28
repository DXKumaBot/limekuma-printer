using System.Text.Json.Serialization;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonGradeRankEnum = Limekuma.Prober.Common.GradeRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonProberTypeEnum = Limekuma.Prober.Common.ProberType;
using CommonTitleColorEnum = Limekuma.Prober.Common.TitleColor;

namespace Limekuma.Prober.DivingFish.Models;

public record Player
{
    [JsonPropertyName("username")]
    public required string AccountName { get; init; }

    [JsonPropertyName("rating")]
    public required int DXRating { get; init; }

    [JsonPropertyName("additional_rating")]
    public required CommonGradeRankEnum GradeRank { get; init; }

    [JsonPropertyName("nickname")]
    public required string Name { get; init; }

    [JsonPropertyName("plate")]
    public required string PlateName { get; init; }

    [JsonPropertyName("charts")]
    public required Bests Bests { get; init; }

    [JsonPropertyName("user_general_data")]
    [Obsolete("This field is always empty.")]
    public object? UserGeneralData { get; init; }

    public static implicit operator CommonPlayer(Player player) =>
        new()
        {
            Prober = CommonProberTypeEnum.DivingFish,
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
