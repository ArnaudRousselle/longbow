using System.ComponentModel.Composition;
using System.Linq;
using LongBow.Dal.Utilities;
using LongBow.Dom;

namespace LongBow.Dal.Implementations
{
	[Export(typeof(IRepetitiveBillingRepository))]
	public class RepetitiveBillingRepository : SerializableObject<RepetitiveBilling>, IRepetitiveBillingRepository
	{
	}
}
