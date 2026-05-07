using Fractions;
using Limekuma.Prober.DivingFish.Enums;
using Limekuma.Utils;
using System.Text.Json.Serialization;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonRecord = Limekuma.Prober.Common.Record;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;

namespace Limekuma.Prober.DivingFish.Models;

public class Record
{
    private Lazy<int>? _dxScoreRank;

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
    public required Fraction LevelValue
    {
        get;
        init
        {
            if (value < 0 || value > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "LevelValue out of range.");
            }

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

    [JsonIgnore]
    public Song Song
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            Songs songData = Songs.SharedSongs;
            field = songData.SongsById[Id];
            return field;
        }
    }

    [JsonIgnore]
    public Chart Chart => field ??= Song.Charts[DifficultyIndex];

    [JsonIgnore]
    public int DXScoreRank => (_dxScoreRank ??= new(() =>
    {
        Fraction ratio = DXScore / Chart.TotalDXScore;
        if (ratio < new Fraction(17, 20)) return 0;
        if (ratio < new Fraction(9, 10)) return 1;
        if (ratio < new Fraction(93, 100)) return 2;
        if (ratio < new Fraction(19, 20)) return 3;
        if (ratio < new Fraction(97, 100)) return 4;
        return ratio <= 1 ? 5 : 0;
    })).Value;

    public static implicit operator CommonRecord(Record record)
    {
        Song song = record.Song;

        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.Achievements,
            (record.Difficulty is Difficulty.Utage && song.ChartIds.Count > 1) ? 202 : 101);
        // ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DXScore, record.TotalDXScore);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(record.DifficultyIndex, song.ChartIds.Count);
        return new()
        {
            Chart = song.CommonCharts[record.DifficultyIndex],
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
