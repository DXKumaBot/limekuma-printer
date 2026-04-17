using Limekuma.Prober.Common;
using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;

namespace Limekuma.Prober.Lxns.Models;

public record Record : SimpleRecord
{
    private Lazy<Chart>? _chart;

    private Lazy<decimal>? _levelValue;

    private Lazy<Song>? _song;

    private Lazy<int>? _totalDXScore;

    [JsonPropertyName("achievements")]
    public required decimal Achievements
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("dx_score")]
    public required int DXScore
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("dx_star")]
    public required int DXScoreRank
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 5);
            field = value;
        }
    }

    [JsonPropertyName("dx_rating")]
    public decimal? DXRating
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value ?? throw new ArgumentNullException(nameof(value)));
            field = value;
        }
    }

    [JsonPropertyName("rate")]
    public new Ranks? Rank { get; init; }

    [JsonPropertyName("play_time")]
    public DateTimeOffset? PlayTime { get; init; }

    [JsonPropertyName("upload_time")]
    public DateTimeOffset? UploadTime { get; init; }

    [JsonPropertyName("last_played_time")]
    public DateTimeOffset? LastPlayedTime { get; init; }

    public string AudioUrl => $"https://assets2.lxns.net/maimai/music/{Id}.mp3";

    public string JacketUrl => $"https://assets2.lxns.net/maimai/jacket/{Id}.png";

    public Song Song => (_song ??= new(() =>
    {
        SongData songData = SongData.Shared;
        if (!songData.SongsById.TryGetValue(Id, out Song? song))
        {
            throw new KeyNotFoundException($"Song with ID {Id} not found");
        }

        return song;
    })).Value;

    public Chart Chart => (_chart ??= new(() =>
    {
        int index = (int)Difficulty;
        return Type switch
        {
            SongTypes.Standard => Song.Charts.Standard[index],
            SongTypes.DX => Song.Charts.DX[index],
            SongTypes.Utage => Song.Charts.Utage![index],
            _ => throw new ArgumentOutOfRangeException()
        };
    })).Value;

    public int TotalDXScore => (_totalDXScore ??= new(() => Chart.Notes!.Total * 3)).Value;

    public decimal LevelValue => (_levelValue ??= new(() => Chart.LevelValue)).Value;

    private static CommonDifficulties MapDifficulty(Difficulties difficulty) => difficulty switch
    {
        Difficulties.Dummy => CommonDifficulties.Dummy,
        Difficulties.Basic => CommonDifficulties.Basic,
        Difficulties.Advanced => CommonDifficulties.Advanced,
        Difficulties.Expert => CommonDifficulties.Expert,
        Difficulties.Master => CommonDifficulties.Master,
        Difficulties.ReMaster => CommonDifficulties.ReMaster,
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
    };

    public static implicit operator CommonRecord(Record record)
    {
        Chart chart = record.Chart;
        SongData songData = SongData.Shared;
        int versionGroup = chart.Version - (chart.Version % 500);
        if (!songData.VersionsByGroup.TryGetValue(versionGroup, out Version? version))
        {
            throw new KeyNotFoundException($"Version group {versionGroup} not found");
        }

        bool inCurrentGenre = songData.Versions[^1].VersionNumber == versionGroup;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.Achievements,
            record.Type is SongTypes.Utage && ((UtageChart)record.Chart).IsBuddy ? 202 : 101);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DXScore, record.TotalDXScore);
        return new()
        {
            Chart = new()
            {
                Song = new()
                {
                    Id = record.Type is SongTypes.Standard ? record.Id : record.Id + 10000,
                    Title = record.Title,
                    Type = (CommonSongTypes)record.Type,
                    Genre = version.Title,
                    InCurrentGenre = inCurrentGenre,
                    AudioUrl = record.AudioUrl,
                    JacketUrl = record.JacketUrl
                },
                Difficulty = record.Type is SongTypes.Utage
                    ? CommonDifficulties.Utage
                    : MapDifficulty(record.Difficulty),
                TotalDXScore = record.TotalDXScore,
                Level = record.Level,
                LevelValue = record.LevelValue,
                Notes = chart.Notes!
            },
            ComboFlag = record.ComboFlag ?? ComboFlags.None,
            SyncFlag = record.SyncFlag ?? SyncFlags.None,
            Rank = record.Rank ?? Ranks.D,
            Achievements = record.Achievements,
            DXRating = (int)(record.DXRating ?? 0),
            DXScoreRank = record.DXScoreRank,
            DXScore = record.DXScore
        };
    }
}
