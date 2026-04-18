using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseStatus>))]
public enum ResponseStatus
{
    Success,

    [JsonStringEnumMemberName("error")]
    Error
}
