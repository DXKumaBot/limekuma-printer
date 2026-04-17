using Limekuma.Prober.Common;

namespace Limekuma.Utils;

internal static class SortExtensions
{
    extension(ParallelQuery<CommonRecord> records)
    {
        internal OrderedParallelQuery<CommonRecord> SortRecordForBests() => records.OrderByDescending(x => x.DXRating)
            .ThenByDescending(x => x.Chart.LevelValue).ThenByDescending(x => x.Achievements);

        internal OrderedParallelQuery<CommonRecord> SortRecordForList() => records.OrderByDescending(x => x.Achievements)
            .ThenByDescending(x => x.DXRating).ThenByDescending(x => x.Chart.LevelValue);

        internal (ParallelQuery<CommonRecord> Ever, ParallelQuery<CommonRecord> Current) SplitTopBestsByQuota(
            int everQuota, int currentQuota)
        {
            ParallelQuery<CommonRecord> ever = records.Where(record => !record.Chart.Song.InCurrentGenre).Take(everQuota);
            ParallelQuery<CommonRecord> current = records.Where(record => record.Chart.Song.InCurrentGenre).Take(currentQuota);
            return (ever, current);
        }
    }

    extension(IEnumerable<CommonRecord> records)
    {
        internal IOrderedEnumerable<CommonRecord> SortRecordForBests() => records.OrderByDescending(x => x.DXRating)
            .ThenByDescending(x => x.Chart.LevelValue).ThenByDescending(x => x.Achievements);

        internal IOrderedEnumerable<CommonRecord> SortRecordForList() => records.OrderByDescending(x => x.Achievements)
            .ThenByDescending(x => x.DXRating).ThenByDescending(x => x.Chart.LevelValue);
    }
}
