using System.Globalization;
using Codexus.OpenSDK.Entities.X19;

namespace Codexus.OpenSDK.Extensions;

public static class EnumExtensions
{
	extension(X19EnumGameType type)
	{
		public string ToOrdinalString()
		{
			int num = (int)type;
			return num.ToString(CultureInfo.InvariantCulture);
		}
	}
}
