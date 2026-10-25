using System;

namespace Codexus.Protocol772Compat;

public static class Protocol772Registration
{
	public static Type PacketType => typeof(LoginSuccess772Compat);
}
