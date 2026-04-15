using Grpc.Core;
using Limekuma.Prober.Common;
using Limekuma.Prober.DivingFish.Models;
using Limekuma.Utils;
using System.Collections.Immutable;
using Status = Limekuma.Prober.DivingFish.Models.Status;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("fit_level", true)]
public sealed class FitLevelScoreProcesser : IScoreProcesser
{
    public (ImmutableArray<CommonRecord>, ImmutableArray<CommonRecord>) Process(IReadOnlyList<CommonRecord> records)
    {
        if (records.Any(r => r.DXScore is 0 && (r.DXScoreRank > 0 || r.Rank > Ranks.A)))
        {
            throw new RpcException(new(StatusCode.PermissionDenied, "Mask enabled"));
        }

        ParallelQuery<CommonRecord> projectedRecords = records.AsParallel().Select(record =>
        {
            decimal fitLevel = record.Chart.LevelValue;
            if (Status.Shared.TryGetChartState(record.Chart.Song.Id, (int)record.Chart.Difficulty - 1,
                    out ChartState chartState))
            {
                fitLevel = chartState.FitLevel;
            }

            (_, decimal coefficient, _) = ConstantMap.ResolveRankAndCoefficient(record.Achievements);
            int rating = (int)(fitLevel * (record.Achievements > 100.5m ? 100.5m : record.Achievements) * coefficient);
            decimal level = (int)(fitLevel * 100) / 100m;
            return new CommonRecord()
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
}
