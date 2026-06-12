using Fractions;
using Limekuma.Prober.Common;
using System.Collections.Frozen;

namespace Limekuma.Utils;

public static class ConstantMap
{
    private static readonly FrozenDictionary<AchievementsRank, Fraction> RatingFactors =
        new Dictionary<AchievementsRank, Fraction>
        {
            [AchievementsRank.D] = 0,
            [AchievementsRank.C] = 50,
            [AchievementsRank.B] = 60,
            [AchievementsRank.BB] = 70,
            [AchievementsRank.BBB] = 75,
            [AchievementsRank.A] = 80,
            [AchievementsRank.AA] = 90,
            [AchievementsRank.AAA] = 94,
            [AchievementsRank.S] = 97,
            [AchievementsRank.SPlus] = 98,
            [AchievementsRank.SS] = 99,
            [AchievementsRank.SSPlus] = new(199, 2),
            [AchievementsRank.SSS] = 100,
            [AchievementsRank.SSSPlus] = new(201, 2),
            [(AchievementsRank)14] = new(201, 2)
        }.ToFrozenDictionary();

    public static readonly FrozenDictionary<string, FrozenSet<string>> VersionMap =
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
            ["爽"] = ["maimai でらっくす PLUS", "maimai でらっくす Splash", "舞萌DX 2021"],
            ["煌"] = ["maimai でらっくす PLUS", "maimai でらっくす Splash", "舞萌DX 2021"],
            ["宙"] = ["maimai でらっくす Splash PLUS", "maimai でらっくす UNiVERSE", "舞萌DX 2022"],
            ["星"] = ["maimai でらっくす Splash PLUS", "maimai でらっくす UNiVERSE", "舞萌DX 2022"],
            ["祭"] = ["maimai でらっくす UNiVERSE PLUS", "maimai でらっくす FESTiVAL", "舞萌DX 2023"],
            ["祝"] = ["maimai でらっくす UNiVERSE PLUS", "maimai でらっくす FESTiVAL", "舞萌DX 2023"],
            ["双"] = ["maimai でらっくす FESTiVAL PLUS", "maimai でらっくす BUDDiES", "舞萌DX 2024"],
            ["宴"] = ["maimai でらっくす FESTiVAL PLUS", "maimai でらっくす BUDDiES", "舞萌DX 2024"],
            ["镜"] = ["maimai でらっくす BUDDiES PLUS", "maimai でらっくす PRiSM", "舞萌DX 2025"],
            ["彩"] = ["maimai でらっくす PRiSM PLUS", "舞萌DX 2026"],
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

    public static Fraction GetRatingMinAchievement(AchievementsRank rank) => RatingFactors[rank];

    public static (AchievementsRank, Fraction, Fraction)
        ResolveRankAndCoefficient(Fraction achievements)
    {
        if (achievements > 101)
        {
            throw new ArgumentOutOfRangeException(nameof(achievements), achievements,
                "Achievements cannot exceed 101%");
        }

        if (achievements >= new Fraction(201, 2)) return (AchievementsRank.SSSPlus, new(28, 125), new(3, 20));
        if (achievements >= new Fraction(1004999, 10000)) return (AchievementsRank.SSS, new(111, 500), new(7, 50));
        if (achievements >= 100) return (AchievementsRank.SSS, new(27, 125), new(7, 50));
        if (achievements >= new Fraction(999999, 10000)) return (AchievementsRank.SSPlus, new(107, 500), new(27, 200));
        if (achievements >= new Fraction(199, 2)) return (AchievementsRank.SSPlus, new(211, 1000), new(13, 100));
        if (achievements >= 99) return (AchievementsRank.SS, new(26, 125), new(3, 25));
        if (achievements >= new Fraction(989999, 10000)) return (AchievementsRank.SPlus, new(103, 500), new(11, 100));
        if (achievements >= 98) return (AchievementsRank.SPlus, new(203, 1000), new(11, 100));
        if (achievements >= 97) return (AchievementsRank.S, new(1, 5), new(1, 10));
        if (achievements >= new Fraction(969999, 10000)) return (AchievementsRank.AAA, new(22, 125), new(47, 500));
        if (achievements >= 94) return (AchievementsRank.AAA, new(21, 125), new(47, 500));
        if (achievements >= 90) return (AchievementsRank.AA, new(19, 125), new(9, 100));
        if (achievements >= 80) return (AchievementsRank.A, new(17, 125), new(2, 25));
        if (achievements >= new Fraction(799999, 10000)) return (AchievementsRank.BBB, new(16, 125), new(2, 25));
        if (achievements >= 75) return (AchievementsRank.BBB, new(3, 25), new(3, 40));
        if (achievements >= 70) return (AchievementsRank.BB, new(14, 125), new(7, 100));
        if (achievements >= 60) return (AchievementsRank.B, new(12, 125), new(3, 50));
        if (achievements >= 50) return (AchievementsRank.C, new(2, 25), new(1, 20));
        if (achievements >= 40) return (AchievementsRank.D, new(8, 125), new(1, 25));
        if (achievements >= 30) return (AchievementsRank.D, new(6, 125), new(3, 100));
        if (achievements >= 20) return (AchievementsRank.D, new(4, 125), new(1, 50));
        if (achievements >= 10) return (AchievementsRank.D, new(2, 125), new(1, 100));
        if (achievements >= 0) return (AchievementsRank.D, Fraction.Zero, Fraction.Zero);

        throw new ArgumentOutOfRangeException(nameof(achievements), achievements,
            "Achievements must be a non-negative value");
    }
}
