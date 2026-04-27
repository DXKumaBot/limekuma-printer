using System.Collections.Frozen;
using System.Collections.Immutable;
using CommonChart = Limekuma.Prober.Common.Chart;

namespace Limekuma.Prober.DivingFish.Models;

internal class Songs(IEnumerable<Song> songs)
{
    private Lazy<ImmutableArray<CommonChart>>? _charts;
    private readonly DateTimeOffset _pullTime = DateTimeOffset.Now;
    private readonly ImmutableArray<Song> _songs = [.. songs];

    public FrozenDictionary<int, Song> SongsById => field ??= Shared.AsParallel().ToFrozenDictionary(x => x.Id);

    public ImmutableArray<CommonChart> Charts => (_charts ??= new(() => Shared.AsParallel().SelectMany(x => x.CommonCharts).ToImmutableArray())).Value;

    public static Songs SharedSongs
    {
        get
        {
            if (field is not null && DateTimeOffset.Now.AddHours(10).Date == field._pullTime.AddHours(10).Date)
            {
                return field;
            }

            DfResourceClient resource = new();
            field = (Songs)resource.GetSongsAsync().Result;

            return field;
        }
    }

    public static ImmutableArray<Song> Shared => SharedSongs;

    public static explicit operator Songs(List<Song> songs) => new(songs);

    public static implicit operator ImmutableArray<Song>(Songs a) => a._songs;
}
