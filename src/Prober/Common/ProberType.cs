using System.Text.Json.Serialization;

namespace Limekuma.Prober.Common;

[JsonConverter(typeof(JsonStringEnumConverter<ProberType>))]
public enum ProberType
{
    [JsonStringEnumMemberName("lxns")]
    Lxns,

    [JsonStringEnumMemberName("diving-fish")]
    DivingFish
}
