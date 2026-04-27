using Limekuma.Prober.Common;
using Limekuma.Utils;
using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Limekuma.ScoreFilter;

[ScoreFilterTag("version")]
public sealed class VersionScoreFilter : IScoreFilter
{
    public Func<Chart, bool> GetCounter(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return _ => true;
        }

        VersionExtraInfo? extraInfo = JsonSerializer.Deserialize<VersionExtraInfo>(condition);
        if (extraInfo is null)
        {
            return _ => true;
        }

        if (!ConstantMap.VersionMap.TryGetValue(extraInfo.Version, out FrozenSet<string>? Version))
        {
            return _ => true;
        }

        if (extraInfo.Difficulty is null)
        {
            return x => Version.Contains(x.Song.VersionTitle);
        }

        return x => Version.Contains(x.Song.VersionTitle) && x.Difficulty == extraInfo.Difficulty;
    }

    public Func<Record, bool> GetFilter(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return _ => true;
        }

        VersionExtraInfo? extraInfo = JsonSerializer.Deserialize<VersionExtraInfo>(condition);
        if (extraInfo is null)
        {
            return _ => true;
        }

        if (!ConstantMap.VersionMap.TryGetValue(extraInfo.Version, out FrozenSet<string>? version))
        {
            return _ => true;
        }

        if (extraInfo.Difficulty is null)
        {
            return x => version.Contains(x.Chart.Song.VersionTitle);
        }

        return x => version.Contains(x.Chart.Song.VersionTitle) && x.Chart.Difficulty == extraInfo.Difficulty;
    }

    public record VersionExtraInfo
    {
        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("diff")]
        public required Difficulty? Difficulty { get; init; }
    }
}
