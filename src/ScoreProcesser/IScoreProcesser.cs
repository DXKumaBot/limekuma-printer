using Grpc.Core;
using Limekuma.Prober.Common;

namespace Limekuma.ScoreProcesser;

public interface IScoreProcesser
{
    (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records) =>
        throw new RpcException(new(StatusCode.InvalidArgument,
            "Single score data is not supported by this processer."));

    (ParallelQuery<Record>, ParallelQuery<Record>) Process(ParallelQuery<Record> records1p,
        ParallelQuery<Record> records2p) => Process(records1p);
}
