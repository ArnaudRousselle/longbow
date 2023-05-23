using LongBow.Common.EventMessages;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace LongBow.Common.Regions
{
	public class NewFileLoadedAwareRegion : RegionBehavior
	{
		public const string Key = "NewFileLoadedAwareRegion";

		protected override void OnAttach()
		{
			var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

			eventAggregator
				.GetEvent<NewFileLoadedEvent>()
				.Subscribe(fileInfo =>
				           {
					           foreach (var view in Region.Views)
					           {
						           Region.Remove(view);
					           }
				           });
		}
	}
}
