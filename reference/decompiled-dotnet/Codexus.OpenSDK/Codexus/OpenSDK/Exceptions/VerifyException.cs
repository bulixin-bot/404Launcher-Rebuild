using System;

namespace Codexus.OpenSDK.Exceptions;

public class VerifyException : Exception
{
	public VerifyException(string message)
		: base(message)
	{
	}
}
