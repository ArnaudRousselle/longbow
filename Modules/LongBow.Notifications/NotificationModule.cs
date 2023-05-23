using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;

namespace LongBow.Notifications
{
	[ModuleExport(typeof(NotificationModule))]
	public class NotificationModule : IModule
	{
		public void Initialize()
		{
			
		}
	}
}
