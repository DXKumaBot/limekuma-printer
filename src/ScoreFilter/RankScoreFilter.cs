using Limekuma.Prober.Common;
using System.Text.Json;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("rank")]
public sealed class RankScoreFilter : IScoreFilter
{
    public Func<Record, bool> GetFilter(string? condition)
    {
        if (condition is null)
        {
            return _ => true;
        }

        List<AchievementsRank>? ranks = JsonSerializer.Deserialize<List<AchievementsRank>>(condition);
        if (ranks is null)
        {
            return _ => true;
        }

        return x => ranks.Contains(x.Rank);
    }
}
