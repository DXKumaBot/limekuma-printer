using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record Bests
{
    [JsonPropertyName("sd")]
    public required List<Record> Ever { get; init; }

    [JsonPropertyName("dx")]
    public required List<Record> Current { get; init; }
}
