using System.Buffers;
using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.13.1716")]
internal static class _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__Utilities
{
	internal static readonly TimeSpan s_defaultTimeout = ((AppContext.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") is TimeSpan timeSpan) ? timeSpan : Regex.InfiniteMatchTimeout);

	internal static readonly bool s_hasTimeout = s_defaultTimeout != Regex.InfiniteMatchTimeout;

	internal static readonly SearchValues<string> s_indexOfString_name_OrdinalIgnoreCase = SearchValues.Create(new ReadOnlySpan<string>("name"), StringComparison.OrdinalIgnoreCase);
}
