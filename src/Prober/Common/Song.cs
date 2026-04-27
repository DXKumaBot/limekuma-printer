namespace Limekuma.Prober.Common;

public class Song
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public required ChartType Type { get; init; }

    public required string VersionTitle { get; init; }

    public required bool InCurrentVersion { get; init; }

    public required string AudioUrl { get; init; }

    public required string JacketUrl { get; init; }
}
