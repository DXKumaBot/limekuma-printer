using Limekuma.Prober.Lxns.Enums;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using CommonChart = Limekuma.Prober.Common.Chart;
using CommonChartTypeEnum = Limekuma.Prober.Common.ChartType;
using CommonDifficultyEnum = Limekuma.Prober.Common.Difficulty;
using CommonNotes = Limekuma.Prober.Common.Notes;

namespace Limekuma.Prober.Lxns.Models;

public record Song
{
    private Lazy<ImmutableArray<CommonChart>>? _commonCharts;

    private Lazy<bool>? _inCurrentVersion;

    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("artist")]
    public required string Artist { get; init; }

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }

    [JsonPropertyName("bpm")]
    public required int Bpm { get; init; }

    [JsonPropertyName("map")]
    public string? Map { get; init; }

    [JsonPropertyName("version")]
    public required int VersionNumber { get; init; }

    [JsonPropertyName("rights")]
    public string? Rights { get; init; }

    [JsonPropertyName("locked")]
    public bool? Locked { get; init; } = false;

    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; } = false;

    [JsonPropertyName("difficulties")]
    public required Charts Charts { get; init; }

    [JsonIgnore]
    public Version Version
    {
        get
        {
            if (field is not null)
            {
                return field;
            }

            field = SongData.Shared.GetVersionFloor(VersionNumber);
            return field;
        }
    }

    [JsonIgnore]
    public bool InCurrentVersion => (_inCurrentVersion ??= new(() => SongData.Shared.Versions[^1].VersionNumber == Version.VersionNumber)).Value;

    [JsonIgnore]
    public string AudioUrl => field ??= $"https://assets2.lxns.net/maimai/music/{Id}.mp3";

    [JsonIgnore]
    public string JacketUrl => field ??= $"https://assets2.lxns.net/maimai/jacket/{Id}.png";

    [JsonIgnore]
    public ImmutableArray<CommonChart> CommonCharts => (_commonCharts ??= new(() => [.. Charts.Standard.Union(Charts.DX)
        .Union((Charts.Utage?.Select<UtageChart, Chart>(x => new()
        {
            Type = x.Type,
            Difficulty = x.Difficulty,
            Level = x.Level,
            LevelValue = x.LevelValue,
            Charter = x.Charter,
            VersionNumber = x.VersionNumber,
            Notes = x.Notes?.Value is CommonNotes notes
                ? notes
                : x.Notes?.Value is BuddyNotes buddyNotes
                    ? new CommonNotes
                    {
                        Total = buddyNotes.Player1.Total + buddyNotes.Player2.Total,
                        Tap = buddyNotes.Player1.Tap + buddyNotes.Player2.Tap,
                        Hold = buddyNotes.Player1.Hold + buddyNotes.Player2.Hold,
                        Slide = buddyNotes.Player1.Slide + buddyNotes.Player2.Slide,
                        Touch = buddyNotes.Player1.Touch + buddyNotes.Player2.Touch,
                        Break = buddyNotes.Player1.Break + buddyNotes.Player2.Break
                    }
                    : throw new NullReferenceException()
        })) ?? [])
        .AsParallel().Select(x =>
    {
        return new CommonChart
        {
            Song = new()
            {
                Id = x.Type is ChartType.Standard ? Id : Id + 10000,
                Title = Title,
                Type = (CommonChartTypeEnum)x.Type,
                VersionTitle = Version.Title,
                InCurrentVersion = InCurrentVersion,
                AudioUrl = AudioUrl,
                JacketUrl = JacketUrl
            },
            Difficulty = x.Type is ChartType.Utage
                ? CommonDifficultyEnum.Utage
                : MapDifficulty(x.Difficulty),
            TotalDXScore = x.TotalDXScore,
            Level = x.Level,
            LevelValue = x.LevelValue,
            Notes = x.Notes!
        };
    })])).Value;

    private static CommonDifficultyEnum MapDifficulty(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Basic => CommonDifficultyEnum.Basic,
        Difficulty.Advanced => CommonDifficultyEnum.Advanced,
        Difficulty.Expert => CommonDifficultyEnum.Expert,
        Difficulty.Master => CommonDifficultyEnum.Master,
        Difficulty.ReMaster => CommonDifficultyEnum.ReMaster,
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
    };
}
