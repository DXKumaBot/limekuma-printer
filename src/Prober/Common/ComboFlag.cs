using System.Text.Json.Serialization;

namespace Limekuma.Prober.Common;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<ComboFlag>))]
public enum ComboFlag
{
    [JsonIgnore]
    None = 0b0000_0000,

    [JsonIgnore]
    Plus = 0b0000_0001,

    [JsonStringEnumMemberName("fc")]
    FullCombo = 0b0000_0010,

    [JsonStringEnumMemberName("fcp")]
    FullComboPlus = FullCombo | Plus,

    [JsonStringEnumMemberName("ap")]
    AllPerfect = 0b0000_0100,

    [JsonStringEnumMemberName("app")]
    AllPerfectPlus = AllPerfect | Plus
}
