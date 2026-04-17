using Grpc.Core;
using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Limekuma.Services;

public sealed partial class BestsService : BestsApi.BestsApiBase
{
    private static async Task PrepareDataAsync(IEnumerable<CommonRecord> bestsEver,
        IEnumerable<CommonRecord> bestsCurrent) => await Task.WhenAll(
        ServiceHelper.PrepareRecordDataAsync(bestsEver),
        ServiceHelper.PrepareRecordDataAsync(bestsCurrent));

    private static async
        Task<(ImmutableArray<CommonRecord> BestEver, ImmutableArray<CommonRecord> BestCurrent, int EverTotal, int
            CurrentTotal, CommonUser?)> ProcessBestsByTagsAsync(IReadOnlySet<string> tags, string condition,
            ParallelQuery<CommonRecord> records,
            Func<string, Task<(CommonUser, ParallelQuery<CommonRecord>)>> secondDataProvider)
    {
        if (ScoreProcesserHelper.GetProcesserByTags(tags) is not { } selectedProcesser)
        {
            throw new RpcException(new(StatusCode.InvalidArgument, "Invalid arguments"));
        }

        CommonUser? user2p = null;
        bool mayMask = ServiceExecutionHelper.HasMaskedScores(records);
        ServiceExecutionHelper.EnsurePermission(!(mayMask && selectedProcesser.MaskMutex), "Mask enabled");

        ParallelQuery<CommonRecord> bestEver;
        ParallelQuery<CommonRecord> bestCurrent;
        if (selectedProcesser.RequireSecondData)
        {
            (user2p, ParallelQuery<CommonRecord> records2p) = await secondDataProvider(condition);
            (bestEver, bestCurrent) = selectedProcesser.Processer.Process(records, records2p);
        }
        else
        {
            (Func<CommonRecord, bool> predicate, bool maskMutex) =
                ScoreFilterHelper.GetPredicateByTags(tags, condition);
            ServiceExecutionHelper.EnsurePermission(!(mayMask && maskMutex), "Mask enabled");

            ParallelQuery<CommonRecord> filteredRecords = records.Where(predicate).SortRecordForBests();
            (bestEver, bestCurrent) = selectedProcesser.Processer.Process(filteredRecords);
        }

        int everTotal = bestEver.Sum(x => x.DXRating);
        int currentTotal = bestCurrent.Sum(x => x.DXRating);

        await PrepareDataAsync(bestEver, bestCurrent);

        return ([.. bestEver], [.. bestCurrent], everTotal, currentTotal, user2p);
    }

    public record SecondExtraInfo(
        [property: JsonPropertyName("source")]
        string Source,
        [property: JsonPropertyName("user_info")]
        Union<LxnsExtraInfo, DivingFishExtraInfo> UserInfo);
}
