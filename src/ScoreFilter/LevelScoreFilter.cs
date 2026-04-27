using Limekuma.Prober.Common;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("level")]
public sealed class LevelScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition) => x => x.Level == condition;

    public Func<Record, bool> GetFilter(string? condition) => x => x.Chart.Level == condition;
}
