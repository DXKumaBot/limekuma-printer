using Limekuma.Prober.Common;
using System.Collections.Frozen;

namespace Limekuma.Utils;

public static class ConstantMap
{
    private static readonly FrozenDictionary<Ranks, decimal> RatingFactors =
        new Dictionary<Ranks, decimal>
        {
            [Ranks.D] = 0,
            [Ranks.C] = 50,
            [Ranks.B] = 60,
            [Ranks.BB] = 70,
            [Ranks.BBB] = 75,
            [Ranks.A] = 80,
            [Ranks.AA] = 90,
            [Ranks.AAA] = 94,
            [Ranks.S] = 97,
            [Ranks.SPlus] = 98,
            [Ranks.SS] = 99,
            [Ranks.SSPlus] = 99.5m,
            [Ranks.SSS] = 100,
            [Ranks.SSSPlus] = 100.5m,
            [(Ranks)14] = 100.5m
        }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, FrozenSet<string>> GenreMap =
        new Dictionary<string, FrozenSet<string>>
        {
            ["真"] = ["maimai", "maimai PLUS"],
            ["超"] = ["maimai GreeN", "GreeN"],
            ["檄"] = ["maimai GreeN PLUS", "GreeN PLUS"],
            ["橙"] = ["maimai ORANGE", "ORANGE"],
            ["晓"] = ["maimai ORANGE PLUS", "ORANGE PLUS"],
            ["桃"] = ["maimai PiNK", "PiNK"],
            ["樱"] = ["maimai PiNK PLUS", "PiNK PLUS"],
            ["紫"] = ["maimai MURASAKi", "MURASAKi"],
            ["堇"] = ["maimai MURASAKi PLUS", "MURASAKi PLUS"],
            ["白"] = ["maimai MiLK", "MiLK"],
            ["雪"] = ["maimai MiLK PLUS", "MiLK PLUS"],
            ["辉"] = ["maimai FiNALE", "FiNALE"],
            ["熊"] = ["maimai でらっくす", "舞萌DX"],
            ["华"] = ["maimai でらっくす", "舞萌DX"],
            ["爽"] = ["maimai でらっくす Splash", "舞萌DX 2021"],
            ["煌"] = ["maimai でらっくす Splash", "舞萌DX 2021"],
            ["宙"] = ["maimai でらっくす UNiVERSE", "舞萌DX 2022"],
            ["星"] = ["maimai でらっくす UNiVERSE", "舞萌DX 2022"],
            ["祭"] = ["maimai でらっくす FESTiVAL", "舞萌DX 2023"],
            ["祝"] = ["maimai でらっくす FESTiVAL", "舞萌DX 2023"],
            ["双"] = ["maimai でらっくす BUDDiES", "舞萌DX 2024"],
            ["宴"] = ["maimai でらっくす BUDDiES", "舞萌DX 2024"],
            ["镜"] = ["maimai でらっくす PRiSM", "舞萌DX 2025"],
            ["彩"] = ["maimai でらっくす PRiSM", "舞萌DX 2025"],
            ["舞"] =
            [
                "maimai",
                "maimai PLUS",
                "maimai GreeN",
                "maimai GreeN PLUS",
                "maimai ORANGE",
                "maimai ORANGE PLUS",
                "maimai PiNK",
                "maimai PiNK PLUS",
                "maimai MURASAKi",
                "maimai MURASAKi PLUS",
                "maimai MiLK",
                "maimai MiLK PLUS",
                "maimai FiNALE",
                "GreeN",
                "GreeN PLUS",
                "ORANGE",
                "ORANGE PLUS",
                "PiNK",
                "PiNK PLUS",
                "MURASAKi",
                "MURASAKi PLUS",
                "MiLK",
                "MiLK PLUS",
                "FiNALE"
            ],
            ["霸"] =
            [
                "maimai",
                "maimai PLUS",
                "maimai GreeN",
                "maimai GreeN PLUS",
                "maimai ORANGE",
                "maimai ORANGE PLUS",
                "maimai PiNK",
                "maimai PiNK PLUS",
                "maimai MURASAKi",
                "maimai MURASAKi PLUS",
                "maimai MiLK",
                "maimai MiLK PLUS",
                "maimai FiNALE",
                "GreeN",
                "GreeN PLUS",
                "ORANGE",
                "ORANGE PLUS",
                "PiNK",
                "PiNK PLUS",
                "MURASAKi",
                "MURASAKi PLUS",
                "MiLK",
                "MiLK PLUS",
                "FiNALE"
            ]
        }.ToFrozenDictionary();

    public static decimal GetRatingMinAchievement(Ranks rank) => RatingFactors[rank];

    public static (Ranks, decimal, decimal) ResolveRankAndCoefficient(decimal achievements) => achievements switch
    {
        > 101 => throw new ArgumentOutOfRangeException(nameof(achievements), achievements,
            "Achievements cannot exceed 101%"),
        >= 100.5m => (Ranks.SSSPlus, 0.224m, 0.15m),
        >= 100.4999m => (Ranks.SSS, 0.222m, 0.14m),
        >= 100 => (Ranks.SSS, 0.216m, 0.14m),
        >= 99.9999m => (Ranks.SSPlus, 0.214m, 0.135m),
        >= 99.5m => (Ranks.SSPlus, 0.211m, 0.13m),
        >= 99 => (Ranks.SS, 0.208m, 0.12m),
        >= 98.9999m => (Ranks.SPlus, 0.206m, 0.11m),
        >= 98 => (Ranks.SPlus, 0.203m, 0.11m),
        >= 97 => (Ranks.S, 0.2m, 0.1m),
        >= 96.9999m => (Ranks.AAA, 0.176m, 0.094m),
        >= 94 => (Ranks.AAA, 0.168m, 0.094m),
        >= 90 => (Ranks.AA, 0.152m, 0.09m),
        >= 80 => (Ranks.A, 0.136m, 0.08m),
        >= 79.9999m => (Ranks.BBB, 0.128m, 0.08m),
        >= 75 => (Ranks.BBB, 0.12m, 0.075m),
        >= 70 => (Ranks.BB, 0.112m, 0.07m),
        >= 60 => (Ranks.B, 0.096m, 0.06m),
        >= 50 => (Ranks.C, 0.08m, 0.05m),
        >= 40 => (Ranks.D, 0.064m, 0.04m),
        >= 30 => (Ranks.D, 0.048m, 0.03m),
        >= 20 => (Ranks.D, 0.032m, 0.02m),
        >= 10 => (Ranks.D, 0.016m, 0.01m),
        >= 0 => (Ranks.D, 0, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(achievements), achievements,
            "Achievements must be a non-negative value")
    };
}
