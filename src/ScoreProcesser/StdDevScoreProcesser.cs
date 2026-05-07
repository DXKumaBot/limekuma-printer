using Fractions;
using Limekuma.Prober.DivingFish.Models;
using Limekuma.Utils;
using CommonRecord = Limekuma.Prober.Common.Record;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("std_dev")]
public sealed class StdDevScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records)
    {
        ParallelQuery<CommonRecord> rankedRecords = records.Select(record =>
            {
                Fraction stdDev = Fraction.Zero;
                Fraction fitLevel = record.Chart.LevelValue;
                if (Status.Shared.TryGetChartState(record.Chart.Song.Id, (int)record.Chart.Difficulty,
                        out ChartState chartState))
                {
                    stdDev = chartState.StandardDeviation;
                    fitLevel = chartState.FitLevel;
                }

                record.ExtraInfo = stdDev;
                Fraction score = record.DXRating
                                 * (Fraction.One + (stdDev / 10))
                                 * (Fraction.One + (fitLevel / (fitLevel > 0 ? 10 : 1)));
                return (Record: record, Score: score);
            }).OrderByDescending(x => x.Score).ThenByDescending(x => x.Record.Chart.LevelValue)
            .ThenByDescending(x => x.Record.Achievements).Select(x => x.Record);
        return rankedRecords.SplitTopBestsByQuota(35, 15);
    }

    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records1p, ParallelQuery<CommonRecord> records2p) => throw new NotSupportedException();
}
