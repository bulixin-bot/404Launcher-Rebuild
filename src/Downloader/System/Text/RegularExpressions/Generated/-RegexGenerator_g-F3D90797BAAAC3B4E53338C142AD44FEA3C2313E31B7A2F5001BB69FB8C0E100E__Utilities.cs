using System.Buffers;
using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.11113")]
internal static class _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	internal static readonly SearchValues<string> s_indexOfString_bytes_Ordinal = SearchValues.Create(new ReadOnlySpan<string>("bytes"), StringComparison.Ordinal);
}
