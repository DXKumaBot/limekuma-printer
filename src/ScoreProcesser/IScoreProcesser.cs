using Grpc.Core;
using Limekuma.Prober.Common;

namespace Limekuma.ScoreProcesser;

public interface IScoreProcesser
{
    (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records) =>
        throw new RpcException(new(StatusCode.InvalidArgument,
            "Single score data is not supported by this processer."));

    (ParallelQuery<CommonRecord>, ParallelQuery<CommonRecord>) Process(ParallelQuery<CommonRecord> records1p,
        ParallelQuery<CommonRecord> records2p) => Process(records1p);
}
