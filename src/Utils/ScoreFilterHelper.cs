using Limekuma.Prober.Common;
using Limekuma.ScoreFilter;
using System.Collections.Frozen;
using System.Reflection;

namespace Limekuma.Utils;

internal static class ScoreFilterHelper
{
    private static readonly FrozenDictionary<string, (IScoreFilter, bool)> Filters = BuildFilters();

    internal static (Func<Record, bool>, Func<Chart, bool>, bool) GetPredicateByTags(IReadOnlySet<string> tags, string? condition)
    {
        if (tags is null)
        {
            return (_ => true, _ => true, false);
        }

        ParallelQuery<(IScoreFilter, bool)> selectedFilters =
            tags.AsParallel().Select(Filters.GetValueOrDefault).Where(x => x.Item1 is not null);
        ParallelQuery<Func<Record, bool>> predicates = selectedFilters.Select(x => x.Item1.GetFilter(condition));
        ParallelQuery<Func<Chart, bool>> counters = selectedFilters.Select(x => x.Item1.GetCounter(condition));
        bool maskMutex = selectedFilters.Any(x => x.Item2);

        return (record => predicates.All(predicate => predicate(record)), chart => counters.All(counter => counter(chart)), maskMutex);
    }

    private static FrozenDictionary<string, (IScoreFilter, bool)> BuildFilters() => typeof(IScoreFilter).Assembly
        .GetTypes().Where(type =>
            type is { IsInterface: false, IsAbstract: false } && typeof(IScoreFilter).IsAssignableFrom(type))
        .Select(type => new
        {
            Type = type,
            Attribute = type.GetCustomAttribute<ScoreFilterTagAttribute>()
        }).Where(x => !string.IsNullOrWhiteSpace(x.Attribute?.Tag)).Select(x => new
        {
            x.Attribute!.Tag,
            Filter = Activator.CreateInstance(x.Type) as IScoreFilter,
            x.Attribute.MaskMutex
        }).Where(x => x.Filter is not null).ToFrozenDictionary(x => x.Tag!,
            x => (filter: x.Filter!, maskMutex: x.MaskMutex), StringComparer.OrdinalIgnoreCase);
}
