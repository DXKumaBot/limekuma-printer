using Grpc.Core;
using Limekuma.Prober.Lxns;
using Limekuma.Prober.Lxns.Enums;
using Limekuma.Prober.Lxns.Models;
using Limekuma.Render;
using Limekuma.Utils;
using SixLabors.ImageSharp;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using CommonAchievementsRankEnum = Limekuma.Prober.Common.AchievementsRank;
using CommonClassRankEnum = Limekuma.Prober.Common.ClassRank;
using CommonComboFlagEnum = Limekuma.Prober.Common.ComboFlag;
using CommonCourseRankEnum = Limekuma.Prober.Common.CourseRank;
using CommonPlayer = Limekuma.Prober.Common.User;
using CommonRecord = Limekuma.Prober.Common.Record;
using CommonSyncFlagEnum = Limekuma.Prober.Common.SyncFlag;
using CommonTrophyColorEnum = Limekuma.Prober.Common.TrophyColor;

namespace Limekuma.Services;

public partial class BestsService
{
    private static async Task<(CommonPlayer, ParallelQuery<CommonRecord>)> PrepareLxnsRecordsForProcessAsync(
        string devToken, string personalToken)
    {
        Task<Player> playerTask = LxnsGatewayService.GetPlayerByPersonalTokenAsync(devToken, personalToken);
        Task<List<Record>> recordsTask = LxnsGatewayService.GetRecordsAsync(personalToken);
        await Task.WhenAll(playerTask, recordsTask);

        Player player = await playerTask;
        List<Record> records = await recordsTask;
        return (player, records.AsParallel()
            .Where(x => x.Type is not ChartType.Utage && SongData.Shared.SongsById.ContainsKey(x.Id))
            .Select(x => (CommonRecord)x));
    }

    private static async Task<(CommonPlayer, ImmutableArray<CommonRecord>, ImmutableArray<CommonRecord>, int, int)>
        PrepareLxnsDataAsync(string devToken, uint? qq, string? personalToken)
    {
        Player player;
        if (qq.HasValue)
        {
            player = await LxnsGatewayService.GetPlayerByQqAsync(devToken, qq.Value);
        }
        else if (!string.IsNullOrEmpty(personalToken))
        {
            player = await LxnsGatewayService.GetPlayerByPersonalTokenAsync(devToken, personalToken);
        }
        else
        {
            throw new RpcException(new(StatusCode.InvalidArgument, "QQ or token is required."));
        }

        Bests bests = await LxnsGatewayService.GetBestsAsync(player);

        CommonPlayer user = player;

        ParallelQuery<CommonRecord>
            bestEver = bests.Ever.AsParallel().Select(x => (CommonRecord)x).SortRecordForBests();
        ParallelQuery<CommonRecord> bestCurrent =
            bests.Current.AsParallel().Select(x => (CommonRecord)x).SortRecordForBests();

        await ServiceHelper.PrepareUserDataAsync(user);
        await PrepareDataAsync(bestEver, bestCurrent);

        return (user, [.. bestEver], [.. bestCurrent], bests.EverDXRating, bests.CurrentDXRating);
    }

    private static async Task<(CommonPlayer, ImmutableArray<CommonRecord>, ImmutableArray<CommonRecord>, int, int)>
        PrepareRiRenLxnsDataAsync()
    {
        LxnsResourceClient resource = new();
        SongData songData = await resource.GetSongsAsync(includeNotes: true);
        ParallelQuery<CommonRecord> allRecords = songData.Songs.AsParallel().SelectMany(song => song.Charts.Standard
            .Concat(song.Charts.DX).Where(chart => chart.Notes is not null).Select(chart =>
                (CommonRecord)new Record
                {
                    Achievements = 101,
                    Difficulty = chart.Difficulty,
                    Id = song.Id,
                    DXScore = chart.Notes!.Total * 3,
                    DXScoreRank = 5,
                    Level = chart.Level,
                    Title = song.Title,
                    Type = chart.Type,
                    ComboFlag = CommonComboFlagEnum.AllPerfectPlus,
                    DXRating = (int)(chart.LevelValue * 22.512m),
                    Rank = CommonAchievementsRankEnum.SSSPlus,
                    SyncFlag = CommonSyncFlagEnum.FullSyncDXPlus
                }));
        (ParallelQuery<CommonRecord> bestEver, ParallelQuery<CommonRecord> bestCurrent) =
            allRecords.SortRecordForBests().SplitTopBestsByQuota(35, 15);
        int everTotal = bestEver.Sum(x => x.DXRating);
        int currentTotal = bestCurrent.Sum(x => x.DXRating);
        CommonPlayer user = new()
        {
            Name = "ＤＸＫｕｍａ",
            DXRating = everTotal + currentTotal,
            TrophyColor = CommonTrophyColorEnum.Rainbow,
            TrophyText = "でらっくま",
            CourseRank = CommonCourseRankEnum.HiddenKaiden,
            ClassRank = CommonClassRankEnum.Legend,
            IconId = 101,
            PlateId = 55103,
            FrameId = 109101
        };

        await ServiceHelper.PrepareUserDataAsync(user);
        await PrepareDataAsync(bestEver, bestCurrent);

        return (user, [.. bestEver], [.. bestCurrent], everTotal, currentTotal);
    }

    public override async Task GetFromLxns(LxnsBestsRequest request, IServerStreamWriter<ImageResponse> responseStream,
        ServerCallContext context)
    {
        FrozenSet<string> requestTags = request.Tags.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        CommonPlayer player;
        CommonPlayer? player2 = null;
        ImmutableArray<CommonRecord> bestEver;
        ImmutableArray<CommonRecord> bestCurrent;
        int everTotal;
        int currentTotal;
        if (ScoreProcesserHelper.GetProcesserByTags(requestTags) is not null)
        {
            (player, ParallelQuery<CommonRecord> records) =
                await PrepareLxnsRecordsForProcessAsync(request.DevToken, request.PersonalToken);
            await ServiceHelper.PrepareUserDataAsync(player);
            (bestEver, bestCurrent, everTotal, currentTotal, player2) = await ProcessBestsByTagsAsync(requestTags,
                request.Condition, records,
                async condition =>
                {
                    SecondExtraInfo extraInfo =
                        ServiceExecutionHelper.DeserializeOrThrow<SecondExtraInfo>(condition,
                            "Failed to deserialize extra info for processing bests");
                    if (extraInfo.Source is "lxns" && extraInfo.UserInfo.Value is LxnsExtraInfo lxnsInfo)
                    {
                        return await PrepareLxnsRecordsForProcessAsync(request.DevToken, lxnsInfo.PersonalToken);
                    }

                    if (extraInfo.Source is "diving-fish" && extraInfo.UserInfo.Value is DivingFishExtraInfo dfInfo)
                    {
                        return await PrepareDfRecordsForProcessAsync(dfInfo.Token!, dfInfo.QQ, dfInfo.Frame,
                            dfInfo.Plate, dfInfo.Icon);
                    }

                    throw new RpcException(new(StatusCode.InvalidArgument, "Invalid extra info for processing bests"));
                });
        }
        else if (requestTags.Contains("common"))
        {
            (player, bestEver, bestCurrent, everTotal, currentTotal) =
                await PrepareLxnsDataAsync(request.DevToken, request.Qq, request.PersonalToken);
        }
        else if (requestTags.Contains("riren"))
        {
            (player, bestEver, bestCurrent, everTotal, currentTotal) = await PrepareRiRenLxnsDataAsync();
        }
        else
        {
            throw new RpcException(new(StatusCode.InvalidArgument, "Invalid tags for lxns bests request"));
        }

        using Image bestsImage = await new Drawer().DrawBestsAsync(player, bestEver, bestCurrent, everTotal,
            currentTotal,
            request.Condition, "lxns", requestTags, player2);

        await responseStream.WriteToResponseAsync(bestsImage);
    }

    public record LxnsExtraInfo(
        [property: JsonPropertyName("dev_token")]
        string? DevToken,
        [property: JsonPropertyName("personal_token")]
        string PersonalToken);
}
