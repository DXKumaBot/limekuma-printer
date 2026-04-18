using System.Text.Json.Serialization;
using CommonNotes = Limekuma.Prober.Common.Notes;

namespace Limekuma.Prober.DivingFish.Models;

public record Chart
{
    private Lazy<CommonNotes>? _notes;

    [JsonPropertyName("notes")]
    public required List<int> NotesNumber { get; set; }

    [JsonPropertyName("charter")]
    public required string CharterName { get; set; }

    [JsonIgnore]
    public CommonNotes Notes => (_notes ??= new(() => new()
    {
        Total = NotesNumber.Sum(),
        Tap = NotesNumber[0],
        Hold = NotesNumber[1],
        Slide = NotesNumber[2],
        Touch = NotesNumber.Count > 4 ? NotesNumber[3] : 0,
        Break = NotesNumber.Count < 5 ? NotesNumber[3] : NotesNumber[4]
    })).Value;
}
