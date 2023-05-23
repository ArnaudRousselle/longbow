namespace LongBow.Common.Interfaces.Bll
{
	public interface IBusinessContext
	{
		IBillingManager BillingManager { get; }
		IBillingCalculationManager BillingCalculationManager { get; }
		IRepetitiveBillingManager RepetitiveBillingManager { get; }
		void Save();
		void SaveAs(string filePath);
		void Load(string filePath, bool keepEntry);
		bool IsDurty { get; }
	}
}
