using Grpc.Core;
using Limekuma.Prober.Lxns.Enums;
using Limekuma.Prober.Lxns.Models;
using Limekuma.Render;
using Limekuma.Utils;
using SixLabors.ImageSharp;
using System.Collections.Frozen;
using System.Collections.Immutable;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonRecord = Limekuma.Prober.Common.Record;

namespace Limekuma.Services;

public partial class ListService
{
    public override async Task GetFromLxns(LxnsListRequest request, IServerStreamWriter<ImageResponse> responseStream,
        ServerCallContext context)
    {
        FrozenSet<string> requestTags = request.Tags.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        Task<Player> playerTask =
            LxnsGatewayService.GetPlayerByPersonalTokenAsync(request.DevToken, request.PersonalToken);
        Task<List<Record>> sourceRecordsTask =
            LxnsGatewayService.GetRecordsAsync(request.PersonalToken);
        await Task.WhenAll(playerTask, sourceRecordsTask);

        CommonPlayer player = await playerTask;
        ParallelQuery<CommonRecord> sourceRecords = (await sourceRecordsTask).AsParallel()
            .Where(x => x.Type is not ChartType.Utage && SongData.Shared.SongsById.ContainsKey(x.Id))
            .Select(x => (CommonRecord)x);
        (ImmutableArray<CommonRecord> cRecords, int totalCount, bool mayMask) =
            BuildListRecords(requestTags, request.Condition, sourceRecords, SongData.Shared.Charts.AsParallel());
        player.MayMasked = mayMask;

        (ImmutableArray<int> counts, int startIndex, int endIndex) =
            await PrepareDataAsync(player, cRecords, request.Page);

        using Image listImage = await new Drawer().DrawListAsync(player, cRecords[startIndex..endIndex], request.Page,
            counts, totalCount, startIndex, request.Condition, request.Tags);

        await responseStream.WriteToResponseAsync(listImage);
    }
}
