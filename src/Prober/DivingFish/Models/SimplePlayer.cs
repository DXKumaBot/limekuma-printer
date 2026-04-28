using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public class SimplePlayer
{
    [JsonPropertyName("ra")]
    public required int DXRating { get; init; }

    [JsonPropertyName("username")]
    public required string AccountName { get; init; }
}
