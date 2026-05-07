using Fractions;
using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("sun", true)]
public sealed class SunScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition) => _ => true;

    public Func<Record, bool> GetFilter(string? condition) => x =>
    {
        Fraction sumScore = (x.Chart.Notes.Tap + x.Chart.Notes.Touch + (x.Chart.Notes.Hold * 2) +
                             (x.Chart.Notes.Slide * 3) + (x.Chart.Notes.Break * 5)) * 5;
        Fraction scoreBound = (Fraction.One - ((sumScore - Fraction.One) / sumScore)) * 100;
        Fraction breakBound = x.Chart.Notes.Break > 0 ? Fraction.One / x.Chart.Notes.Break / 2 : 101;
        Fraction minScore = scoreBound <= breakBound ? scoreBound : breakBound;
        Fraction maxAcc = ConstantMap.GetRatingMinAchievement(x.Rank + 1);
        Fraction minAcc = maxAcc - minScore;
        Fraction achievements = x.Achievements;
        return achievements >= minAcc && achievements < maxAcc;
    };
}
