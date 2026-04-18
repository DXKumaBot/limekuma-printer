using System.Text.Json.Serialization;

namespace Limekuma.Prober.Lxns.Models;

public record Bests
{
    [JsonPropertyName("standard_total")]
    public required int EverDXRating { get; set; }

    [JsonPropertyName("dx_total")]
    public required int CurrentDXRating { get; set; }

    [JsonPropertyName("standard")]
    public required List<Record> Ever { get; set; }

    [JsonPropertyName("dx")]
    public required List<Record> Current { get; set; }
}
