using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Codexus.OpenSDK.Http;

public class HttpWrapper : IDisposable
{
	private readonly string _baseUrl;

	private readonly HttpRequestOptions _defaultOptions;

	public HttpClient Client { get; }

	public HttpWrapper(string baseUrl = "", Action<HttpRequestOptions>? configureDefaults = null, HttpClientHandler? handler = null)
	{
		_baseUrl = baseUrl.TrimEnd('/');
		Client = new HttpClient(handler ?? new HttpClientHandler
		{
			AutomaticDecompression = DecompressionMethods.All
		})
		{
			Timeout = TimeSpan.FromSeconds(30L)
		};
		_defaultOptions = new HttpRequestOptions();
		configureDefaults?.Invoke(_defaultOptions);
		ApplyDefaultOptions();
	}

	public void Dispose()
	{
		Client.Dispose();
		GC.SuppressFinalize(this);
	}

	public async Task<string> GetStringAsync(string url, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpResponseMessage httpResponseMessage = await GetAsync(url, configure, cancellationToken);
		httpResponseMessage.EnsureSuccessStatusCode();
		return await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
	}

	public async Task<T?> GetAsync<T>(string url, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return JsonSerializer.Deserialize<T>(await GetStringAsync(url, configure, cancellationToken));
	}

	public async Task<HttpResponseMessage> GetAsync(string url, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpRequestMessage request = CreateRequest(HttpMethod.Get, url, configure);
		return await Client.SendAsync(request, cancellationToken);
	}

	public async Task<HttpResponseMessage> PostJsonAsync<T>(string url, T data, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		string content = JsonSerializer.Serialize(data);
		return await PostAsync(url, content, "application/json", configure, cancellationToken);
	}

	public async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest data, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpResponseMessage httpResponseMessage = await PostJsonAsync(url, data, configure, cancellationToken);
		httpResponseMessage.EnsureSuccessStatusCode();
		return JsonSerializer.Deserialize<TResponse>(await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken));
	}

	public async Task<HttpResponseMessage> PostFormAsync(string url, Dictionary<string, string> formData, Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpRequestMessage httpRequestMessage = CreateRequest(HttpMethod.Post, url, configure);
		httpRequestMessage.Content = new FormUrlEncodedContent(formData);
		return await Client.SendAsync(httpRequestMessage, cancellationToken);
	}

	public async Task<HttpResponseMessage> PostAsync(string url, string content, string contentType = "application/json", Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpRequestMessage httpRequestMessage = CreateRequest(HttpMethod.Post, url, configure);
		httpRequestMessage.Content = new StringContent(content, Encoding.UTF8, contentType);
		return await Client.SendAsync(httpRequestMessage, cancellationToken);
	}

	public async Task<HttpResponseMessage> PostAsync(string url, byte[] content, string contentType = "application/octet-stream", Action<HttpRequestOptions>? configure = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpRequestMessage httpRequestMessage = CreateRequest(HttpMethod.Post, url, configure);
		httpRequestMessage.Content = new ByteArrayContent(content);
		httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
		return await Client.SendAsync(httpRequestMessage, cancellationToken);
	}

	private HttpRequestMessage CreateRequest(HttpMethod method, string url, Action<HttpRequestOptions>? configure)
	{
		HttpRequestOptions httpRequestOptions = _defaultOptions.Clone();
		configure?.Invoke(httpRequestOptions);
		string requestUri = BuildUrl(url, httpRequestOptions.QueryParameters);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, requestUri);
		if (httpRequestOptions.HttpVersion != null)
		{
			httpRequestMessage.Version = httpRequestOptions.HttpVersion;
		}
		foreach (KeyValuePair<string, string> header in httpRequestOptions.Headers)
		{
			httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
		}
		return httpRequestMessage;
	}

	private string BuildUrl(string url, Dictionary<string, string> queryParams)
	{
		string text = (string.IsNullOrEmpty(_baseUrl) ? url : (_baseUrl + "/" + url.TrimStart('/')));
		if (queryParams.Count == 0)
		{
			return text;
		}
		string text2 = string.Join("&", queryParams.Select((KeyValuePair<string, string> kv) => Uri.EscapeDataString(kv.Key) + "=" + Uri.EscapeDataString(kv.Value)));
		string text3 = (text.Contains('?') ? "&" : "?");
		return text + text3 + text2;
	}

	private void ApplyDefaultOptions()
	{
		foreach (KeyValuePair<string, string> header in _defaultOptions.Headers)
		{
			Client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
		}
	}
}
