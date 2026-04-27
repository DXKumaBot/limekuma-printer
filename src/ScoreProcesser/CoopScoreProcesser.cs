using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("coop", false, true)]
public sealed class CoopScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p,
        ParallelQuery<Record> records2p)
    {
        ParallelQuery<Record> records = records1p.Select(x =>
        {
            x.ExtraInfo = 0;
            return x;
        }).Union(records2p.Select(x =>
        {
            x.ExtraInfo = 1;
            return x;
        })).GroupBy(x => (x.Chart.Song.Id, x.Chart.Difficulty)).Select(x => x.SortRecordForBests().First());

        return records.SortRecordForBests().SplitTopBestsByQuota(35, 15);
    }

    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records) => throw new NotSupportedException();
}
