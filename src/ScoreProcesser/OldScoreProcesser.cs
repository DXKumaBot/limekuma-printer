using Fractions;
using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("old", true)]
public sealed class OldScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records)
    {
        ParallelQuery<Record> projectedRecords = records.Select(record =>
        {
            (_, _, Fraction oldCoefficient) = ConstantMap.ResolveRankAndCoefficient(record.Achievements);
            Fraction cap = new(201, 2);
            Fraction achievements = record.Achievements <= cap ? record.Achievements : cap;
            Fraction ratingValue = record.Chart.LevelValue * achievements * oldCoefficient;
            int rating = ratingValue.ToInt32Truncated();
            return new Record
            {
                Achievements = record.Achievements,
                DXRating = rating,
                Chart = record.Chart,
                ComboFlag = record.ComboFlag,
                DXScore = record.DXScore,
                DXScoreRank = record.DXScoreRank,
                Rank = record.Rank,
                SyncFlag = record.SyncFlag
            };
        });
        return projectedRecords.SortRecordForBests().SplitTopBestsByQuota(25, 15);
    }

    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p, ParallelQuery<Record> records2p) => throw new NotSupportedException();
}
