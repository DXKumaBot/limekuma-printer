using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonNotes = Limekuma.Prober.Common.Notes;

namespace Limekuma.Prober.Lxns.Models;

public record Chart
{
    [JsonPropertyName("type")]
    public required ChartType Type { get; init; }

    [JsonPropertyName("difficulty")]
    public required Difficulty Difficulty { get; init; }

    [JsonPropertyName("level")]
    public required string Level { get; init; }

    [JsonPropertyName("level_value")]
    public required decimal LevelValue { get; init; }

    [JsonPropertyName("note_designer")]
    public required string CharterName { get; init; }

    [JsonPropertyName("version")]
    public required int Version { get; init; }

    [JsonPropertyName("notes")]
    public CommonNotes? Notes { get; init; }
}
