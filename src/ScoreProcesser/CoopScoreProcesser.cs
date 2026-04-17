using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("coop", false, true)]
public sealed class CoopScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records1p,
        ParallelQuery<CommonRecord> records2p)
    {
        ParallelQuery<CommonRecord> records = records1p.Select(x =>
        {
            x.ExtraInfo = 0;
            return x;
        }).Union(records2p.Select(x =>
        {
            x.ExtraInfo = 1;
            return x;
        }));
        records.DistinctBy(x => (x.Chart.Song.Id, x.Chart.Difficulty));
        return records.SortRecordForBests().SplitTopBestsByQuota(35, 15);
    }
}
