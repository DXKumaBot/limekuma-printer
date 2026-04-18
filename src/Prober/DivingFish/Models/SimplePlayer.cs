using System.Text.Json.Serialization;

namespace Limekuma.Prober.DivingFish.Models;

public class SimplePlayer
{
    [JsonPropertyName("ra")]
    public required int DXRating { get; set; }

    [JsonPropertyName("username")]
    public required string AccountName { get; set; }
}
