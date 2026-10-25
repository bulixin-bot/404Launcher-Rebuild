using System.Text;
using Codexus.OpenSDK.Cipher;
using Codexus.OpenSDK.Extensions;

namespace Codexus.OpenSDK.Entities.Yggdrasil;

public class UserProfile
{
	private static readonly byte[] TokenKey = new byte[16]
	{
		172, 36, 156, 105, 199, 44, 179, 180, 78, 192,
		204, 108, 84, 58, 129, 149
	};

	public required int UserId { get; set; }

	public required string UserToken { get; set; }

	public int GetAuthId()
	{
		return Skip32Cipher.Encrypt(UserId, "SaintSteve"u8.ToArray());
	}

	public byte[] GetAuthToken()
	{
		return Encoding.ASCII.GetBytes(UserToken).Xor(TokenKey);
	}
}
