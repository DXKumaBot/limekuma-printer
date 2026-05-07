using Fractions;
using Limekuma.Prober.DivingFish.Models;
using Limekuma.Utils;
using CommonRecord = Limekuma.Prober.Common.Record;
using DFStatus = Limekuma.Prober.DivingFish.Models.Status;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("fit_level", true)]
public sealed class FitLevelScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records)
    {
        ParallelQuery<CommonRecord> projectedRecords = records.Select(record =>
        {
            Fraction fitLevel = record.Chart.LevelValue;
            if (DFStatus.Shared.TryGetChartState(record.Chart.Song.Id, (int)record.Chart.Difficulty,
                    out ChartState chartState))
            {
                fitLevel = chartState.FitLevel;
            }

            (_, Fraction coefficient, _) = ConstantMap.ResolveRankAndCoefficient(record.Achievements);
            Fraction cap = new(201, 2);
            Fraction achievements = record.Achievements <= cap ? record.Achievements : cap;
            Fraction ratingValue = fitLevel * achievements * coefficient;
            int rating = ratingValue.ToInt32Truncated();
            Fraction level = fitLevel.Truncate(2);
            return new CommonRecord
            {
                Achievements = record.Achievements,
                DXRating = rating,
                Chart = new()
                {
                    LevelValue = level,
                    Difficulty = record.Chart.Difficulty,
                    Level = record.Chart.Level,
                    Notes = record.Chart.Notes,
                    Song = record.Chart.Song,
                    TotalDXScore = record.Chart.TotalDXScore
                },
                ComboFlag = record.ComboFlag,
                DXScore = record.DXScore,
                DXScoreRank = record.DXScoreRank,
                Rank = record.Rank,
                SyncFlag = record.SyncFlag,
                ExtraInfo = record.Chart.LevelValue
            };
        });
        return projectedRecords.SortRecordForBests().SplitTopBestsByQuota(35, 15);
    }

    public (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records1p, ParallelQuery<CommonRecord> records2p) => throw new NotSupportedException();
}
