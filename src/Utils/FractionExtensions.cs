using Fractions;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Limekuma.Utils;

internal static class FractionExtensions
{
    extension(Fraction value)
    {
        public Fraction Truncate(int decimalPlaces)
        {
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces), decimalPlaces,
                    "Decimal places must be non-negative.");
            }

            BigInteger scale = BigInteger.Pow(10, decimalPlaces);
            BigInteger truncated = (value.Numerator * scale) / value.Denominator;
            return new Fraction(truncated, scale);
        }

        public int ToInt32Truncated()
        {
            return (int)(value.Numerator / value.Denominator);
        }
    }
}

public sealed class FractionJsonConverter : JsonConverter<Fraction>
{
    public override Fraction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => ReadFromNumber(ref reader),
            JsonTokenType.String => ReadFromString(ref reader),
            JsonTokenType.StartObject => ReadFromObject(ref reader),
            _ => throw new JsonException($"Unexpected token for fraction: {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, Fraction value, JsonSerializerOptions options)
    {
        if (CanRepresentAsDecimal(value, out decimal decimalValue))
        {
            writer.WriteNumberValue(decimalValue);
            return;
        }

        writer.WriteStringValue(value.ToString());
    }

    private static Fraction ReadFromNumber(ref Utf8JsonReader reader)
    {
        if (reader.TryGetDecimal(out decimal decimalValue))
        {
            return Fraction.FromDecimal(decimalValue);
        }

        if (reader.TryGetInt64(out long intValue))
        {
            return new Fraction(intValue, 1);
        }

        throw new JsonException("Numeric value is out of range for fraction.");
    }

    private static Fraction ReadFromString(ref Utf8JsonReader reader)
    {
        string? text = reader.GetString();
        if (!string.IsNullOrWhiteSpace(text) &&
            Fraction.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out Fraction value))
        {
            return value;
        }

        throw new JsonException($"Invalid fraction string '{text}'.");
    }

    private static Fraction ReadFromObject(ref Utf8JsonReader reader)
    {
        BigInteger? numerator = null;
        BigInteger? denominator = null;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Invalid fraction object structure.");
            }

            string? name = reader.GetString();
            if (!reader.Read())
            {
                throw new JsonException("Unexpected end while reading fraction object.");
            }

            if (name is "numerator")
            {
                numerator = ReadBigInteger(ref reader);
            }
            else if (name is "denominator")
            {
                denominator = ReadBigInteger(ref reader);
            }
            else
            {
                reader.Skip();
            }
        }

        if (!numerator.HasValue || !denominator.HasValue)
        {
            throw new JsonException("Fraction object must contain numerator and denominator.");
        }

        return new Fraction(numerator.Value, denominator.Value);
    }

    private static BigInteger ReadBigInteger(ref Utf8JsonReader reader)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? text = reader.GetString();
            if (!BigInteger.TryParse(text, out BigInteger value))
            {
                throw new JsonException($"Invalid integer string '{text}'.");
            }

            return value;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out long longValue))
            {
                return new BigInteger(longValue);
            }

            return new BigInteger(reader.GetDecimal());
        }

        throw new JsonException("Invalid token type for integer.");
    }

    private static bool CanRepresentAsDecimal(Fraction value, out decimal decimalValue)
    {
        decimalValue = 0;
        try
        {
            decimalValue = value.ToDecimal();
            return true;
        }
        catch (OverflowException)
        {
            return false;
        }
    }
}
