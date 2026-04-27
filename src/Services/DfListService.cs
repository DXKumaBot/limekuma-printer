using Grpc.Core;
using Limekuma.Prober.DivingFish.Enums;
using Limekuma.Prober.DivingFish.Models;
using Limekuma.Render;
using Limekuma.Utils;
using SixLabors.ImageSharp;
using System.Collections.Frozen;
using System.Collections.Immutable;
using CommonChart = Limekuma.Prober.Common.Chart;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonRecord = Limekuma.Prober.Common.Record;

namespace Limekuma.Services;

public partial class ListService
{
    public override async Task GetFromDivingFish(DivingFishListRequest request,
        IServerStreamWriter<ImageResponse> responseStream, ServerCallContext context)
    {
        FrozenSet<string> requestTags = request.Tags.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        PlayerData player = await DfGatewayService.GetPlayerDataAsync(request.Token, request.Qq);

        CommonPlayer user = player;
        user.PlateId = request.Plate;
        user.IconId = request.Icon;

        (ImmutableArray<CommonRecord> records, int totalCount, bool mayMask) = BuildListRecords(requestTags, request.Condition,
            player.Records.AsParallel().Where(x =>
                    x.Difficulty is not Difficulty.Utage && Songs.SharedSongs.SongsById.ContainsKey(x.Id))
                .Select(x => (CommonRecord)x), Songs.SharedSongs.Charts.AsParallel());
        user.MayMasked = mayMask;

        (ImmutableArray<int> counts, int startIndex, int endIndex) =
            await PrepareDataAsync(user, records, request.Page);

        using Image listImage = await new Drawer().DrawListAsync(user, records[startIndex..endIndex], request.Page,
            counts, totalCount, startIndex, request.Condition, request.Tags);

        await responseStream.WriteToResponseAsync(listImage);
    }
}
