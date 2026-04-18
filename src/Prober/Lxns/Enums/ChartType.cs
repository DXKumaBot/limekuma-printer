using System.Text.Json.Serialization;

namespace Limekuma.Prober.Lxns.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ChartType>))]
public enum ChartType
{
    [JsonStringEnumMemberName("standard")]
    Standard,

    [JsonStringEnumMemberName("dx")]
    DX,

    [JsonStringEnumMemberName("utage")]
    Utage
}
