using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.11113")]
internal sealed class _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__RangePatternRegex_0 : Regex
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
					if (_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if (num <= inputSpan.Length - 8)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny(_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_indexOfString_bytes_Ordinal);
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
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int capturePosition = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				int num13 = 0;
				ReadOnlySpan<char> span = inputSpan.Slice(num);
				if (!span.StartsWith("bytes"))
				{
					UncaptureUntil(0);
					return false;
				}
				num += 5;
				span = inputSpan.Slice(num);
				num10 = num;
				int i;
				for (i = 0; (uint)i < (uint)span.Length && char.IsWhiteSpace(span[i]); i++)
				{
				}
				span = span.Slice(i);
				num += i;
				num11 = num;
				while (true)
				{
					num9 = Crawlpos();
					num5 = num;
					num4 = num;
					num3 = Crawlpos();
					num6 = num;
					int j;
					for (j = 0; (uint)j < (uint)span.Length && char.IsDigit(span[j]); j++)
					{
					}
					span = span.Slice(j);
					num += j;
					Capture(2, num6, num);
					int k;
					for (k = 0; (uint)k < (uint)span.Length && char.IsWhiteSpace(span[k]); k++)
					{
					}
					span = span.Slice(k);
					num += k;
					if (!span.IsEmpty && span[0] == '-')
					{
						num++;
						span = inputSpan.Slice(num);
						num12 = num;
						int l;
						for (l = 0; (uint)l < (uint)span.Length && char.IsWhiteSpace(span[l]); l++)
						{
						}
						span = span.Slice(l);
						num += l;
						num13 = num;
						goto IL_01fd;
					}
					goto IL_024d;
					IL_024d:
					num = num4;
					span = inputSpan.Slice(num);
					UncaptureUntil(num3);
					if (span.IsEmpty || span[0] != '*')
					{
						goto IL_00a1;
					}
					num2 = 1;
					num++;
					span = inputSpan.Slice(num);
					goto IL_02a9;
					IL_00a1:
					UncaptureUntil(num9);
					if (_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num10 >= num11)
					{
						break;
					}
					num = --num11;
					span = inputSpan.Slice(num);
					continue;
					IL_01fd:
					capturePosition = Crawlpos();
					num7 = num;
					int m;
					for (m = 0; (uint)m < (uint)span.Length && char.IsDigit(span[m]); m++)
					{
					}
					span = span.Slice(m);
					num += m;
					Capture(3, num7, num);
					num2 = 0;
					goto IL_02a9;
					IL_02a9:
					while (true)
					{
						Capture(1, num5, num);
						int n;
						for (n = 0; (uint)n < (uint)span.Length && char.IsWhiteSpace(span[n]); n++)
						{
						}
						span = span.Slice(n);
						num += n;
						if (!span.IsEmpty && span[0] == '/')
						{
							int num14;
							for (num14 = 1; (uint)num14 < (uint)span.Length && char.IsWhiteSpace(span[num14]); num14++)
							{
							}
							span = span.Slice(num14);
							num += num14;
							num8 = num;
							int num15 = num;
							int num16;
							for (num16 = 0; (uint)num16 < (uint)span.Length && char.IsDigit(span[num16]); num16++)
							{
							}
							if (num16 != 0)
							{
								span = span.Slice(num16);
								num += num16;
							}
							else
							{
								num = num15;
								span = inputSpan.Slice(num);
								if (span.IsEmpty || span[0] != '*')
								{
									goto IL_028f;
								}
								num++;
								span = inputSpan.Slice(num);
							}
							Capture(4, num8, num);
							runtextpos = num;
							Capture(0, start, num);
							return true;
						}
						goto IL_028f;
						IL_028f:
						if (_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_hasTimeout)
						{
							CheckTimeout();
						}
						if (num2 == 0)
						{
							break;
						}
						if (num2 != 1)
						{
							continue;
						}
						goto IL_00a1;
					}
					UncaptureUntil(capturePosition);
					if (_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_hasTimeout)
					{
						CheckTimeout();
					}
					if (num12 < num13)
					{
						num = --num13;
						span = inputSpan.Slice(num);
						goto IL_01fd;
					}
					goto IL_024d;
				}
				UncaptureUntil(0);
				return false;
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				void UncaptureUntil(int num17)
				{
					while (Crawlpos() > num17)
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

	internal static readonly _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__RangePatternRegex_0 Instance = new _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__RangePatternRegex_0();

	private _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__RangePatternRegex_0()
	{
		pattern = "bytes\\s*((?<from>\\d*)\\s*-\\s*(?<to>\\d*)|\\*)\\s*\\/\\s*(?<size>\\d+|\\*)";
		roptions = RegexOptions.Compiled;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EF3D90797BAAAC3B4E53338C142AD44FEA3C2313E31B7A2F5001BB69FB8C0E100E__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		CapNames = new Hashtable
		{
			{ "0", 0 },
			{ "1", 1 },
			{ "from", 2 },
			{ "size", 4 },
			{ "to", 3 }
		};
		capslist = new string[5] { "0", "1", "from", "to", "size" };
		capsize = 5;
	}
}
