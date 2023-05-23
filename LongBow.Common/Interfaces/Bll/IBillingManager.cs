using System;
using System.Collections.Generic;
using System.Xml;
using LongBow.Dom;

namespace LongBow.Common.Interfaces.Bll
{
	public interface IBillingManager
	{
		bool IsDurty { get; }

		void FindBilling(DateTime date, double amount, out List<Billing> perfectMatchings,
			out List<Billing> incompleteMatchings);
		void AddBilling(Billing billing);
		void RemoveBilling(int billingId);
		void UpdateBilling(Billing billing);
		void UpdateChecked(int billingId, bool isChecked);
		void UpdateDelayed(int billingId, bool isDelayed);
		void UpdateArchived(int billingId, bool isArchived);
		int MergeBillings(List<BillingForMerging> billings);
		List<Billing> GetBillings();
		Billing GetBilling(int billingId);
		void Save(XmlTextWriter writer);
		void Load(XmlDocument xmlDocument);
	}

	public class BillingForMerging
	{
		public int BillingId { get; set; }
		public double Amount { get; set; }
		public bool Positive { get; set; }
		public DateTime TransactionDate { get; set; }
		public DateTime ValuationDate { get; set; }
		public bool Checked { get; set; }
		public bool Delayed { get; set; }
		public string Title { get; set; }

		public BillingForMerging(
			int billingId,
			double amount,
			bool positive,
			DateTime transactionDate,
			DateTime valuationDate,
			bool isChecked,
			bool delayed,
			string title)
		{
			BillingId = billingId;
			Amount = amount;
			Positive = positive;
			TransactionDate = transactionDate;
			ValuationDate = valuationDate;
			Checked = isChecked;
			Delayed = delayed;
			Title = title;
		}
	}
}
