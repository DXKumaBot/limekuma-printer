using Limekuma.Prober.Lxns.Enums;
using System.Text.Json.Serialization;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonChartTypeEnum = Limekuma.Prober.Common.ChartType;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonDifficultyEnum = Limekuma.Prober.Common.Difficulty;
using CommonRecord = Limekuma.Prober.Common.Record;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

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
    public new CommonAchievementsRankEnum? Rank { get; init; }

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
            ChartType.Standard => Song.Charts.Standard[index],
            ChartType.DX => Song.Charts.DX[index],
            ChartType.Utage => Song.Charts.Utage![index],
            _ => throw new ArgumentOutOfRangeException()
        };
    })).Value;

    public int TotalDXScore => (_totalDXScore ??= new(() => Chart.Notes!.Total * 3)).Value;

    public decimal LevelValue => (_levelValue ??= new(() => Chart.LevelValue)).Value;

    private static CommonDifficultyEnum MapDifficulty(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Basic => CommonDifficultyEnum.Basic,
        Difficulty.Advanced => CommonDifficultyEnum.Advanced,
        Difficulty.Expert => CommonDifficultyEnum.Expert,
        Difficulty.Master => CommonDifficultyEnum.Master,
        Difficulty.ReMaster => CommonDifficultyEnum.ReMaster,
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
            record.Type is ChartType.Utage && ((UtageChart)record.Chart).IsBuddy ? 202 : 101);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DXScore, record.TotalDXScore);
        return new()
        {
            Chart = new()
            {
                Song = new()
                {
                    Id = record.Type is ChartType.Standard ? record.Id : record.Id + 10000,
                    Title = record.Title,
                    Type = (CommonChartTypeEnum)record.Type,
                    Genre = version.Title,
                    InCurrentGenre = inCurrentGenre,
                    AudioUrl = record.AudioUrl,
                    JacketUrl = record.JacketUrl
                },
                Difficulty = record.Type is ChartType.Utage
                    ? CommonDifficultyEnum.Utage
                    : MapDifficulty(record.Difficulty),
                TotalDXScore = record.TotalDXScore,
                Level = record.Level,
                LevelValue = record.LevelValue,
                Notes = chart.Notes!
            },
            ComboFlag = record.ComboFlag ?? CommonComboFlagEnum.None,
            SyncFlag = record.SyncFlag ?? CommonSyncFlagEnum.None,
            Rank = record.Rank ?? CommonAchievementsRankEnum.D,
            Achievements = record.Achievements,
            DXRating = (int)(record.DXRating ?? 0),
            DXScoreRank = record.DXScoreRank,
            DXScore = record.DXScore
        };
    }
}
