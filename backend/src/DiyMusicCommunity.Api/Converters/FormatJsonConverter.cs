using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiyMusicCommunity.Domain.Enums;

namespace DiyMusicCommunity.Api.Converters;

/// <summary>
/// Serialises <see cref="Format"/> values using their <see cref="DescriptionAttribute"/> text
/// (e.g. "12\" Vinyl" instead of "Vinyl12"). Deserialises both the description string and the
/// raw enum name so existing stored values remain compatible.
/// </summary>
public sealed class FormatJsonConverter : JsonConverter<Format>
{
    private static readonly Dictionary<Format, string> ToDescription = BuildToDescription();
    private static readonly Dictionary<string, Format> FromString = BuildFromString();

    public override Format Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is not null && FromString.TryGetValue(value, out var format))
        {
            return format;
        }

        throw new JsonException($"Unknown Format value: '{value}'.");
    }

    public override void Write(Utf8JsonWriter writer, Format value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToDescription.TryGetValue(value, out var description)
            ? description
            : value.ToString());
    }

    private static Dictionary<Format, string> BuildToDescription()
    {
        return Enum.GetValues<Format>()
            .ToDictionary(
                f => f,
                f => typeof(Format)
                         .GetField(f.ToString())
                         ?.GetCustomAttribute<DescriptionAttribute>()
                         ?.Description ?? f.ToString());
    }

    private static Dictionary<string, Format> BuildFromString()
    {
        var map = new Dictionary<string, Format>(StringComparer.OrdinalIgnoreCase);

        foreach (var f in Enum.GetValues<Format>())
        {
            // Accept both the description ("12\" Vinyl") and the raw name ("Vinyl12")
            var description = typeof(Format)
                .GetField(f.ToString())
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;

            map[f.ToString()] = f;

            if (description is not null && !map.ContainsKey(description))
            {
                map[description] = f;
            }
        }

        return map;
    }
}
