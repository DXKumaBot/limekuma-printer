using Limekuma.Prober.Common;
using System.Collections.Immutable;

namespace Limekuma.Utils;

internal static class SortExtensions
{
    extension(IEnumerable<CommonRecord> records)
    {
        internal IOrderedEnumerable<CommonRecord> SortRecordForBests() => records.OrderByDescending(x => x.DXRating)
            .ThenByDescending(x => x.Chart.LevelValue).ThenByDescending(x => x.Achievements);

        internal IOrderedEnumerable<CommonRecord> SortRecordForList() => records.OrderByDescending(x => x.Achievements)
            .ThenByDescending(x => x.DXRating).ThenByDescending(x => x.Chart.LevelValue);

        internal (ImmutableArray<CommonRecord> Ever, ImmutableArray<CommonRecord> Current) SplitTopBestsByQuota(
            int everQuota, int currentQuota)
        {
            ImmutableArray<CommonRecord> ever =
                [.. records.Where(record => !record.Chart.Song.InCurrentGenre).Take(everQuota)];
            ImmutableArray<CommonRecord> current =
                [.. records.Where(record => record.Chart.Song.InCurrentGenre).Take(currentQuota)];
            return (ever, current);
        }
    }
}
