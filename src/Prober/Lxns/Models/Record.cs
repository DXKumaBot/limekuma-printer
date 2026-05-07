using Fractions;
using Limekuma.Prober.Lxns.Enums;
using Limekuma.Utils;
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
    [JsonPropertyName("achievements")]
    public required Fraction Achievements
    {
        get;
        init
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Achievements cannot be negative.");
            }

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
    public Fraction? DXRating
    {
        get;
        init
        {
            Fraction ratingValue = value ?? throw new ArgumentNullException(nameof(value));
            if (ratingValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "DXRating cannot be negative.");
            }

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

    [JsonIgnore]
    public Song Song
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            SongData songData = SongData.Shared;
            field = songData.SongsById[Id];
            return field;
        }
    }

    [JsonIgnore]
    public Chart Chart
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            int index = (int)Difficulty;
            field = Type switch
            {
                ChartType.Standard => Song.Charts.Standard[index],
                ChartType.DX => Song.Charts.DX[index],
                ChartType.Utage => Song.Charts.Utage![index],
                _ => throw new ArgumentOutOfRangeException()
            };

            return field;
        }
    }

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
        Song song = record.Song;
        Chart chart = record.Chart;
        SongData songData = SongData.Shared;
        int versionGroup = chart.VersionNumber - (chart.VersionNumber % 500);
        Version version = songData.VersionsByGroup[versionGroup];
        bool inCurrentVersion = songData.Versions[^1].VersionNumber == version.VersionNumber;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.Achievements,
            record.Type is ChartType.Utage && ((UtageChart)record.Chart).IsBuddy ? 202 : 101);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DXScore, chart.TotalDXScore);
        return new()
        {
            Chart = new()
            {
                Song = new()
                {
                    Id = record.Type is ChartType.Standard ? record.Id : record.Id + 10000,
                    Title = record.Title,
                    Type = (CommonChartTypeEnum)record.Type,
                    VersionTitle = version.Title,
                    InCurrentVersion = inCurrentVersion,
                    AudioUrl = song.AudioUrl,
                    JacketUrl = song.JacketUrl
                },
                Difficulty = record.Type is ChartType.Utage
                    ? CommonDifficultyEnum.Utage
                    : MapDifficulty(record.Difficulty),
                TotalDXScore = chart.TotalDXScore,
                Level = record.Level,
                LevelValue = chart.LevelValue,
                Notes = chart.Notes!
            },
            ComboFlag = record.ComboFlag ?? CommonComboFlagEnum.None,
            SyncFlag = record.SyncFlag ?? CommonSyncFlagEnum.None,
            Rank = record.Rank ?? CommonAchievementsRankEnum.D,
            Achievements = record.Achievements,
            DXRating = (record.DXRating ?? Fraction.Zero).ToInt32Truncated(),
            DXScoreRank = record.DXScoreRank,
            DXScore = record.DXScore
        };
    }
}
