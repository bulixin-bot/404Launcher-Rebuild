using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Codexus.Development.SDK.Analysis;

public class NettyEncryptionEncoder : MessageToByteEncoder<IByteBuffer>
{
	private readonly CfbBlockCipher _encryptor;

	public NettyEncryptionEncoder(byte[] key)
	{
		_encryptor = new CfbBlockCipher(new AesEngine(), 8);
		_encryptor.Init(forEncryption: true, new ParametersWithIV(new KeyParameter(key), key));
	}

	protected override void Encode(IChannelHandlerContext context, IByteBuffer message, IByteBuffer output)
	{
		int readableBytes = message.ReadableBytes;
		output.EnsureWritable(readableBytes);
		int num = readableBytes + message.ArrayOffset + message.ReaderIndex;
		int num2 = output.ArrayOffset;
		output.SetWriterIndex(readableBytes);
		for (int i = message.ArrayOffset + message.ReaderIndex; i < num; i++)
		{
			_encryptor.ProcessBlock(message.Array, i, output.Array, num2);
			num2++;
		}
		message.SkipBytes(readableBytes);
	}
}
