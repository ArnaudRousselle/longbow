using System.ComponentModel;

namespace LongBow.Common.Enumerations
{
	public enum Orientation
	{
		[Description("Débit")]
		Negative,
		[Description("Crédit")]
		Positive,
	}

	public static class OrientationConverter
	{
		public static bool ConvertToBool(Orientation value)
		{
			return value == Orientation.Positive;
		}

		public static Orientation ConvertToEnum(bool value)
		{
			return value ? Orientation.Positive : Orientation.Negative;
		}
	}
}
