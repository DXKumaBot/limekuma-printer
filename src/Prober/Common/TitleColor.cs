using System.Text.Json.Serialization;

namespace Limekuma.Prober.Common;

[JsonConverter(typeof(JsonStringEnumConverter<TitleColor>))]
public enum TitleColor
{
    Normal,
    Bronze,
    Silver,
    Gold,
    Rainbow
}
