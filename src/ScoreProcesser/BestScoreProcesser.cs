using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Immutable;

namespace Limekuma.ScoreProcesser;

[ScoreProcesserTag("best")]
public sealed class BestScoreProcesser : IScoreProcesser
{
    public (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records)
    {
        (ImmutableArray<Record>.Builder Ever, ImmutableArray<Record>.Builder Current) state = (
            ImmutableArray.CreateBuilder<Record>(35), ImmutableArray.CreateBuilder<Record>(15));
        (ImmutableArray<Record>.Builder Ever, ImmutableArray<Record>.Builder Current) rankedState = records
            .SortRecordForBests().Aggregate(state, static (acc, record) =>
            {
                if (acc.Ever.Count >= 35 && acc.Current.Count >= 15)
                {
                    return acc;
                }

                (record.Chart.Song.InCurrentGenre switch
                {
                    true => acc.Current.Count < 15 ? acc.Current : acc.Ever,
                    false => acc.Ever.Count < 35 ? acc.Ever : acc.Current
                }).Add(record);
                return acc;
            });
        ImmutableArray<Record>.Builder ever = rankedState.Ever;
        ImmutableArray<Record>.Builder current = rankedState.Current;
        return (ever.ToImmutable().AsParallel(), current.ToImmutable().AsParallel());
    }
}
