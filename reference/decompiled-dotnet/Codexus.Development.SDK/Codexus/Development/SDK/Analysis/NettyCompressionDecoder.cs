using System;
using System.Buffers;
using System.Collections.Generic;
using Codexus.Development.SDK.Extensions;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace Codexus.Development.SDK.Analysis;

public class NettyCompressionDecoder : ByteToMessageDecoder
{
	private const int InitialBufferSize = 4096;

	private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

	private readonly Inflater _inflater = new Inflater();

	public int Threshold { get; set; }

	public NettyCompressionDecoder(int threshold)
	{
		Threshold = threshold;
	}

	protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
	{
		if (input.ReadableBytes == 0)
		{
			return;
		}
		int num = input.ReadVarIntFromBuffer();
		if (num == 0)
		{
			output.Add(input.ReadBytes(input.ReadableBytes));
			return;
		}
		if (num < Threshold)
		{
			throw new DecoderException($"Decompressed length {num} is below threshold {Threshold}");
		}
		byte[] array = new byte[input.ReadableBytes];
		input.ReadBytes(array);
		byte[] array2 = _arrayPool.Rent(Math.Max(4096, num));
		try
		{
			_inflater.Reset();
			_inflater.SetInput(array);
			if (_inflater.IsNeedingDictionary)
			{
				throw new DecoderException("Inflater requires dictionary");
			}
			IByteBuffer byteBuffer = context.Allocator.HeapBuffer(num);
			int num2 = 0;
			while (!_inflater.IsFinished && num2 < num)
			{
				int num3 = _inflater.Inflate(array2);
				if (num3 == 0 && _inflater.IsNeedingInput)
				{
					throw new DecoderException("Incomplete compressed data");
				}
				byteBuffer.WriteBytes(array2, 0, num3);
				num2 += num3;
			}
			if (num2 != num)
			{
				byteBuffer.Release();
				throw new DecoderException($"Decompressed length mismatch: expected {num}, got {num2}");
			}
			output.Add(byteBuffer);
		}
		finally
		{
			_arrayPool.Return(array2);
		}
	}
}
