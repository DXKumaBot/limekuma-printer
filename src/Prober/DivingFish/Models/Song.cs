using Fractions;
using Limekuma.Prober.DivingFish.Enums;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using CommonChart = Limekuma.Prober.Common.Chart;
using CommonChartTypeEnum = Limekuma.Prober.Common.ChartType;
using CommonDifficultyEnum = Limekuma.Prober.Common.Difficulty;
using CommonSong = Limekuma.Prober.Common.Song;

namespace Limekuma.Prober.DivingFish.Models;

public record Song
{
    private Lazy<ImmutableArray<CommonChart>>? _commonCharts;

    private Lazy<int>? _id;

    [JsonPropertyName("id")]
    public required string IdString { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("type")]
    public required ChartType Type { get; init; }

    [JsonPropertyName("ds")]
    public required List<Fraction> LevelValues { get; init; }

    [JsonPropertyName("level")]
    public required List<string> Levels { get; init; }

    [JsonPropertyName("cids")]
    public required List<int> ChartIds { get; init; }

    [JsonPropertyName("charts")]
    public required List<Chart> Charts { get; init; }

    [JsonPropertyName("basic_info")]
    public required BasicInfo BasicInfo { get; init; }

    [JsonIgnore]
    public int Id => (_id ??= new(() =>
    {
        if (!int.TryParse(IdString, out int id))
        {
            throw new InvalidDataException();
        }

        return id;
    })).Value;

    [JsonIgnore]
    public string AudioUrl => field ??=
        $"https://assets2.lxns.net/maimai/music/{(Id is > 10000 and < 100000 ? Id % 10000 : Id)}.mp3";

    [JsonIgnore]
    public string JacketUrl => field ??= $"https://maimai.diving-fish.com/covers/{Id}.png";

    [JsonIgnore]
    public ImmutableArray<CommonChart> CommonCharts => (_commonCharts ??= new(() => [.. Charts.AsParallel().Select((x, i) => new CommonChart
    {
        Song = this,
        Difficulty = Id >= 100000 ? CommonDifficultyEnum.Utage : MapDifficulty(i),
        TotalDXScore = x.TotalDXScore,
        Level = Levels[i],
        LevelValue = LevelValues[i],
        Notes = x.Notes
    })])).Value;

    private static CommonDifficultyEnum MapDifficulty(int index) => index switch
    {
        0 => CommonDifficultyEnum.Basic,
        1 => CommonDifficultyEnum.Advanced,
        2 => CommonDifficultyEnum.Expert,
        3 => CommonDifficultyEnum.Master,
        4 => CommonDifficultyEnum.ReMaster,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
    };

    public static implicit operator CommonSong(Song song) => new()
    {
        Id = song.Id,
        Title = song.Title,
        Type = song.Id >= 100000 ? CommonChartTypeEnum.Utage : (CommonChartTypeEnum)song.Type,
        VersionTitle = song.BasicInfo.Version,
        InCurrentVersion = song.BasicInfo.InCurrentVersion,
        AudioUrl = song.AudioUrl,
        JacketUrl = song.JacketUrl
    };
}
