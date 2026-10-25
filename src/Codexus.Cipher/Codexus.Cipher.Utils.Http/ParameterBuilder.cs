using System;
using System.Collections.Generic;
using System.Linq;

namespace Codexus.Cipher.Utils.Http;

public class ParameterBuilder
{
	private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

	public string? Url { get; set; }

	public ParameterBuilder()
	{
	}

	public ParameterBuilder(string parameter)
	{
		if (parameter.Contains('?'))
		{
			Url = parameter.Substring(0, parameter.IndexOf('?'));
			string text = parameter;
			int num = parameter.IndexOf('?') + 1;
			parameter = text.Substring(num, text.Length - num);
		}
		string[] array = parameter.Split('&');
		for (int num = 0; num < array.Length; num++)
		{
			string[] array2 = array[num].Split('=');
			if (array2.Length == 2)
			{
				_parameters.Add(array2[0], array2[1]);
			}
		}
	}

	public string Get(string parameter)
	{
		if (!_parameters.TryGetValue(parameter, out string value))
		{
			return string.Empty;
		}
		return value;
	}

	public ParameterBuilder Append(string key, string value)
	{
		_parameters[key] = value;
		return this;
	}

	public ParameterBuilder Remove(string key)
	{
		_parameters.Remove(key);
		return this;
	}

	public string FormUrlEncode()
	{
		IEnumerable<string> values = _parameters.Select((KeyValuePair<string, string> p) => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value));
		return string.Join("&", values);
	}

	public string ToQueryUrl()
	{
		return Url + "?" + FormUrlEncode();
	}

	public override string ToString()
	{
		return _parameters.Aggregate(string.Empty, (string current, KeyValuePair<string, string> kv) => ((current == string.Empty) ? current : (current + "&")) + kv.Key + "=" + kv.Value);
	}
}
