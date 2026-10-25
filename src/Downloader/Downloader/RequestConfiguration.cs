using System;
using System.Net;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace Downloader;

public class RequestConfiguration
{
	public string Accept { get; set; }

	public bool AllowAutoRedirect { get; set; }

	public AuthenticationHeaderValue Authorization { get; set; }

	public DecompressionMethods AutomaticDecompression { get; set; }

	[Obsolete("This property is obsolete and has no effect.")]
	public RequestCachePolicy CachePolicy { get; set; }

	[Obsolete("This property is obsolete and has no effect.")]
	public string ConnectionGroupName { get; set; }

	public X509CertificateCollection ClientCertificates { get; set; }

	public string ContentType { get; set; }

	public CookieContainer CookieContainer { get; set; }

	public ICredentials Credentials { get; set; }

	public string Expect { get; set; }

	public WebHeaderCollection Headers { get; set; }

	public DateTime? IfModifiedSince { get; set; }

	public TokenImpersonationLevel ImpersonationLevel { get; set; }

	public bool KeepAlive { get; set; }

	public TimeSpan KeepAliveTimeout { get; set; }

	public int MaximumAutomaticRedirections { get; set; }

	public bool Pipelined { get; set; }

	public bool PreAuthenticate { get; set; }

	public Version ProtocolVersion { get; set; }

	public IWebProxy Proxy { get; set; }

	public string Referer { get; set; }

	[Obsolete("This property is obsolete and has no effect.")]
	public bool SendChunked { get; set; }

	public int Timeout { get; set; }

	public string TransferEncoding { get; set; }

	public bool UseDefaultCredentials { get; set; }

	public string UserAgent { get; set; }

	public RequestConfiguration()
	{
		Headers = new WebHeaderCollection();
		AllowAutoRedirect = true;
		AutomaticDecompression = DecompressionMethods.None;
		ClientCertificates = new X509CertificateCollection();
		ImpersonationLevel = TokenImpersonationLevel.Delegation;
		KeepAlive = false;
		KeepAliveTimeout = TimeSpan.FromMinutes(30L);
		MaximumAutomaticRedirections = 50;
		Pipelined = true;
		ProtocolVersion = HttpVersion.Version11;
		Timeout = 30000;
		UserAgent = "Downloader/" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
	}
}
