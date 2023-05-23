using System.Collections.Generic;

namespace LongBow.FileImport.Objects
{
	public class DataFromFile
	{
		public string BankId { get; set;} //code banque
		public string BranchId { get; set; } //code agence
		public string AcctId { get; set; } //numéro de compte
		public string CurDef { get; set; } //devise
		public List<DataLine> Lines { get; set; } 
	}
}
