using Grpc.Core;
using Limekuma.Prober.DivingFish.Enums;
using Limekuma.Prober.DivingFish.Models;
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
    private static async Task<(CommonPlayer, ParallelQuery<CommonRecord>)> PrepareDfRecordsForProcessAsync(string token,
        uint? qq, int? frame, int? plate, int? icon)
    {
        ServiceExecutionHelper.EnsureArgument(qq.HasValue && frame.HasValue && plate.HasValue && icon.HasValue);
        PlayerData player = await DfGatewayService.GetPlayerDataAsync(token, qq!.Value);

        CommonPlayer user = player;
        user.FrameId = frame!.Value;
        user.PlateId = plate!.Value;
        user.IconId = icon!.Value;
        return (user, player.Records.AsParallel().Where(x =>
                x.Difficulty is not Difficulty.Utage && Songs.SharedSongs.SongsById.ContainsKey(x.Id.ToString()))
            .Select(x => (CommonRecord)x));
    }

    private static async Task<(CommonPlayer, ImmutableArray<CommonRecord>, ImmutableArray<CommonRecord>, int, int)>
        PrepareDfDataAsync(uint? qq, int? frame, int? plate, int? icon)
    {
        ServiceExecutionHelper.EnsureArgument(qq.HasValue && frame.HasValue && plate.HasValue && icon.HasValue);
        Player player = await DfGatewayService.GetPlayerAsync(qq!.Value);

        CommonPlayer user = player;
        user.FrameId = frame!.Value;
        user.PlateId = plate!.Value;
        user.IconId = icon!.Value;

        ParallelQuery<CommonRecord> bestEver =
            player.Bests.Ever.AsParallel().Select(x => (CommonRecord)x).SortRecordForBests();
        int everTotal = bestEver.Sum(x => x.DXRating);

        ParallelQuery<CommonRecord> bestCurrent =
            player.Bests.Current.AsParallel().Select(x => (CommonRecord)x).SortRecordForBests();
        int currentTotal = bestCurrent.Sum(x => x.DXRating);

        await ServiceHelper.PrepareUserDataAsync(user);
        await PrepareDataAsync(bestEver, bestCurrent);

        return (user, [.. bestEver], [.. bestCurrent], everTotal, currentTotal);
    }

    private static async Task<(CommonPlayer, ImmutableArray<CommonRecord>, ImmutableArray<CommonRecord>, int, int)>
        PrepareRiRenDfDataAsync()
    {
        ParallelQuery<CommonRecord> allRecords = Songs.Shared.AsParallel().SelectMany(song =>
        {
            if (!int.TryParse(song.Id, out int id))
            {
                return [];
            }

            int chartCount = Math.Min(song.Charts.Count, Math.Min(song.LevelValues.Count, song.Levels.Count));
            return Enumerable.Range(0, chartCount).AsParallel().Select(i => (CommonRecord)new Record
            {
                Achievements = 101,
                ComboFlag = CommonComboFlagEnum.AllPerfectPlus,
                Difficulty = (Difficulty)(i + 1),
                DifficultyIndex = i,
                DXRating = (int)(song.LevelValues[i] * 22.512m),
                DXScore = song.Charts[i].Notes.Total * 3,
                Id = id,
                Level = song.Levels[i],
                LevelValue = song.LevelValues[i],
                Rank = CommonAchievementsRankEnum.SSSPlus,
                SyncFlag = CommonSyncFlagEnum.FullSyncDXPlus,
                Title = song.Title,
                Type = song.Type
            });
        });
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
            CourseRank = CommonCourseRankEnum.Urakaiden,
            ClassRank = CommonClassRankEnum.Legend,
            IconId = 101,
            PlateId = 55103,
            FrameId = 109101
        };

        await ServiceHelper.PrepareUserDataAsync(user);
        await PrepareDataAsync(bestEver, bestCurrent);

        return (user, [.. bestEver], [.. bestCurrent], everTotal, currentTotal);
    }

    public override async Task GetFromDivingFish(DivingFishBestsRequest request,
        IServerStreamWriter<ImageResponse> responseStream, ServerCallContext context)
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
            (player, ParallelQuery<CommonRecord> records) = await PrepareDfRecordsForProcessAsync(request.Token,
                request.Qq, request.Frame, request.Plate, request.Icon);
            await ServiceHelper.PrepareUserDataAsync(player);
            (bestEver, bestCurrent, everTotal, currentTotal, player2) = await ProcessBestsByTagsAsync(requestTags,
                request.Condition, records, async condition =>
                {
                    SecondExtraInfo extraInfo =
                        ServiceExecutionHelper.DeserializeOrThrow<SecondExtraInfo>(condition,
                            "Failed to deserialize extra info for processing bests");
                    if (extraInfo.Source is "lxns" && extraInfo.UserInfo.Value is LxnsExtraInfo lxnsInfo)
                    {
                        return await PrepareLxnsRecordsForProcessAsync(lxnsInfo.DevToken!, lxnsInfo.PersonalToken);
                    }

                    if (extraInfo.Source is "diving_fish" && extraInfo.UserInfo.Value is DivingFishExtraInfo dfInfo)
                    {
                        return await PrepareDfRecordsForProcessAsync(request.Token, dfInfo.QQ, dfInfo.Frame,
                            dfInfo.Plate, dfInfo.Icon);
                    }

                    throw new RpcException(new(StatusCode.InvalidArgument, "Invalid extra info for processing bests"));
                });
        }
        else if (requestTags.Contains("common"))
        {
            (player, bestEver, bestCurrent, everTotal, currentTotal) =
                await PrepareDfDataAsync(request.Qq, request.Frame, request.Plate, request.Icon);
        }
        else if (requestTags.Contains("riren"))
        {
            (player, bestEver, bestCurrent, everTotal, currentTotal) = await PrepareRiRenDfDataAsync();
        }
        else
        {
            throw new RpcException(new(StatusCode.InvalidArgument, "Invalid tags for diving fish bests request"));
        }

        using Image bestsImage = await new Drawer().DrawBestsAsync(player, bestEver, bestCurrent, everTotal,
            currentTotal,
            request.Condition, "divingfish", requestTags, player2);

        await responseStream.WriteToResponseAsync(bestsImage);
    }

    public record DivingFishExtraInfo(
        [property: JsonPropertyName("token")]
        string? Token,
        [property: JsonPropertyName("qq")]
        uint QQ,
        [property: JsonPropertyName("frame")]
        int Frame,
        [property: JsonPropertyName("plate")]
        int Plate,
        [property: JsonPropertyName("icon")]
        int Icon);
}
