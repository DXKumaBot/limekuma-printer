using Limekuma.Prober.Common;
using System.Text.Json;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("dx_star")]
public sealed class DXScoreRankScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition) => _ => true;

    public Func<Record, bool> GetFilter(string? condition)
    {
        if (condition is null)
        {
            return _ => true;
        }

        List<int>? dxScoreRanks = JsonSerializer.Deserialize<List<int>>(condition);
        if (dxScoreRanks is null)
        {
            return _ => true;
        }

        return x => dxScoreRanks.Contains(x.DXScoreRank);
    }
}
