using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("steal", false, true)]
public sealed class StealScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p,
        ParallelQuery<Record> records2p)
    {
        (ParallelQuery<Record> iBaseEver, ParallelQuery<Record> iBaseCurrent) =
            records1p.SortRecordForBests().SplitTopBestsByQuota(35, 15);
        ImmutableArray<Record> baseEver = [.. iBaseEver];
        ImmutableArray<Record> baseCurrent = [.. iBaseCurrent];
        int everMin = baseEver.IsDefaultOrEmpty ? 0 : baseEver[^1].DXRating;
        int currentMin = baseCurrent.IsDefaultOrEmpty ? 0 : baseCurrent[^1].DXRating;

        bool handleType = records1p.Count() > records2p.Count();
        ParallelQuery<Record> processRecords = handleType ? records2p : records1p;
        ParallelQuery<Record> controlRecords = handleType ? records1p : records2p;
        FrozenDictionary<(int SongId, Difficulty Difficulty), Record> controlLookup =
            controlRecords.ToFrozenDictionary(x => (x.Chart.Song.Id, x.Chart.Difficulty));
        ParallelQuery<Record> selectedRecords = processRecords.Select(processRecord =>
        {
            if (!controlLookup.TryGetValue((processRecord.Chart.Song.Id, processRecord.Chart.Difficulty),
                    out Record? controlRecord))
            {
                return null;
            }

            Record record = handleType ? processRecord : controlRecord;
            Record anotherRecord = handleType ? controlRecord : processRecord;
            Record selected = new()
            {
                Achievements = record.Achievements,
                Chart = record.Chart,
                ComboFlag = record.ComboFlag,
                DXRating = record.DXRating,
                DXScore = record.DXScore,
                DXScoreRank = record.DXScoreRank,
                Rank = record.Rank,
                SyncFlag = record.SyncFlag,
                ExtraInfo = anotherRecord.DXRating
            };
            return selected;
        }).OfType<Record>();

        ParallelQuery<Record> current = selectedRecords.Where(x => x.Chart.Song.InCurrentVersion)
            .OrderByDescending(x => x.DXRating > currentMin)
            .ThenByDescending(x => x.DXRating - x.ExtraInfo).ThenByDescending(x => x.Chart.LevelValue)
            .ThenByDescending(x => x.Achievements).Take(15);
        ParallelQuery<Record> ever = selectedRecords.Where(x => !x.Chart.Song.InCurrentVersion)
            .OrderByDescending(x => x.DXRating > everMin)
            .ThenByDescending(x => x.DXRating - x.ExtraInfo).ThenByDescending(x => x.Chart.LevelValue)
            .ThenByDescending(x => x.Achievements).Take(35);

        return (ever, current);
    }

    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records) => throw new NotSupportedException();
}
