using System.Text.Json.Serialization;
using CommonTitleColorEnum = Limekuma.Prober.Common.TitleColor;

namespace Limekuma.Prober.Lxns.Models;

public record Collection
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("required")]
    public List<CollectionRequired>? Required { get; init; }
}

public record Title : Collection
{
    [JsonPropertyName("color")]
    public required CommonTitleColorEnum Color { get; init; }
}

public record Icon : Collection
{
    public string Url => field ??= $"https://assets2.lxns.net/maimai/icon/{Id}.png";

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }
}

public record NamePlate : Collection
{
    public string Url => field ??= $"https://assets2.lxns.net/maimai/plate/{Id}.png";

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }
}

public record Frame : Collection
{
    public string Url => field ??= $"https://assets2.lxns.net/maimai/frame/{Id}.png";

    [JsonPropertyName("genre")]
    public required string Genre { get; init; }
}
