using Limekuma.Render.ExpressionEngine;
using Limekuma.Render.Types;
using Fractions;
using SixLabors.ImageSharp;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Limekuma.Render;

public sealed partial class TemplateReader
{
    private async Task<string> EvaluateRequiredTemplateAttributeAsync(XElement element, string name, object? scope) =>
        await EvaluateTemplateAsync(GetRequiredAttributeValue(element, name), scope);

    private async Task<string> EvaluateTemplateAttributeOrAsync(XElement element, string name, string defaultValue,
        object? scope)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return defaultValue;
        }

        return await EvaluateTemplateAsync(attribute.Value, scope);
    }

    private async Task<string?> EvaluateOptionalTemplateAttributeAsync(XElement element, string name, object? scope)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return null;
        }

        return await EvaluateTemplateAsync(attribute.Value, scope);
    }

    private async Task<T> EvaluateRequiredExpressionAttributeAsync<T>(XElement element, string name, object? scope)
    {
        string raw = GetRequiredAttributeValue(element, name);
        T value = await EvaluateExpressionAsAsync<T>(raw, scope) ?? throw new InvalidOperationException(
            $"DSL[AttributeNull] Required attribute evaluated to null. Context: element='{element.Name.LocalName}', attribute='{name}', expression='{raw}'");
        return value;
    }

    private async Task<T> EvaluateExpressionAttributeOrAsync<T>(XElement element, string name, T defaultValue,
        object? scope)
    {
        XAttribute? attribute = element.Attribute(name);
        if (attribute is null)
        {
            return defaultValue;
        }

        if (defaultValue is null && string.IsNullOrEmpty(attribute.Value))
        {
            return defaultValue;
        }

        T? value = await EvaluateExpressionAsAsync<T>(attribute.Value, scope);
        return value ?? defaultValue;
    }

    private async Task<T> EvaluateRequiredExpressionAsAsync<T>(string raw, object? scope)
    {
        T value = await EvaluateExpressionAsAsync<T>(raw, scope) ??
                  throw new InvalidOperationException(
                      $"DSL[ExpressionEval] Can not evaluate expression. Context: expression='{raw}'");
        return value;
    }

    private async Task<Color> ParseColorAttributeOrAsync(XElement element, string name, string defaultRaw,
        object? scope)
    {
        string raw = GetAttributeValueOrDefault(element, name, defaultRaw);
        string colorText = await EvaluateTemplateAsync(raw, scope);
        return Color.Parse(colorText);
    }

    private async Task<Color?> ParseColorAttributeAsync(XElement element, string name, object? scope)
    {
        string? raw = element.Attribute(name)?.Value;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        string colorText = await EvaluateTemplateAsync(raw, scope);
        return Color.Parse(colorText);
    }

    private async Task<ResamplerType> ParseResamplerTypeAsync(XElement element, object? scope) =>
        await EvaluateExpressionAttributeOrAsync(element, "resampler", ResamplerType.Lanczos3, scope);

    private async Task<string> EvaluateTemplateAsync(string? template, object? scope)
    {
        if (template is null)
        {
            return string.Empty;
        }

        List<string> expressionTexts = [];
        StringBuilder safeTemplateBuilder = new(template.Length);
        int templateOffset = 0;
        IEnumerable<Match> matches = ExprSegmentRegex().Matches(template);
        templateOffset = matches.Select((match, tokenIndex) => (match, tokenIndex)).Aggregate(templateOffset,
            (offset, item) =>
            {
                Match match = item.match;
                safeTemplateBuilder.Append(template, offset, match.Index - offset);
                safeTemplateBuilder.Append("__EXPR_TOKEN_");
                safeTemplateBuilder.Append(item.tokenIndex);
                safeTemplateBuilder.Append("__");
                expressionTexts.Add(match.Groups[1].Value);
                return match.Index + match.Length;
            });

        safeTemplateBuilder.Append(template, templateOffset, template.Length - templateOffset);
        string safeTemplate = safeTemplateBuilder.ToString();

        IDictionary<string, object?> values = ScopeFlattener.Flatten(scope);
        Dictionary<string, object?> normalizedValues = NormalizeTemplateValues(values);
        string formatted = Formatter.Format(safeTemplate, normalizedValues);
        if (expressionTexts.Count is 0)
        {
            return formatted;
        }

        StringBuilder output = new(formatted.Length);
        int formattedOffset = 0;

        foreach (Match match in ExprTokenRegex().Matches(formatted))
        {
            output.Append(formatted, formattedOffset, match.Index - formattedOffset);
            if (!int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture,
                    out int exprIndex) || exprIndex < 0 || exprIndex >= expressionTexts.Count)
            {
                output.Append(match.Value);
                formattedOffset = match.Index + match.Length;
                continue;
            }

            object? value = await expressionEngine.EvalAsync(expressionTexts[exprIndex], scope);
            output.Append(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
            formattedOffset = match.Index + match.Length;
        }

        output.Append(formatted, formattedOffset, formatted.Length - formattedOffset);
        return output.ToString();
    }

    private async Task<T?> EvaluateExpressionAsAsync<T>(string raw, object? scope)
    {
        Type targetType = typeof(T);
        Type nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        bool isEnumTarget = nonNullableType.IsEnum;

        string templateValue = await EvaluateTemplateAsync(raw, scope);
        if (TryConvert(templateValue, nonNullableType, out object? convertedTemplateValue))
        {
            return (T?)convertedTemplateValue;
        }

        if (isEnumTarget && !string.IsNullOrWhiteSpace(templateValue))
        {
            throw BuildInvalidEnumException(nonNullableType, templateValue, raw);
        }

        object? expressionValue = await expressionEngine.EvalAsync(raw, scope);
        if (expressionValue is null)
        {
            return default;
        }

        if (targetType.IsInstanceOfType(expressionValue))
        {
            return (T)expressionValue;
        }

        if (TryConvert(expressionValue, nonNullableType, out object? convertedExpressionValue))
        {
            return (T?)convertedExpressionValue;
        }

        if (isEnumTarget)
        {
            throw BuildInvalidEnumException(nonNullableType, expressionValue, raw);
        }

        return (T?)Convert.ChangeType(expressionValue, nonNullableType, CultureInfo.InvariantCulture);
    }

    private static InvalidOperationException
        BuildInvalidEnumException(Type enumType, object? rawValue, string expression) => new(
        $"DSL[Enum] Invalid enum value. Context: enum='{enumType.Name}', value='{Convert.ToString(rawValue, CultureInfo.InvariantCulture)}', expression='{expression}', allowed='{string.Join(", ", Enum.GetNames(enumType))}'");

    private static bool TryConvert(object? value, Type targetType, out object? converted)
    {
        if (value is null)
        {
            converted = null;
            return false;
        }

        if (targetType == typeof(string))
        {
            converted = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
            return true;
        }

        if (targetType == typeof(int) && int.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
        {
            converted = intValue;
            return true;
        }

        if (targetType == typeof(decimal) && decimal.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue))
        {
            converted = decimalValue;
            return true;
        }

        if (targetType == typeof(float) && float.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
        {
            converted = floatValue;
            return true;
        }

        if (targetType == typeof(double) && double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue))
        {
            converted = doubleValue;
            return true;
        }

        if (targetType == typeof(bool) &&
            bool.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), out bool boolValue))
        {
            converted = boolValue;
            return true;
        }

        if (targetType.IsEnum && Enum.TryParse(targetType, Convert.ToString(value, CultureInfo.InvariantCulture), true,
                out object? enumValue))
        {
            converted = enumValue;
            return true;
        }

        converted = null;
        return false;
    }

    private static Dictionary<string, object?> NormalizeTemplateValues(IDictionary<string, object?> values)
    {
        Dictionary<string, object?> normalized = new(values.Count, StringComparer.OrdinalIgnoreCase);
        Dictionary<object, object?> objectCache = new(ReferenceEqualityComparer.Instance);
        foreach ((string key, object? value) in values)
        {
            normalized[key] = NormalizeTemplateValue(value, key, 0, objectCache);
        }

        return normalized;
    }

    private static object? NormalizeTemplateValue(object? value, string? key, int depth,
        IDictionary<object, object?> objectCache)
    {
        if (value is null)
        {
            return null;
        }

        if (value is Fraction fraction)
        {
            return TryConvertFractionToDecimal(fraction, out decimal decimalValue)
                ? decimalValue
                : (decimal)fraction.Numerator / (decimal)fraction.Denominator;
        }

        if (depth > 6 || IsLeafType(value.GetType()))
        {
            return value;
        }

        bool shouldExpandObject = string.IsNullOrEmpty(key) || !key.Contains('.', StringComparison.Ordinal);
        if (!shouldExpandObject)
        {
            return value;
        }

        if (objectCache.TryGetValue(value, out object? cachedValue))
        {
            return cachedValue;
        }

        if (value is IDictionary dictionary)
        {
            Dictionary<string, object?> normalizedDictionary = new(StringComparer.OrdinalIgnoreCase);
            objectCache[value] = normalizedDictionary;
            foreach (DictionaryEntry entry in dictionary)
            {
                string entryKey = Convert.ToString(entry.Key, CultureInfo.InvariantCulture) ?? string.Empty;
                normalizedDictionary[entryKey] =
                    NormalizeTemplateValue(entry.Value, string.Empty, depth + 1, objectCache);
            }

            return normalizedDictionary;
        }

        if (value is IEnumerable enumerable and not string)
        {
            List<object?> normalizedList = [];
            objectCache[value] = normalizedList;
            foreach (object? item in enumerable)
            {
                normalizedList.Add(NormalizeTemplateValue(item, string.Empty, depth + 1, objectCache));
            }

            return normalizedList;
        }

        Dictionary<string, object?> normalizedObject = new(StringComparer.OrdinalIgnoreCase);
        objectCache[value] = normalizedObject;
        PropertyInfo[] properties = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (PropertyInfo property in properties)
        {
            if (!property.CanRead || property.GetIndexParameters().Length is not 0)
            {
                continue;
            }

            object? propertyValue = property.GetValue(value);
            normalizedObject[property.Name] =
                NormalizeTemplateValue(propertyValue, string.Empty, depth + 1, objectCache);
        }

        return normalizedObject;
    }

    private static bool IsLeafType(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) ||
               type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) ||
               type == typeof(Guid);
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
    }

    private static bool TryConvertFractionToDecimal(Fraction value, out decimal decimalValue)
    {
        try
        {
            decimalValue = value.ToDecimal();
            return true;
        }
        catch (OverflowException)
        {
            decimalValue = default;
            return false;
        }
    }

    [GeneratedRegex("__EXPR_TOKEN_(\\d+)__")]
    private static partial Regex ExprTokenRegex();
}
