using Grpc.Core;
using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Immutable;

namespace Limekuma.Services;

public sealed partial class ListService : ListApi.ListApiBase
{
    private static (ImmutableArray<Record>, int, bool) BuildListRecords(IReadOnlySet<string> tags,
        string? condition, ParallelQuery<Record> records, ParallelQuery<Chart> charts)
    {
        (Func<Record, bool> predicate, Func<Chart, bool> counter, bool maskMutex) = ScoreFilterHelper.GetPredicateByTags(tags, condition);
        bool mayMask = ServiceExecutionHelper.HasMaskedScores(records);
        ServiceExecutionHelper.EnsurePermission(!(mayMask && maskMutex), "Mask enabled");

        ParallelQuery<Record> filteredRecords = records.Where(predicate).SortRecordForList();
        int totalCount = charts.Where(counter).Count();
        return ([.. filteredRecords], totalCount, mayMask);
    }

    private static async Task<(ImmutableArray<int>, int, int)> PrepareDataAsync(User user,
        ImmutableArray<Record> records, int page)
    {
        int i = (page - 1) * 55;
        int count = records.Length;
        if (i >= count)
        {
            throw new RpcException(new(StatusCode.OutOfRange, "The page number is out of the range boundary."));
        }

        await ServiceHelper.PrepareUserDataAsync(user);

        int end = Math.Min(i + 55, count);
        await ServiceHelper.PrepareRecordDataAsync(records[i..end]);

        int[] counts = new int[15];
        foreach (Record record in records)
        {
            AccumulateRecordStats(record, counts);
        }

        return ([.. counts], i, end);
    }

    private static void AccumulateRecordStats(Record record, int[] counts)
    {
        if (record.Rank >= AchievementsRank.SSSPlus)
        {
            counts[0]++;
        }

        if (record.Rank >= AchievementsRank.SSS)
        {
            counts[1]++;
        }

        if (record.Rank >= AchievementsRank.SSPlus)
        {
            counts[2]++;
        }

        if (record.Rank >= AchievementsRank.SS)
        {
            counts[3]++;
        }

        if (record.Rank >= AchievementsRank.SPlus)
        {
            counts[4]++;
        }

        if (record.Rank >= AchievementsRank.S)
        {
            counts[5]++;
        }

        if (record.Rank >= AchievementsRank.A)
        {
            counts[6]++;
        }

        if (record.ComboFlag.HasFlag(ComboFlag.AllPerfectPlus))
        {
            counts[7]++;
        }

        if (record.ComboFlag.HasFlag(ComboFlag.AllPerfect))
        {
            counts[8]++;
        }

        if (record.ComboFlag.HasFlag(ComboFlag.FullComboPlus))
        {
            counts[9]++;
        }

        if (record.ComboFlag.HasFlag(ComboFlag.FullCombo))
        {
            counts[10]++;
        }

        if (record.SyncFlag.HasFlag(SyncFlag.FullSyncDXPlus))
        {
            counts[11]++;
        }

        if (record.SyncFlag.HasFlag(SyncFlag.FullSyncDX))
        {
            counts[12]++;
        }

        if (record.SyncFlag.HasFlag(SyncFlag.FullSyncPlus))
        {
            counts[13]++;
        }

        if (record.SyncFlag.HasFlag(SyncFlag.FullSync))
        {
            counts[14]++;
        }
    }
}
