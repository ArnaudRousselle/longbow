using System.Collections.Generic;
using System.Xml;
using LongBow.Dom;

namespace LongBow.Common.Interfaces.Bll
{
	public interface IRepetitiveBillingManager
	{
		bool IsDurty { get; }
		void AddRepetitiveBilling(RepetitiveBilling repetitiveBilling);
		void RemoveRepetitiveBilling(int repetitiveBillingId);
		void UpdateRepetitiveBilling(RepetitiveBilling repetitiveBilling);
		void ShiftRepetitiveBilling(int repetitiveBillingId);
		List<RepetitiveBilling> GetRepetitiveBillings();
		RepetitiveBilling GetRepetitiveBilling(int repetitiveBillingId);
		void Save(XmlTextWriter writer);
		void Load(XmlDocument xmlDocument);
	}
}
