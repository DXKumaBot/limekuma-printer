using Grpc.Core;
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
            (_, _, decimal oldCoefficient) = ConstantMap.ResolveRankAndCoefficient(record.Achievements);
            int rating = (int)(record.Chart.LevelValue * (record.Achievements > 100.5m ? 100.5m : record.Achievements) *
                               oldCoefficient);
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
}
