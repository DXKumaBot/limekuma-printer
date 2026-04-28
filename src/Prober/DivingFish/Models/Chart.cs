using System.Text.Json.Serialization;
using CommonNotes = Limekuma.Prober.Common.Notes;

namespace Limekuma.Prober.DivingFish.Models;

public record Chart
{
    private Lazy<int>? _totalDXScore;

    [JsonPropertyName("notes")]
    public required List<int> NotesNumber { get; init; }

    [JsonPropertyName("charter")]
    public required string Charter { get; init; }

    [JsonIgnore]
    public CommonNotes Notes => field ??= new()
    {
        Total = NotesNumber.Sum(),
        Tap = NotesNumber[0],
        Hold = NotesNumber[1],
        Slide = NotesNumber[2],
        Touch = NotesNumber.Count > 4 ? NotesNumber[3] : 0,
        Break = NotesNumber.Count < 5 ? NotesNumber[3] : NotesNumber[4]
    };

    [JsonIgnore]
    public int TotalDXScore => (_totalDXScore ??= new(() => Notes.Total * 3)).Value;
}
