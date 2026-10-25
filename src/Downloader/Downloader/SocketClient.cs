using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Threading;
using System.Threading.Tasks;
using Downloader.Extensions.Helpers;

namespace Downloader;

public class SocketClient : IDisposable
{
	private const string HeaderContentLengthKey = "Content-Length";

	private const string HeaderContentDispositionKey = "Content-Disposition";

	private const string HeaderContentRangeKey = "Content-Range";

	private const string HeaderAcceptRangesKey = "Accept-Ranges";

	private const string FilenameStartPointKey = "filename=";

	private readonly Regex _contentRangePattern = RangePatternRegex();

	private bool _isDisposed;

	private bool? _isSupportDownloadInRange;

	private ConcurrentDictionary<string, string> ResponseHeaders { get; set; } = new ConcurrentDictionary<string, string>();

	private HttpClient Client { get; }
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.11113")]
	private static Regex RangePatternRegex()
	{
		return _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__RangePatternRegex_0.Instance;
	}

	public SocketClient(RequestConfiguration config)
	{
		Client = GetHttpClientWithSocketHandler(config);
	}

	private SocketsHttpHandler GetSocketsHttpHandler(RequestConfiguration config)
	{
		SocketsHttpHandler socketsHttpHandler = new SocketsHttpHandler
		{
			AllowAutoRedirect = config.AllowAutoRedirect,
			MaxAutomaticRedirections = config.MaximumAutomaticRedirections,
			AutomaticDecompression = config.AutomaticDecompression,
			PreAuthenticate = config.PreAuthenticate,
			UseCookies = (config.CookieContainer != null),
			UseProxy = (config.Proxy != null),
			MaxConnectionsPerServer = 1000,
			PooledConnectionIdleTimeout = config.KeepAliveTimeout,
			PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
			EnableMultipleHttp2Connections = true,
			ConnectTimeout = TimeSpan.FromMilliseconds(config.Timeout, 0L)
		};
		X509CertificateCollection clientCertificates = config.ClientCertificates;
		if (clientCertificates != null && clientCertificates.Count > 0)
		{
			socketsHttpHandler.SslOptions.ClientCertificates = config.ClientCertificates;
		}
		socketsHttpHandler.SslOptions.EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
		socketsHttpHandler.SslOptions.RemoteCertificateValidationCallback = ExceptionHelper.CertificateValidationCallBack;
		if (config.KeepAlive)
		{
			socketsHttpHandler.KeepAlivePingTimeout = config.KeepAliveTimeout;
			socketsHttpHandler.KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests;
		}
		if (config.Credentials != null)
		{
			socketsHttpHandler.Credentials = config.Credentials;
			socketsHttpHandler.PreAuthenticate = config.PreAuthenticate;
		}
		if (socketsHttpHandler.UseCookies && config.CookieContainer != null)
		{
			socketsHttpHandler.CookieContainer = config.CookieContainer;
		}
		if (socketsHttpHandler.UseProxy && config.Proxy != null)
		{
			socketsHttpHandler.Proxy = config.Proxy;
		}
		if (!string.IsNullOrWhiteSpace(config.Expect))
		{
			socketsHttpHandler.Expect100ContinueTimeout = TimeSpan.FromSeconds(1L);
		}
		return socketsHttpHandler;
	}

	private HttpClient GetHttpClientWithSocketHandler(RequestConfiguration config)
	{
		HttpClient httpClient = new HttpClient(GetSocketsHttpHandler(config));
		httpClient.DefaultRequestHeaders.Clear();
		if (!string.IsNullOrWhiteSpace(config.Accept))
		{
			httpClient.DefaultRequestHeaders.Add("Accept", config.Accept);
		}
		if (!string.IsNullOrWhiteSpace(config.UserAgent))
		{
			httpClient.DefaultRequestHeaders.Add("User-Agent", config.UserAgent);
		}
		httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
		httpClient.DefaultRequestHeaders.Add("Connection", config.KeepAlive ? "keep-alive" : "close");
		httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
		{
			NoCache = true
		};
		WebHeaderCollection headers = config.Headers;
		if (headers != null && headers.Count > 0)
		{
			string[] allKeys = config.Headers.AllKeys;
			foreach (string text in allKeys)
			{
				if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(config.Headers[text]))
				{
					httpClient.DefaultRequestHeaders.Add(text, config.Headers[text]);
				}
			}
		}
		if (!string.IsNullOrWhiteSpace(config.Referer))
		{
			httpClient.DefaultRequestHeaders.Referrer = new Uri(config.Referer);
		}
		if (!string.IsNullOrWhiteSpace(config.ContentType))
		{
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(config.ContentType));
		}
		if (!string.IsNullOrWhiteSpace(config.TransferEncoding))
		{
			httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue(config.TransferEncoding));
			httpClient.DefaultRequestHeaders.TransferEncoding.Add(new TransferCodingHeaderValue(config.TransferEncoding));
		}
		if (!string.IsNullOrWhiteSpace(config.Expect))
		{
			httpClient.DefaultRequestHeaders.Add("Expect", config.Expect);
		}
		return httpClient;
	}

	private async Task FetchResponseHeaders(Request request, bool addRange, CancellationToken cancelToken = default(CancellationToken))
	{
		string redirectedUrl = default;
		try
		{
			if (!ResponseHeaders.IsEmpty)
			{
				return;
			}
			HttpRequestMessage request2 = request.GetRequest();
			if (addRange)
			{
				request2.Headers.Range = new RangeHeaderValue(0L, 0L);
			}
			using HttpResponseMessage response = await SendRequestAsync(request2, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
			if (response.StatusCode.IsRedirectStatus() && request.Configuration.AllowAutoRedirect)
			{
				return;
			}
			if (response.Headers.Location != null)
			{
				string text = response.Headers.Location.ToString();
				if (!string.IsNullOrWhiteSpace(text) && !request.Address.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					request.Address = new Uri(text);
					await FetchResponseHeaders(request, addRange: true, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
					return;
				}
			}
			EnsureResponseAddressIsSameWithOrigin(request, response);
		}
		catch (HttpRequestException ex) when (ex.IsRequestedRangeNotSatisfiable())
		{
			await FetchResponseHeaders(request, addRange: false, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (HttpRequestException error) when (request.Configuration.AllowAutoRedirect && error.IsRedirectError() && ResponseHeaders.TryGetValue("location", out redirectedUrl) && !string.IsNullOrWhiteSpace(redirectedUrl) && !request.Address.ToString().Equals(redirectedUrl, StringComparison.OrdinalIgnoreCase))
		{
			request.Address = new Uri(redirectedUrl);
			await FetchResponseHeaders(request, addRange: true, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void EnsureResponseAddressIsSameWithOrigin(Request request, HttpResponseMessage response)
	{
		Uri redirectUrl = GetRedirectUrl(response);
		if (redirectUrl != null)
		{
			request.Address = redirectUrl;
		}
	}

	internal Uri GetRedirectUrl(HttpResponseMessage response)
	{
		Uri uri = response?.Headers.Location;
		if (uri != null)
		{
			return uri;
		}
		return response?.RequestMessage?.RequestUri;
	}

	public async ValueTask<long> GetFileSizeAsync(Request request)
	{
		if (await IsSupportDownloadInRange(request).ConfigureAwait(continueOnCapturedContext: false))
		{
			return GetTotalSizeFromContentRange(ResponseHeaders.ToDictionary());
		}
		return GetTotalSizeFromContentLength(ResponseHeaders.ToDictionary());
	}

	internal long GetTotalSizeFromContentLength(Dictionary<string, string> headers)
	{
		if (headers.TryGetValue("Content-Length", out var value) && long.TryParse(value, out var result))
		{
			return result;
		}
		return -1L;
	}

	public async ValueTask ThrowIfIsNotSupportDownloadInRange(Request request)
	{
		if (!(await IsSupportDownloadInRange(request).ConfigureAwait(continueOnCapturedContext: false)))
		{
			throw new NotSupportedException("The downloader cannot continue downloading because the network or server failed to download in range.");
		}
	}

	public async ValueTask<bool> IsSupportDownloadInRange(Request request)
	{
		if (_isSupportDownloadInRange.HasValue)
		{
			return _isSupportDownloadInRange.Value;
		}
		await FetchResponseHeaders(request, addRange: true).ConfigureAwait(continueOnCapturedContext: false);
		if (ResponseHeaders.TryGetValue("Accept-Ranges", out var value) && value.Equals("none", StringComparison.OrdinalIgnoreCase))
		{
			_isSupportDownloadInRange = false;
			return false;
		}
		if (ResponseHeaders.TryGetValue("Content-Range", out var value2) && !string.IsNullOrWhiteSpace(value2))
		{
			_isSupportDownloadInRange = true;
			return true;
		}
		_isSupportDownloadInRange = false;
		return false;
	}

	internal long GetTotalSizeFromContentRange(Dictionary<string, string> headers)
	{
		if (headers.TryGetValue("Content-Range", out var value) && !string.IsNullOrWhiteSpace(value) && _contentRangePattern.IsMatch(value))
		{
			if (!long.TryParse(_contentRangePattern.Match(value).Groups["size"].Value, out var result))
			{
				return -1L;
			}
			return result;
		}
		return -1L;
	}

	public async Task<string> SetRequestFileNameAsync(Request request)
	{
		if (!string.IsNullOrWhiteSpace(request.FileName))
		{
			return request.FileName;
		}
		string text = await GetUrlDispositionFilenameAsync(request).ConfigureAwait(continueOnCapturedContext: false);
		if (string.IsNullOrWhiteSpace(text))
		{
			text = request.GetFileNameFromUrl();
			if (string.IsNullOrWhiteSpace(text))
			{
				text = Guid.NewGuid().ToString("N");
			}
		}
		request.FileName = text;
		return text;
	}

	internal async Task<string> GetUrlDispositionFilenameAsync(Request request)
	{
		try
		{
			if (request.Address == null || !request.Address.IsAbsoluteUri)
			{
				return null;
			}
			if (!request.Address.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) && !request.Address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			if (string.IsNullOrWhiteSpace(request.Address.Host))
			{
				return null;
			}
			if (string.IsNullOrWhiteSpace(request.Address.AbsolutePath) || request.Address.AbsolutePath.Equals("/", StringComparison.OrdinalIgnoreCase) || request.Address.Segments.Length <= 1)
			{
				return null;
			}
			await FetchResponseHeaders(request, addRange: true).ConfigureAwait(continueOnCapturedContext: false);
			if (ResponseHeaders.TryGetValue("Content-Disposition", out var value))
			{
				string text = request.ToUnicode(value);
				if (!string.IsNullOrWhiteSpace(text))
				{
					string text2 = text.Split(';').FirstOrDefault((string part) => part.Trim().StartsWith("filename=", StringComparison.OrdinalIgnoreCase));
					if (!string.IsNullOrWhiteSpace(text2))
					{
						return text2.Replace("filename=", "").Replace("\"", "").Trim();
					}
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancelToken = default(CancellationToken))
	{
		HttpResponseMessage httpResponseMessage = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(continueOnCapturedContext: false);
		ResponseHeaders.Clear();
		foreach (KeyValuePair<string, IEnumerable<string>> header in httpResponseMessage.Headers)
		{
			ResponseHeaders.TryAdd(header.Key, header.Value.FirstOrDefault());
		}
		foreach (KeyValuePair<string, IEnumerable<string>> header2 in httpResponseMessage.Content.Headers)
		{
			ResponseHeaders.TryAdd(header2.Key, header2.Value.FirstOrDefault());
		}
		httpResponseMessage.EnsureSuccessStatusCode();
		return httpResponseMessage;
	}

	public void Dispose()
	{
		if (!_isDisposed)
		{
			_isDisposed = true;
			Client?.Dispose();
		}
	}
}
