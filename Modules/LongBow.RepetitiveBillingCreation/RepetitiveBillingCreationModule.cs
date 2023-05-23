using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace LongBow.RepetitiveBillingCreation
{
	[ModuleExport(typeof(RepetitiveBillingCreationModule))]
	public class RepetitiveBillingCreationModule : IModule
	{
		public void Initialize()
		{
			
		}
	}
}
