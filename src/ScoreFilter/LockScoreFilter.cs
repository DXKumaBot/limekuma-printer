using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("lock", true)]
public sealed class LockScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition) => _ => true;

    public Func<Record, bool> GetFilter(string? condition) => x =>
    {
        decimal sumScore = (x.Chart.Notes.Tap + x.Chart.Notes.Touch + (x.Chart.Notes.Hold * 2) +
                            (x.Chart.Notes.Slide * 3) + (x.Chart.Notes.Break * 5)) * 5;
        decimal minScore = Math.Min((1 - ((sumScore - 1) / sumScore)) * 100,
            x.Chart.Notes.Break > 0 ? 1m / x.Chart.Notes.Break / 2 : 101);
        decimal minAcc = ConstantMap.GetRatingMinAchievement(x.Rank);
        decimal maxAcc = minAcc + minScore;
        return x.Achievements >= minAcc && x.Achievements < maxAcc;
    };
}
