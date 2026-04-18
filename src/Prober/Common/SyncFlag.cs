using System.Text.Json.Serialization;

namespace Limekuma.Prober.Common;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<SyncFlag>))]
public enum SyncFlag
{
    [JsonIgnore]
    None = 0b0000_0000,

    [JsonStringEnumMemberName("sync")]
    SyncPlay = 0b0000_0001,

    [JsonIgnore]
    Full = 0b0000_0010,

    [JsonStringEnumMemberName("fs")]
    FullSync = Full | SyncPlay,

    [JsonIgnore]
    Plus = 0b0000_0100,

    [JsonStringEnumMemberName("fsp")]
    FullSyncPlus = Full | SyncPlay | Plus,

    [JsonIgnore]
    DX = 0b0000_1000,

    [JsonStringEnumMemberName("fsd")]
    FullSyncDX = Full | SyncPlay | DX,

    [JsonStringEnumMemberName("fsdp")]
    FullSyncDXPlus = Full | SyncPlay | DX | Plus
}
