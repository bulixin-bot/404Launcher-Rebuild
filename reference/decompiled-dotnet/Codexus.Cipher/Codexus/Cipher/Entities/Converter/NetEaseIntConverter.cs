using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codexus.Cipher.Entities.Converter;

public class NetEaseIntConverter : JsonConverter<string>
{
	public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string text;
		if (reader.TokenType != JsonTokenType.Number)
		{
			text = reader.GetString();
			if (text == null)
			{
				return string.Empty;
			}
		}
		else
		{
			text = reader.GetInt32().ToString();
		}
		return text;
	}

	public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value);
	}
}
