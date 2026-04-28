using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public record BasicInfo
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("artist")]
    public required string Artist { get; init; }

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }

    [JsonPropertyName("bpm")]
    public required int Bpm { get; init; }

    [JsonPropertyName("release_date")]
    [Obsolete("This field is always empty.")]
    public string? ReleaseDate { get; init; }

    [JsonPropertyName("from")]
    public required string Version { get; init; }

    [JsonPropertyName("is_new")]
    public required bool InCurrentVersion { get; init; }
}
