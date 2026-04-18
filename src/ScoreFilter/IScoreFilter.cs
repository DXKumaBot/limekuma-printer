using Limekuma.Prober.Common;

namespace Limekuma.ScoreFilter;

public interface IScoreFilter
{
    Func<Record, bool> GetFilter(string? condition);
}
