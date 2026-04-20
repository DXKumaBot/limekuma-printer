using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("genre")]
public sealed class GenreScoreFilter : IScoreFilter
{
    public Func<Record, bool> GetFilter(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return _ => true;
        }
        GenreExtraInfo? extraInfo = JsonSerializer.Deserialize<GenreExtraInfo>(condition);
        if (extraInfo is null)
        {
            return _ => true;
        }

        if (!ConstantMap.GenreMap.TryGetValue(extraInfo.Genre, out FrozenSet<string>? genre))
        {
            return _ => true;
        }

        if (extraInfo.Difficulty is null)
        {
            return x => genre.Contains(x.Chart.Song.Genre);
        }

        return x => genre.Contains(x.Chart.Song.Genre) && x.Chart.Difficulty == extraInfo.Difficulty;
    }

    public record GenreExtraInfo
    {
        [JsonPropertyName("genre")]
        public required string Genre { get; init; }

        [JsonPropertyName("diff")]
        public required Difficulty? Difficulty { get; init; }
    }
}
