using Limekuma.Prober.Common;
using System.Collections.Frozen;

namespace Limekuma.Utils;

public static class ConstantMap
{
    private static readonly FrozenDictionary<Ranks, float> RatingFactors =
        new Dictionary<Ranks, float>
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
            [Ranks.SSPlus] = 99.5f,
            [Ranks.SSS] = 100,
            [Ranks.SSSPlus] = 100.5f,
            [(Ranks)14] = 100.5f
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

    public static float GetRatingMinAchievement(Ranks rank) => RatingFactors[rank];

    public static (Ranks, float, float) ResolveRankAndCoefficient(double achievements) => achievements switch
    {
        > 101 => throw new ArgumentOutOfRangeException(),
        >= 100.5 => (Ranks.SSSPlus, 0.224f, 0.15f),
        >= 100.4999 => (Ranks.SSS, 0.222f, 0.14f),
        >= 100 => (Ranks.SSS, 0.216f, 0.14f),
        >= 99.9999 => (Ranks.SSPlus, 0.214f, 0.135f),
        >= 99.5f => (Ranks.SSPlus, 0.211f, 0.13f),
        >= 99 => (Ranks.SS, 0.208f, 0.12f),
        >= 98.9999 => (Ranks.SPlus, 0.206f, 0.11f),
        >= 98 => (Ranks.SPlus, 0.203f, 0.11f),
        >= 97 => (Ranks.S, 0.2f, 0.1f),
        >= 96.9999 => (Ranks.AAA, 0.176f, 0.094f),
        >= 94 => (Ranks.AAA, 0.168f, 0.094f),
        >= 90 => (Ranks.AA, 0.152f, 0.09f),
        >= 80 => (Ranks.A, 0.136f, 0.08f),
        >= 79.9999 => (Ranks.BBB, 0.128f, 0.08f),
        >= 75 => (Ranks.BBB, 0.12f, 0.075f),
        >= 70 => (Ranks.BB, 0.112f, 0.07f),
        >= 60 => (Ranks.B, 0.096f, 0.06f),
        >= 50 => (Ranks.C, 0.08f, 0.05f),
        >= 40 => (Ranks.D, 0.064f, 0.04f),
        >= 30 => (Ranks.D, 0.048f, 0.03f),
        >= 20 => (Ranks.D, 0.032f, 0.02f),
        >= 10 => (Ranks.D, 0.016f, 0.01f),
        >= 0 => (Ranks.D, 0, 0),
        _ => throw new ArgumentOutOfRangeException()
    };
}
