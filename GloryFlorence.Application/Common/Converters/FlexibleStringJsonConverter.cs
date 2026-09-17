using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GloryFlorence.Application.Common.Converters
{
    public class FlexibleStringJsonConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long lVal)) return lVal.ToString();
                if (reader.TryGetDouble(out double dVal)) return dVal.ToString();
                return reader.GetDecimal().ToString();
            }

            if (reader.TokenType == JsonTokenType.True)
            {
                return "true";
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return "false";
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var items = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var str = reader.GetString();
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            items.Add(str);
                        }
                    }
                    else if (reader.TokenType == JsonTokenType.Number)
                    {
                        if (reader.TryGetInt64(out long lVal)) items.Add(lVal.ToString());
                        else if (reader.TryGetDouble(out double dVal)) items.Add(dVal.ToString());
                    }
                }
                return items.Count > 0 ? string.Join(", ", items) : string.Empty;
            }

            using var doc = JsonDocument.ParseValue(ref reader);
            return doc.RootElement.ToString();
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
