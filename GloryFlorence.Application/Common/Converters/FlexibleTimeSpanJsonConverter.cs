using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloryFlorence.Application.Common.Converters
{
    public class FlexibleTimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        private static readonly string[] Formats = new[]
        {
            "hh\\:mm\\:ss",
            "h\\:mm\\:ss",
            "hh\\:mm",
            "h\\:mm",
            "c",
            "g",
            "G"
        };

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    return TimeSpan.Zero;
                }

                if (TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out var ts))
                {
                    return ts;
                }

                if (TimeSpan.TryParseExact(str, Formats, CultureInfo.InvariantCulture, out ts))
                {
                    return ts;
                }

                if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                {
                    return dt.TimeOfDay;
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long val))
                {
                    return TimeSpan.FromMinutes(val);
                }
                if (reader.TryGetDouble(out double dVal))
                {
                    return TimeSpan.FromMinutes(dVal);
                }
            }

            return TimeSpan.Zero;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture));
        }
    }

    public class NullableFlexibleTimeSpanJsonConverter : JsonConverter<TimeSpan?>
    {
        private static readonly FlexibleTimeSpanJsonConverter NonNullableConverter = new FlexibleTimeSpanJsonConverter();

        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String && string.IsNullOrWhiteSpace(reader.GetString()))
            {
                return null;
            }

            return NonNullableConverter.Read(ref reader, typeof(TimeSpan), options);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                NonNullableConverter.Write(writer, value.Value, options);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
