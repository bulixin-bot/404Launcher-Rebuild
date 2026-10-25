using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.13.1716")]
internal sealed class _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__CaptchaRegex_0 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				while (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan) && runtextpos != inputSpan.Length)
				{
					runtextpos++;
					if (_003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 27)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__Utilities.s_indexOfString_name_OrdinalIgnoreCase);
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if ((uint)span.Length < 4u || !span.StartsWith("name", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				int i;
				for (i = 4; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
				{
				}
				span = span.Slice(i);
				num += i;
				if (span.IsEmpty || span[0] != '=')
				{
					UncaptureUntil(0);
					return false;
				}
				int j;
				for (j = 1; (uint)j < (uint)span.Length && char.IsWhiteSpace(span[j]); j++)
				{
				}
				span = span.Slice(j);
				num += j;
				char c;
				if ((uint)span.Length < 12u || (((c = span[0]) != '"') & (c != '\'')) || !span.Slice(1).StartsWith("captcha_", StringComparison.OrdinalIgnoreCase) || ((((c = span[9]) | 0x20) != 105) & (c != 'İ')) || (span[10] | 0x20) != 100 || (((c = span[11]) != '"') & (c != '\'')))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 12;
				span = inputSpan.Slice(num);
				int k;
				for (k = 0; (uint)k < (uint)span.Length && char.IsWhiteSpace(span[k]); k++)
				{
				}
				if (k == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(k);
				num += k;
				if ((uint)span.Length < 5u || !span.StartsWith("value", StringComparison.OrdinalIgnoreCase))
				{
					UncaptureUntil(0);
					return false;
				}
				int l;
				for (l = 5; (uint)l < (uint)span.Length && char.IsWhiteSpace(span[l]); l++)
				{
				}
				span = span.Slice(l);
				num += l;
				if (span.IsEmpty || span[0] != '=')
				{
					UncaptureUntil(0);
					return false;
				}
				int m;
				for (m = 1; (uint)m < (uint)span.Length && char.IsWhiteSpace(span[m]); m++)
				{
				}
				span = span.Slice(m);
				num += m;
				if (span.IsEmpty || (((c = span[0]) != '"') & (c != '\'')))
				{
					UncaptureUntil(0);
					return false;
				}
				num++;
				span = inputSpan.Slice(num);
				num2 = num;
				int num3 = span.IndexOfAny('"', '\'');
				if (num3 < 0)
				{
					num3 = span.Length;
				}
				if (num3 == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(num3);
				num += num3;
				Capture(1, num2, num);
				if (span.IsEmpty || (((c = span[0]) != '"') & (c != '\'')))
				{
					UncaptureUntil(0);
					return false;
				}
				Capture(0, start, runtextpos = num + 1);
				return true;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int capturePosition)
				{
					while (Crawlpos() > capturePosition)
					{
						Uncapture();
					}
				}
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__CaptchaRegex_0 Instance = new _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__CaptchaRegex_0();

	private _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__CaptchaRegex_0()
	{
		pattern = "name\\s*=\\s*[\"\"']captcha_id[\"\"']\\s+value\\s*=\\s*[\"\"']([^\"\"']+)[\"\"']";
		roptions = RegexOptions.IgnoreCase;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFF0C8D718C50A26EBCA5458D7DAC54129957934359A242FDCB7E8C16AFC9C3BD4__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 2;
	}
}
