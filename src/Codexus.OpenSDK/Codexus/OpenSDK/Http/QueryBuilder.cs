using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Codexus.OpenSDK.Http;

public class QueryBuilder
{
	private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();

	private string? _baseUrl;

	public int Count => _parameters.Count;

	public QueryBuilder()
	{
	}

	public QueryBuilder(string urlOrQuery)
	{
		if (IsQueryStringOnly(urlOrQuery))
		{
			ParseQueryString(urlOrQuery.TrimStart('?'));
		}
		else
		{
			ParseUrl(urlOrQuery);
		}
	}

	public static QueryBuilder FromDictionary(Dictionary<string, string> parameters)
	{
		QueryBuilder queryBuilder = new QueryBuilder();
		foreach (KeyValuePair<string, string> parameter in parameters)
		{
			queryBuilder._parameters[parameter.Key] = parameter.Value;
		}
		return queryBuilder;
	}

	public static QueryBuilder FromObject(object obj)
	{
		QueryBuilder queryBuilder = new QueryBuilder();
		PropertyInfo[] properties = obj.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			object value = propertyInfo.GetValue(obj);
			if (value != null)
			{
				queryBuilder.Add(propertyInfo.Name, value.ToString());
			}
		}
		return queryBuilder;
	}

	public QueryBuilder Add(string key, string value)
	{
		_parameters[key] = value;
		return this;
	}

	public QueryBuilder Add<T>(string key, T value)
	{
		if (value != null)
		{
			_parameters[key] = value.ToString();
		}
		return this;
	}

	public QueryBuilder AddIf(bool condition, string key, string value)
	{
		if (condition)
		{
			Add(key, value);
		}
		return this;
	}

	public QueryBuilder AddIf(bool condition, string key, Func<string> valueFactory)
	{
		if (condition)
		{
			Add(key, valueFactory());
		}
		return this;
	}

	public QueryBuilder AddIfNotEmpty(string key, string? value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			Add(key, value);
		}
		return this;
	}

	public QueryBuilder AddRange(Dictionary<string, string> parameters)
	{
		foreach (KeyValuePair<string, string> parameter in parameters)
		{
			_parameters[parameter.Key] = parameter.Value;
		}
		return this;
	}

	public QueryBuilder Merge(QueryBuilder other)
	{
		return AddRange(other._parameters);
	}

	public QueryBuilder Remove(string key)
	{
		_parameters.Remove(key);
		return this;
	}

	public QueryBuilder RemoveRange(params string[] keys)
	{
		foreach (string key in keys)
		{
			_parameters.Remove(key);
		}
		return this;
	}

	public QueryBuilder Clear()
	{
		_parameters.Clear();
		return this;
	}

	public string Get(string key)
	{
		return _parameters.GetValueOrDefault(key) ?? throw new Exception("Parameter not found");
	}

	public string GetOrDefault(string key, string defaultValue)
	{
		return _parameters.GetValueOrDefault(key, defaultValue);
	}

	public bool Contains(string key)
	{
		return _parameters.ContainsKey(key);
	}

	public Dictionary<string, string> GetAll()
	{
		return new Dictionary<string, string>(_parameters);
	}

	public QueryBuilder WithBaseUrl(string url)
	{
		_baseUrl = url.Split('?')[0];
		return this;
	}

	public string BuildQueryString()
	{
		if (_parameters.Count == 0)
		{
			return string.Empty;
		}
		IEnumerable<string> values = from p in _parameters
			where !string.IsNullOrEmpty(p.Value)
			select Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value);
		return string.Join("&", values);
	}

	public string BuildUrl()
	{
		if (string.IsNullOrEmpty(_baseUrl))
		{
			throw new InvalidOperationException("Base URL is not set. Call WithBaseUrl() first or use BuildQueryString().");
		}
		string text = BuildQueryString();
		if (string.IsNullOrEmpty(text))
		{
			return _baseUrl;
		}
		string text2 = (_baseUrl.Contains('?') ? "&" : "?");
		return _baseUrl + text2 + text;
	}

	public static string BuildUrl(string baseUrl, Dictionary<string, string> parameters)
	{
		return new QueryBuilder().WithBaseUrl(baseUrl).AddRange(parameters).BuildUrl();
	}

	private bool IsQueryStringOnly(string input)
	{
		if (!input.StartsWith('?'))
		{
			if (input.Contains('='))
			{
				return !input.Contains("://");
			}
			return false;
		}
		return true;
	}

	private void ParseUrl(string url)
	{
		if (!url.Contains('?'))
		{
			_baseUrl = url;
			return;
		}
		string[] array = url.Split('?', 2);
		_baseUrl = array[0];
		ParseQueryString(array[1]);
	}

	private void ParseQueryString(string queryString)
	{
		if (string.IsNullOrEmpty(queryString))
		{
			return;
		}
		string[] array = queryString.Split('&');
		foreach (string text in array)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] array2 = text.Split('=', 2);
				if (array2.Length == 2)
				{
					string key = Uri.UnescapeDataString(array2[0]);
					string value = Uri.UnescapeDataString(array2[1]);
					_parameters[key] = value;
				}
			}
		}
	}

	public QueryBuilder Clone()
	{
		QueryBuilder queryBuilder = new QueryBuilder
		{
			_baseUrl = _baseUrl
		};
		foreach (KeyValuePair<string, string> parameter in _parameters)
		{
			queryBuilder._parameters[parameter.Key] = parameter.Value;
		}
		return queryBuilder;
	}

	public override string ToString()
	{
		return BuildQueryString();
	}
}
