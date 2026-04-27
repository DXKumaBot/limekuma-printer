using Grpc.Core;
using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Limekuma.Services;

public sealed partial class BestsService : BestsApi.BestsApiBase
{
    private static async Task PrepareDataAsync(IEnumerable<Record> bestsEver,
        IEnumerable<Record> bestsCurrent) => await Task.WhenAll(
        ServiceHelper.PrepareRecordDataAsync(bestsEver),
        ServiceHelper.PrepareRecordDataAsync(bestsCurrent));

    private static async
        Task<(ImmutableArray<Record> BestEver, ImmutableArray<Record> BestCurrent, int EverTotal, int
            CurrentTotal, bool, User?)> ProcessBestsByTagsAsync(IReadOnlySet<string> tags, string condition,
            ParallelQuery<Record> records,
            Func<string, Task<(User, ParallelQuery<Record>)>> secondDataProvider)
    {
        if (ScoreProcesserHelper.GetProcesserByTags(tags) is not { } selectedProcesser)
        {
            throw new RpcException(new(StatusCode.InvalidArgument, "Invalid arguments"));
        }

        User? player2 = null;
        bool mayMask = ServiceExecutionHelper.HasMaskedScores(records);
        ServiceExecutionHelper.EnsurePermission(!(mayMask && selectedProcesser.MaskMutex), "Mask enabled");

        ParallelQuery<Record> bestEver;
        ParallelQuery<Record> bestCurrent;
        if (selectedProcesser.RequireSecondData)
        {
            (player2, ParallelQuery<Record> records2p) = await secondDataProvider(condition);
            bool mayMask2p = ServiceExecutionHelper.HasMaskedScores(records2p);
            ServiceExecutionHelper.EnsurePermission(!(mayMask2p && selectedProcesser.MaskMutex), "Mask enabled");
            player2.MayMasked = mayMask2p;
            (bestEver, bestCurrent) = selectedProcesser.Processer.Process(records, records2p);
        }
        else
        {
            (Func<Record, bool> predicate, _, bool maskMutex) =
                ScoreFilterHelper.GetPredicateByTags(tags, condition);
            ServiceExecutionHelper.EnsurePermission(!(mayMask && maskMutex), "Mask enabled");

            ParallelQuery<Record> filteredRecords = records.Where(predicate).SortRecordForBests();
            (bestEver, bestCurrent) = selectedProcesser.Processer.Process(filteredRecords);
        }

        int everTotal = bestEver.Sum(x => x.DXRating);
        int currentTotal = bestCurrent.Sum(x => x.DXRating);

        await PrepareDataAsync(bestEver, bestCurrent);

        return ([.. bestEver], [.. bestCurrent], everTotal, currentTotal, mayMask, player2);
    }

    public record SecondExtraInfo(
        [property: JsonPropertyName("source")]
        ProberType Source,
        [property: JsonPropertyName("user_info")]
        Union<LxnsExtraInfo, DivingFishExtraInfo> UserInfo);
}
