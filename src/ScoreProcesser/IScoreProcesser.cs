using Limekuma.Prober.Common;

namespace Limekuma.ScoreProcesser;

public interface IScoreProcesser
{
    (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records);

    (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p, ParallelQuery<Record> records2p);
}
