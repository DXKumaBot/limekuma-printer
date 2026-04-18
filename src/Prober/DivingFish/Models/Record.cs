using Limekuma.Prober.DivingFish.Enums;
using Limekuma.Utils;
using System.Text.Json.Serialization;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonDifficultyEnum = Limekuma.Prober.Common.Difficulty;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonRecord = Limekuma.Prober.Common.Record;
using CommonChartTypeEnum = Limekuma.Prober.Common.ChartType;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

namespace Limekuma.Prober.DivingFish.Models;

public class Record
{
    private Lazy<Chart>? _chart;

    private Lazy<int>? _dxScoreRank;

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

    [JsonPropertyName("cid")]
    public int? ChartId
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value ?? throw new ArgumentNullException());
            field = value;
        }
    }

    [JsonPropertyName("ds")]
    public required decimal LevelValue
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 15);
            field = value;
        }
    }

    [JsonPropertyName("dxScore")]
    public required int DXScore
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("fc")]
    public required Union<CommonComboFlagEnum, string> ComboFlag { get; init; }

    [JsonPropertyName("fs")]
    public required Union<CommonSyncFlagEnum, string> SyncFlag { get; init; }

    [JsonPropertyName("level")]
    public required string Level { get; init; }

    [JsonPropertyName("level_index")]
    public required int DifficultyIndex
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("level_label")]
    public required Difficulty Difficulty { get; init; }

    [JsonPropertyName("ra")]
    public required int DXRating
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("rate")]
    public required CommonAchievementsRankEnum Rank { get; init; }

    [JsonPropertyName("song_id")]
    public required int Id
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            field = value;
        }
    }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("type")]
    public required ChartType Type { get; init; }

    public string AudioUrl =>
        $"https://assets2.lxns.net/maimai/music/{(Id is > 10000 and < 100000 ? Id % 10000 : Id)}.mp3";

    public string JacketUrl => $"https://maimai.diving-fish.com/covers/{Id}.png";

    public Song Song => (_song ??= new(() =>
    {
        Songs songData = Songs.SharedSongs;
        if (!songData.SongsById.TryGetValue(Id.ToString(), out Song? song))
        {
            throw new KeyNotFoundException($"Song with ID {Id} not found");
        }

        return song;
    })).Value;

    public Chart Chart => (_chart ??= new(() => Song.Charts[DifficultyIndex])).Value;

    public int TotalDXScore => (_totalDXScore ??= new(() => Song.Charts[DifficultyIndex].Notes.Total * 3)).Value;

    public int DXScoreRank => (_dxScoreRank ??= new(() => ((decimal)DXScore / TotalDXScore) switch
    {
        < 0.85m => 0,
        < 0.9m => 1,
        < 0.93m => 2,
        < 0.95m => 3,
        < 0.97m => 4,
        <= 1 => 5,
        _ => 0
    })).Value;

    private static CommonDifficultyEnum MapDifficulty(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Dummy => CommonDifficultyEnum.Dummy,
        Difficulty.Basic => CommonDifficultyEnum.Basic,
        Difficulty.Advanced => CommonDifficultyEnum.Advanced,
        Difficulty.Expert => CommonDifficultyEnum.Expert,
        Difficulty.Master => CommonDifficultyEnum.Master,
        Difficulty.ReMaster => CommonDifficultyEnum.ReMaster,
        Difficulty.Utage => CommonDifficultyEnum.Utage,
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
    };

    public static implicit operator CommonRecord(Record record)
    {
        Song song = record.Song;
        Chart chart = record.Chart;
        BasicInfo basicInfo = song.BasicInfo;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.Achievements,
            (record.Difficulty is Difficulty.Utage && record.Song.Charts.Count > 1) ? 202 : 101);
        // ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DXScore, record.TotalDXScore);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DifficultyIndex, record.Song.Charts.Count);
        return new()
        {
            Chart = new()
            {
                Song = new()
                {
                    Id = record.Id,
                    Title = record.Title,
                    Type = record.Difficulty is Difficulty.Utage
                        ? CommonChartTypeEnum.Utage
                        : (CommonChartTypeEnum)record.Type,
                    Genre = basicInfo.Genre,
                    InCurrentGenre = basicInfo.InCurrentVersion,
                    AudioUrl = record.AudioUrl,
                    JacketUrl = record.JacketUrl
                },
                Difficulty = MapDifficulty(record.Difficulty),
                TotalDXScore = record.TotalDXScore,
                Level = record.Level,
                LevelValue = record.LevelValue,
                Notes = chart.Notes
            },
            ComboFlag = record.ComboFlag,
            SyncFlag = record.SyncFlag,
            Rank = record.Rank,
            Achievements = record.Achievements,
            DXRating = record.DXRating,
            DXScoreRank = record.DXScoreRank,
            DXScore = record.DXScore
        };
    }
}
