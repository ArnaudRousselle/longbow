using System.Collections.Generic;
using System.Xml;
using LongBow.Dom;

namespace LongBow.Dal
{
	public interface IRepetitiveBillingRepository
	{
		void Add(RepetitiveBilling repetitiveBilling);
		void Remove(int repetitiveBillingId);
		void Update(RepetitiveBilling repetitiveBilling);
		List<RepetitiveBilling> GetAll();
		RepetitiveBilling Get(int repetitiveBillingId);
		void Save(XmlTextWriter writer);
		void Load(XmlDocument xmlDocument);
		bool IsDurty { get; }
	}
}
