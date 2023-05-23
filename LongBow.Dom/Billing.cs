using System;
using System.Xml.Serialization;

namespace LongBow.Dom
{
	public class Billing : IDomBase
	{
		[XmlElement("BillingId")]
		public int Id { get; set; }
		public DateTime TransactionDate { get; set; } // date de la transaction
		public DateTime ValuationDate { get; set; } // date de valeur
		public string Title { get; set; } // libellé
		public double Amount { get; set; } // montant
		public bool Positive { get; set; } // sens
		public bool Checked { get; set; } // pointé
		public bool Delayed { get; set; } // différé
		public string Comment { get; set; } // commentaire
		public bool IsArchived { get; set; } // archivée ?
		public bool IsSaving { get; set; } // économies ?
	}
}
