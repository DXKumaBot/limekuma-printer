using Limekuma.Prober.Common;

namespace Limekuma.Utils;

internal static class SortExtensions
{
    extension(ParallelQuery<Record> records)
    {
        internal OrderedParallelQuery<Record> SortRecordForBests() => records.OrderByDescending(x => x.DXRating)
            .ThenByDescending(x => x.Chart.LevelValue).ThenByDescending(x => x.Achievements);

        internal OrderedParallelQuery<Record> SortRecordForList() => records
            .OrderByDescending(x => x.Achievements)
            .ThenByDescending(x => x.DXRating).ThenByDescending(x => x.Chart.LevelValue);

        internal (ParallelQuery<Record> Ever, ParallelQuery<Record> Current) SplitTopBestsByQuota(
            int everQuota, int currentQuota)
        {
            ParallelQuery<Record> ever = records.Where(record => !record.Chart.Song.InCurrentGenre)
                .Take(everQuota);
            ParallelQuery<Record> current =
                records.Where(record => record.Chart.Song.InCurrentGenre).Take(currentQuota);
            return (ever, current);
        }
    }

    extension(IEnumerable<Record> records)
    {
        internal IOrderedEnumerable<Record> SortRecordForBests() => records.OrderByDescending(x => x.DXRating)
            .ThenByDescending(x => x.Chart.LevelValue).ThenByDescending(x => x.Achievements);

        internal IOrderedEnumerable<Record> SortRecordForList() => records.OrderByDescending(x => x.Achievements)
            .ThenByDescending(x => x.DXRating).ThenByDescending(x => x.Chart.LevelValue);
    }
}
