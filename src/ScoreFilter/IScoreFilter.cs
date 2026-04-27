using Limekuma.Prober.Common;

namespace Limekuma.ScoreFilter;

public interface IScoreFilter
{
    Func<Chart, bool> GetCounter(string? condition);

    Func<Record, bool> GetFilter(string? condition);
}
