using System;

namespace LongBow.FileImport.Objects
{
	public class DataLine
	{
		public DateTime DtPosted { get; set; } //date
		public string FitId { get; set; } //numéro d'opération
		public string Name { get; set; }
        public string Memo { get; set; }
        public double TrnAmt { get; set; }//montant
        public string AccountId { get; set; }

	}
}
