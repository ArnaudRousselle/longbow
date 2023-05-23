using System.Xml;
using LongBow.Dom;
using System;
using System.Collections.Generic;

namespace LongBow.Dal
{
	public interface IBillingRepository
	{
		void Add(Billing billing);
		void Remove(int billingId);
		void Update(Billing billing);
		void UpdateChecked(int billingId, bool isChecked);
		void UpdateDelayed(int billingId, bool isDelayed);
		void UpdateArchived(int billingId, bool isArchived);
		List<Billing> GetAll();
		Billing Get(int billingId);
		List<Billing> FindAll(DateTime date, double amount);
		List<Billing> FindAllWithExcludedDate(DateTime excludedDate, double amount);
		void Save(XmlTextWriter writer);
		void Load(XmlDocument xmlDocument);
		bool IsDurty { get; }
	}
}
