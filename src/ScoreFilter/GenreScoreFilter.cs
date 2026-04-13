using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Frozen;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("genre")]
public sealed class GenreScoreFilter : IScoreFilter
{
    public Func<CommonRecord, bool> GetFilter(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return _ => true;
        }

        if (!ConstantMap.GenreMap.TryGetValue(condition, out FrozenSet<string>? genre))
        {
            return _ => true;
        }

        return x => genre.Contains(x.Chart.Song.Genre);
    }
}
