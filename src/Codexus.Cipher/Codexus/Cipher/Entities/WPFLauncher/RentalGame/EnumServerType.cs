using System.Runtime.Serialization;

namespace Codexus.Cipher.Entities.WPFLauncher.RentalGame;

public enum EnumServerType
{
	[EnumMember(Value = "docker")]
	Docker,
	[EnumMember(Value = "vmware")]
	Vmware
}
