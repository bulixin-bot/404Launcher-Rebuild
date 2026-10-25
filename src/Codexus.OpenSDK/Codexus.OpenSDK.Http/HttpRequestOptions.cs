using System;
using System.Collections.Generic;

namespace Codexus.OpenSDK.Http;

public class HttpRequestOptions
{
	public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();

	public Dictionary<string, string> QueryParameters { get; } = new Dictionary<string, string>();

	public Version? HttpVersion { get; set; }

	public HttpRequestOptions AddHeader(string key, string value)
	{
		Headers[key] = value;
		return this;
	}

	public HttpRequestOptions AddHeaders(Dictionary<string, string> headers)
	{
		foreach (KeyValuePair<string, string> header in headers)
		{
			Headers[header.Key] = header.Value;
		}
		return this;
	}

	public HttpRequestOptions WithUserAgent(string userAgent)
	{
		return AddHeader("User-Agent", userAgent);
	}

	public HttpRequestOptions WithBearerToken(string token)
	{
		return AddHeader("Authorization", "Bearer " + token);
	}

	public HttpRequestOptions AddQuery(string key, string value)
	{
		QueryParameters[key] = value;
		return this;
	}

	public HttpRequestOptions AddQueries(Dictionary<string, string> queries)
	{
		foreach (KeyValuePair<string, string> query in queries)
		{
			QueryParameters[query.Key] = query.Value;
		}
		return this;
	}

	public HttpRequestOptions WithVersion(Version version)
	{
		HttpVersion = version;
		return this;
	}

	internal HttpRequestOptions Clone()
	{
		HttpRequestOptions httpRequestOptions = new HttpRequestOptions
		{
			HttpVersion = HttpVersion
		};
		foreach (KeyValuePair<string, string> header in Headers)
		{
			httpRequestOptions.Headers[header.Key] = header.Value;
		}
		foreach (KeyValuePair<string, string> queryParameter in QueryParameters)
		{
			httpRequestOptions.QueryParameters[queryParameter.Key] = queryParameter.Value;
		}
		return httpRequestOptions;
	}
}
