using System.Text.Json;

namespace SastImg.Client.Helpers
{
    public class Int32Converter : System.Text.Json.Serialization.JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString() ?? "0";
                    if (int.TryParse(stringValue, out int value))
                    {
                        return value;
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    return reader.GetInt32();
                }
            }
            catch (JsonException)
            {
                // 忽略异常
            }

            return 0; // 或者返回一个默认值
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
    public class Int64Converter : System.Text.Json.Serialization.JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString() ?? "0";
                    if (long.TryParse(stringValue, out long value))
                    {
                        return value;
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    return reader.GetInt64();
                }
            }
            catch (JsonException)
            {
                // 忽略异常
            }

            return 0L; // 或者返回一个默认值
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
