using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;

namespace Codexus.OpenSDK.Cipher;

public sealed class ChaChaPacker : ChaCha7539Engine
{
	public override string AlgorithmName => $"ChaCha{rounds}";

	public ChaChaPacker(byte[] key, byte[] iv, bool encryption, int rounds = 8)
	{
		base.rounds = rounds;
		Init(encryption, new ParametersWithIV(new KeyParameter(key), iv));
	}
}
