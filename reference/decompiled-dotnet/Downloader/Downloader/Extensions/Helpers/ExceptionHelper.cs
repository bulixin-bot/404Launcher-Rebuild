using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Downloader.Extensions.Helpers;

internal static class ExceptionHelper
{
	internal static bool IsRedirectError(this Exception error)
	{
		if (error is HttpRequestException { StatusCode: not null, StatusCode: var statusCode })
		{
			return statusCode.Value.IsRedirectStatus();
		}
		return false;
	}

	internal static bool IsRedirectStatus(this HttpStatusCode statusCode)
	{
		if ((uint)(statusCode - 301) <= 2u || (uint)(statusCode - 307) <= 1u)
		{
			return true;
		}
		return false;
	}

	internal static bool IsRequestedRangeNotSatisfiable(this Exception error)
	{
		if (error is HttpRequestException { StatusCode: var statusCode } && statusCode.HasValue)
		{
			return statusCode == HttpStatusCode.RequestedRangeNotSatisfiable;
		}
		return false;
	}

	internal static bool IsMomentumError(this Exception error)
	{
		if ((error is HttpRequestException { StatusCode: { } statusCode } && (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.BadGateway)) ? true : false)
		{
			return false;
		}
		if ((error is HttpRequestException { StatusCode: { } statusCode2 } && (statusCode2 == HttpStatusCode.MultipleChoices || statusCode2 == HttpStatusCode.TooManyRequests || (uint)(statusCode2 - 503) <= 1u)) ? true : false)
		{
			return true;
		}
		if (!error.IsRedirectError() && !error.HasSource("System.Net.Http", "System.Net.Sockets", "System.Net.Security"))
		{
			return error.HasTypeOf(typeof(WebException), typeof(SocketException));
		}
		return true;
	}

	internal static bool HasTypeOf(this Exception exp, params Type[] types)
	{
		for (Exception ex = exp; ex != null; ex = ex.InnerException)
		{
			foreach (Type type in types)
			{
				if (ex.GetType() == type)
				{
					return true;
				}
			}
		}
		return false;
	}

	internal static bool HasSource(this Exception exp, params string[] sources)
	{
		for (Exception ex = exp; ex != null; ex = ex.InnerException)
		{
			foreach (string b in sources)
			{
				if (string.Equals(ex.Source, b, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	internal static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		if (sslPolicyErrors == SslPolicyErrors.None)
		{
			return true;
		}
		if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
		{
			if (chain?.ChainStatus != null)
			{
				X509ChainStatus[] chainStatus = chain.ChainStatus;
				for (int i = 0; i < chainStatus.Length; i++)
				{
					X509ChainStatus x509ChainStatus = chainStatus[i];
					if (x509ChainStatus.Status == X509ChainStatusFlags.NotTimeValid)
					{
						return true;
					}
					if ((x509ChainStatus.Status != X509ChainStatusFlags.UntrustedRoot || !(certificate.Subject == certificate.Issuer)) && x509ChainStatus.Status != X509ChainStatusFlags.NoError)
					{
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
}
