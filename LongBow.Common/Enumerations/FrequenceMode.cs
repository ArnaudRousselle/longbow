using System;
using System.ComponentModel;
using LongBow.Dom.Constants;

namespace LongBow.Common.Enumerations
{
	public enum FrequenceMode
	{
		[Description("Mensuel")]
		Monthly,
		[Description("Bimestriel")]
		Bimonthly,
		[Description("Trimestriel")]
		Quarterly,
		[Description("Annuel")]
		Annual
	}

	public static class FrequenceModeConverter
	{
		public static int ConvertToInt(FrequenceMode value)
		{
			switch (value)
			{
				case FrequenceMode.Monthly:
					return FrequenceModeConstant.Monthly;

				case FrequenceMode.Bimonthly:
					return FrequenceModeConstant.Bimonthly;

				case FrequenceMode.Quarterly:
					return FrequenceModeConstant.Quarterly;

				case FrequenceMode.Annual:
					return FrequenceModeConstant.Annual;
			}

			throw new ArgumentException("value");
		}

		public static FrequenceMode ConvertToEnum(int value)
		{
			switch (value)
			{
				case FrequenceModeConstant.Monthly:
					return FrequenceMode.Monthly;

				case FrequenceModeConstant.Bimonthly:
					return FrequenceMode.Bimonthly;

				case FrequenceModeConstant.Quarterly:
					return FrequenceMode.Quarterly;

				case FrequenceModeConstant.Annual:
					return FrequenceMode.Annual;
			}

			throw new ArgumentException("value");
		}
	}
}
