using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Downloader;

public class Request
{
	public readonly RequestConfiguration Configuration;

	public Uri Address { get; set; }

	public string FileName { get; set; }

	public Request(string address)
		: this(address, new RequestConfiguration())
	{
	}

	public Request(string address, RequestConfiguration config)
	{
		if (!Uri.TryCreate(address, UriKind.Absolute, out Uri result))
		{
			result = new Uri(new Uri("http://localhost"), address);
		}
		Address = result;
		Configuration = config ?? new RequestConfiguration();
	}

	public HttpRequestMessage GetRequest()
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, Address);
		httpRequestMessage.Version = Configuration.ProtocolVersion;
		httpRequestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
		httpRequestMessage.Headers.IfModifiedSince = Configuration.IfModifiedSince;
		if (Configuration.Authorization != null)
		{
			httpRequestMessage.Headers.Authorization = Configuration.Authorization;
		}
		else if (Configuration.Credentials != null)
		{
			if (Configuration.Credentials is NetworkCredential networkCredential)
			{
				if (!string.IsNullOrWhiteSpace(networkCredential.UserName) && !string.IsNullOrWhiteSpace(networkCredential.Password))
				{
					httpRequestMessage.Headers.Authorization = GetAuthHeader(networkCredential.UserName, networkCredential.Password);
				}
			}
			else if (Configuration.Credentials is CredentialCache credentialCache)
			{
				NetworkCredential credential = credentialCache.GetCredential(Address, "Basic");
				if (credential != null)
				{
					httpRequestMessage.Headers.Authorization = GetAuthHeader(credential.UserName, credential.Password);
				}
			}
		}
		return httpRequestMessage;
	}

	private static AuthenticationHeaderValue GetAuthHeader(string user, string password)
	{
		return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password)));
	}

	public string GetFileNameFromUrl()
	{
		string text = Path.GetFileName(Address.LocalPath);
		int num = text.IndexOf('?', StringComparison.Ordinal);
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		return text;
	}

	public string ToUnicode(string otherEncodedText)
	{
		return Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(otherEncodedText));
	}
}
