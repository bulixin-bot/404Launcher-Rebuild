using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Codexus.Development.SDK.Analysis;

public class NettyEncryptionDecoder : ByteToMessageDecoder
{
	private readonly CfbBlockCipher _decipher;

	public NettyEncryptionDecoder(byte[] key)
	{
		_decipher = new CfbBlockCipher(new AesEngine(), 8);
		_decipher.Init(forEncryption: false, new ParametersWithIV(new KeyParameter(key), key));
	}

	protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
	{
		int readableBytes = input.ReadableBytes;
		IByteBuffer byteBuffer = context.Allocator.HeapBuffer(readableBytes);
		int num = input.ReaderIndex + input.ArrayOffset + readableBytes;
		int num2 = byteBuffer.ArrayOffset;
		for (int i = input.ReaderIndex + input.ArrayOffset; i < num; i++)
		{
			_decipher.ProcessBlock(input.Array, i, byteBuffer.Array, num2);
			num2++;
		}
		byteBuffer.SetWriterIndex(readableBytes);
		input.SkipBytes(readableBytes);
		output.Add(byteBuffer);
	}
}
