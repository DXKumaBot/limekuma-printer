using Limekuma.Prober.Common;
using Limekuma.Utils;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("dx_score", true)]
public sealed class DxScoreScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records)
    {
        ParallelQuery<Record> projectedRecords = records.Select(record =>
        {
            decimal achievements = (decimal)record.DXScore / record.Chart.TotalDXScore * 101;
            (AchievementsRank rank, decimal coefficient, _) = ConstantMap.ResolveRankAndCoefficient(achievements);

            int rating = (int)(record.Chart.LevelValue * achievements * coefficient);
            return new Record
            {
                Achievements = achievements,
                DXRating = rating,
                Chart = record.Chart,
                ComboFlag = record.ComboFlag,
                DXScore = record.DXScore,
                DXScoreRank = record.DXScoreRank,
                Rank = rank,
                SyncFlag = record.SyncFlag
            };
        });
        return projectedRecords.SortRecordForBests().SplitTopBestsByQuota(35, 15);
    }

    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p, ParallelQuery<Record> records2p) => throw new NotSupportedException();
}
