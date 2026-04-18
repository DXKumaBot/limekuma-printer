using Limekuma.Prober.Common;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("combo")]
public sealed class ComboScoreFilter : IScoreFilter
{
    public Func<Record, bool> GetFilter(string? condition)
    {
        if (!Enum.TryParse(condition, out ComboFlag comboFlag))
        {
            return _ => true;
        }

        return x => x.ComboFlag.HasFlag(comboFlag);
    }
}
