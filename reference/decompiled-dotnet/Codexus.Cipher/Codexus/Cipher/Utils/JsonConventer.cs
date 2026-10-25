using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codexus.Cipher.Utils;

public static class JsonConventer
{
	public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
	{
		public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions? options)
		{
			List<T> list = new List<T>();
			if (reader.TokenType != JsonTokenType.StartArray)
			{
				T item = JsonSerializer.Deserialize<T>(ref reader, options);
				list.Add(item);
				return list;
			}
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				T item2 = JsonSerializer.Deserialize<T>(ref reader, options);
				list.Add(item2);
			}
			return list;
		}

		public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (T item in value)
			{
				JsonSerializer.Serialize(writer, item, options);
			}
			writer.WriteEndArray();
		}
	}

	public class StringFromNumberOrStringConverter : JsonConverter<string>
	{
		public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return reader.TokenType switch
			{
				JsonTokenType.Number => reader.GetInt64().ToString(), 
				JsonTokenType.String => reader.GetString(), 
				_ => throw new JsonException("Unsupported token type for string conversion."), 
			};
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value);
		}
	}
}
