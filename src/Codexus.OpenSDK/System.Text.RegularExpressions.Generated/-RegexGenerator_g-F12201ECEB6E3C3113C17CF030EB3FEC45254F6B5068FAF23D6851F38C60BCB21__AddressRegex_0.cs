using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.13.1716")]
internal sealed class _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__AddressRegex_0 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				if (TryFindNextPossibleStartingPosition(inputSpan) && !TryMatchAtCurrentPosition(inputSpan))
				{
					runtextpos = inputSpan.Length;
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 3 && num == 0)
				{
					return true;
				}
				runtextpos = inputSpan.Length;
				return false;
			}

			private bool TryMatchAtCurrentPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				int start = num;
				int num2 = 0;
				int num3 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (num != 0)
				{
					UncaptureUntil(0);
					return false;
				}
				num2 = num;
				int num4 = span.IndexOf(':');
				if (num4 < 0)
				{
					num4 = span.Length;
				}
				if (num4 == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(num4);
				num += num4;
				Capture(1, num2, num);
				if (span.IsEmpty || span[0] != ':')
				{
					UncaptureUntil(0);
					return false;
				}
				num++;
				span = inputSpan.Slice(num);
				num3 = num;
				int i;
				for (i = 0; (uint)i < (uint)span.Length && char.IsDigit(span[i]); i++)
				{
				}
				if (i == 0)
				{
					UncaptureUntil(0);
					return false;
				}
				span = span.Slice(i);
				num += i;
				Capture(2, num3, num);
				if (num < inputSpan.Length - 1 || ((uint)num < (uint)inputSpan.Length && inputSpan[num] != '\n'))
				{
					UncaptureUntil(0);
					return false;
				}
				runtextpos = num;
				Capture(0, start, num);
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

	internal static readonly _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__AddressRegex_0 Instance = new _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__AddressRegex_0();

	private _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__AddressRegex_0()
	{
		pattern = "^(?<address>[^:]+):(?<port>\\d+)$";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EF12201ECEB6E3C3113C17CF030EB3FEC45254F6B5068FAF23D6851F38C60BCB21__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		CapNames = new Hashtable
		{
			{ "0", 0 },
			{ "address", 1 },
			{ "port", 2 }
		};
		capslist = new string[3] { "0", "address", "port" };
		capsize = 3;
	}
}
