using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LongBow.Dom;
using LongBow.Dal.Utilities;

namespace LongBow.Dal.Implementations
{
	[Export(typeof(IBillingRepository))]
	public class BillingRepository : SerializableObject<Billing>, IBillingRepository
	{
		public void UpdateChecked(int billingId, bool isChecked)
		{
			var billingToUpdate = Data.First(n => n.Id == billingId);
			billingToUpdate.Checked = isChecked;

			IsDurty = true;
		}

		public void UpdateDelayed(int billingId, bool isDelayed)
		{
			var billingToUpdate = Data.First(n => n.Id == billingId);
			billingToUpdate.Delayed = isDelayed;

			IsDurty = true;
		}

		public void UpdateArchived(int billingId, bool isArchived)
		{
			var billingToUpdate = Data.First(n => n.Id == billingId);
			billingToUpdate.IsArchived = isArchived;

			IsDurty = true;
		}

		public List<Billing> FindAll(DateTime date, double amount)
		{
			return Data
				.Where(n =>
					n.ValuationDate.Year == date.Year
					&& n.ValuationDate.Month == date.Month
					&& n.ValuationDate.Day == date.Day
					&& Math.Abs(n.Amount*(n.Positive ? 1 : -1) - amount) < double.Epsilon)
				.Select(n => n.Clone())
				.ToList();
		}

		public List<Billing> FindAllWithExcludedDate(DateTime excludedDate, double amount)
		{
			return Data
				.Where(n => Math.Abs(n.Amount*(n.Positive ? 1 : -1) - amount) < double.Epsilon
							&& (n.ValuationDate.Year != excludedDate.Year
							|| n.ValuationDate.Month != excludedDate.Month
							|| n.ValuationDate.Day != excludedDate.Day)
							&& n.ValuationDate > excludedDate.AddDays(-5)
							&& n.ValuationDate < excludedDate.AddDays(5))
				.Select(n => n.Clone())
				.ToList();
		}
	}
}
