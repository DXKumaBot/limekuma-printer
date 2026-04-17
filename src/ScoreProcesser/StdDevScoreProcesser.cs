using Limekuma.Prober.Common;
using Limekuma.Prober.DivingFish.Models;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("std_dev")]
public sealed class StdDevScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records)
    {
        ParallelQuery<CommonRecord> rankedRecords = records.Select(record =>
            {
                decimal stdDev = 0;
                decimal fitLevel = record.Chart.LevelValue;
                if (Status.Shared.TryGetChartState(record.Chart.Song.Id, (int)record.Chart.Difficulty - 1,
                        out ChartState? chartState))
                {
                    stdDev = chartState.StandardDeviation;
                    fitLevel = chartState.FitLevel;
                }

                record.ExtraInfo = stdDev;
                decimal score = record.DXRating * (1 + (stdDev / 10)) * (1 + (fitLevel / (fitLevel > 0 ? 10 : 1)));
                return (Record: record, Score: score);
            }).OrderByDescending(x => x.Score).ThenByDescending(x => x.Record.Chart.LevelValue)
            .ThenByDescending(x => x.Record.Achievements).Select(x => x.Record);
        return rankedRecords.SplitTopBestsByQuota(35, 15);
    }
}
