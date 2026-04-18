using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("filter")]
public sealed class FilterScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records) =>
        records.SortRecordForBests().SplitTopBestsByQuota(35, 15);
}
