using System.Collections.Specialized;
using LongBow.Common.Contracts;
using LongBow.Common.Interfaces.Tabulation;
using Microsoft.Practices.Prism.Regions;
using System.Windows;
using Microsoft.Practices.ServiceLocation;

namespace LongBow.Common.Regions
{
	public class ClosableAndSwitchableRegionBehavior : RegionBehavior
	{
		public const string Key = "ClosableAndSwitchableRegionBehavior";

		private IRegionManager _regionManager;

		protected override void OnAttach()
		{
			_regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
			Region.Views.CollectionChanged += ViewsOnCollectionChanged;
		}

		private void ViewsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				OnViewAddedToRegion(e.NewItems[0]);
			}
		}

		private void OnViewAddedToRegion(object view)
		{
			var frameworkElement = view as FrameworkElement;

			if (frameworkElement == null)
				return;

			var iTab = frameworkElement.DataContext as IClosable;

			if (iTab != null)
			{
				iTab.Close = () =>
				                {
					                if (Region.Views.Contains(view))
						                Region.Remove(view);
				                };
			}

			var iSwitchableTab = frameworkElement.DataContext as ISwitchable;

			if (iSwitchableTab != null)
			{
				iSwitchableTab.Switch = () =>
				                           {
											   var oldRegion = _regionManager.Regions[RegionNames.LeftDockRegion];
											   var newRegion = _regionManager.Regions[RegionNames.NewWindowRegion];

					                           if (!oldRegion.Views.Contains(view))
					                           {
												   oldRegion = _regionManager.Regions[RegionNames.NewWindowRegion];
												   newRegion = _regionManager.Regions[RegionNames.LeftDockRegion];
					                           }

											   oldRegion.Remove(view);

											   newRegion.Add(view);
											   newRegion.Activate(view);
				                           };
			}
		}

	}
}
