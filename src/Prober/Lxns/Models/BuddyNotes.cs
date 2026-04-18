using System.Text.Json.Serialization;
using CommonNotes = Limekuma.Prober.Common.Notes;

namespace Limekuma.Prober.Lxns.Models;

public record BuddyNotes
{
    [JsonPropertyName("left")]
    public required CommonNotes Player1 { get; init; }

    [JsonPropertyName("right")]
    public required CommonNotes Player2 { get; init; }
}
