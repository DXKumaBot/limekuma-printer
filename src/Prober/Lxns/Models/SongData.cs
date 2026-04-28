using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using CommonChart = Limekuma.Prober.Common.Chart;

namespace Limekuma.Prober.Lxns.Models;

public record SongData
{
    private Lazy<ImmutableArray<CommonChart>>? _charts;
    private readonly DateTimeOffset _pullTime = DateTimeOffset.Now;

    [JsonPropertyName("songs")]
    public required List<Song> Songs { get; init; }

    [JsonPropertyName("genres")]
    public required List<Genre> Genres { get; init; }

    [JsonPropertyName("versions")]
    public required List<Version> Versions { get; init; }

    [JsonIgnore]
    public FrozenDictionary<int, Song> SongsById => field ??= Songs.ToFrozenDictionary(x => x.Id);

    [JsonIgnore]
    public FrozenDictionary<int, Version> VersionsByGroup => field ??=
        Versions.AsParallel().GroupBy(x => x.VersionNumber).ToFrozenDictionary(x => x.Key, x => x.First());

    [JsonIgnore]
    public ImmutableArray<CommonChart> Charts => (_charts ??= new(() => Songs.AsParallel().SelectMany(x => x.CommonCharts).ToImmutableArray())).Value;

    public static SongData Shared
    {
        get
        {
            if (field is not null && DateTimeOffset.Now.AddHours(10).Date == field._pullTime.AddHours(10).Date)
            {
                return field;
            }

            LxnsResourceClient resource = new();
            field = resource.GetSongsAsync(includeNotes: true).Result;

            return field;
        }
    }
}
