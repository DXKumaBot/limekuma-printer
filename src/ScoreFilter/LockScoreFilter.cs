using Fractions;
using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("lock", true)]
public sealed class LockScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition) => _ => true;

    public Func<Record, bool> GetFilter(string? condition) => x =>
    {
        Fraction sumScore = (x.Chart.Notes.Tap + x.Chart.Notes.Touch + (x.Chart.Notes.Hold * 2) +
                             (x.Chart.Notes.Slide * 3) + (x.Chart.Notes.Break * 5)) * 5;
        Fraction scoreBound = (Fraction.One - ((sumScore - Fraction.One) / sumScore)) * 100;
        Fraction breakBound = x.Chart.Notes.Break > 0 ? Fraction.One / x.Chart.Notes.Break / 2 : 101;
        Fraction minScore = scoreBound <= breakBound ? scoreBound : breakBound;
        Fraction minAcc = ConstantMap.GetRatingMinAchievement(x.Rank);
        Fraction maxAcc = minAcc + minScore;
        Fraction achievements = x.Achievements;
        return achievements >= minAcc && achievements < maxAcc;
    };
}
