using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ChartType>))]
public enum ChartType
{
    [JsonStringEnumMemberName("SD")]
    Standard,

    [JsonStringEnumMemberName("DX")]
    DX
}
