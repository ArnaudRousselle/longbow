using System;
using System.Xml.Serialization;
using LongBow.Dom.Constants;

namespace LongBow.Dom
{
	public class RepetitiveBilling : IDomBase
	{
		[XmlElement("RepetitiveBillingId")]
		public int Id { get; set; }
		public DateTime ValuationDate { get; set; } // date de valeur
		public string Title { get; set; } // libellé
		public double Amount { get; set; } // montant
		public bool Positive { get; set; } // sens
		public int FrequenceMode { get; set; } // fréquence de répétition

		public void ShiftValuationDate()
		{
			int shift;

			switch (FrequenceMode)
			{
				case FrequenceModeConstant.Monthly:
					shift = 1;
					break;
				case FrequenceModeConstant.Bimonthly:
					shift = 2;
					break;
				case FrequenceModeConstant.Quarterly:
					shift = 3;
					break;
				case FrequenceModeConstant.Annual:
					shift = 12;
					break;
				default:
					throw new NotImplementedException();
			}

			ValuationDate = ValuationDate
				.Date
				.AddMonths(shift);
		}
	}
}
