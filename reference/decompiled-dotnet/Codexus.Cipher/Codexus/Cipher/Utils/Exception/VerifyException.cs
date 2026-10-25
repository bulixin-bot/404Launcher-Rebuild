using System;

namespace Codexus.Cipher.Utils.Exception;

public class VerifyException : System.Exception
{
	public VerifyException(string message)
		: base(message)
	{
	}

	public static VerifyException Clone(VerifyException exception, string append)
	{
		return new VerifyException(exception.Message + "|" + append);
	}
}
