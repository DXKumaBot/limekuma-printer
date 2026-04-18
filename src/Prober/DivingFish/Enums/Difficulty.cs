using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<Difficulty>))]
public enum Difficulty
{
    Dummy,
    Basic,
    Advanced,
    Expert,
    Master,

    [JsonStringEnumMemberName("Re:MASTER")]
    ReMaster,
    Utage
}
