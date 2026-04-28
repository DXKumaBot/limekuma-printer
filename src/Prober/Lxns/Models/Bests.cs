using System.Text.Json.Serialization;

namespace Limekuma.Prober.Lxns.Models;

public record Bests
{
    [JsonPropertyName("standard_total")]
    public required int EverDXRating { get; init; }

    [JsonPropertyName("dx_total")]
    public required int CurrentDXRating { get; init; }

    [JsonPropertyName("standard")]
    public required List<Record> Ever { get; init; }

    [JsonPropertyName("dx")]
    public required List<Record> Current { get; init; }
}
