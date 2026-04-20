using System.Text.Json.Serialization;

namespace Limekuma.Prober.Common;

[JsonConverter(typeof(JsonStringEnumConverter<Difficulty>))]
public enum Difficulty
{
    Dummy,

    [JsonStringEnumMemberName("绿")]
    Basic,

    [JsonStringEnumMemberName("黄")]
    Advanced,

    [JsonStringEnumMemberName("红")]
    Expert,

    [JsonStringEnumMemberName("紫")]
    Master,

    [JsonStringEnumMemberName("白")]
    ReMaster,

    [JsonStringEnumMemberName("宴")]
    Utage
}
