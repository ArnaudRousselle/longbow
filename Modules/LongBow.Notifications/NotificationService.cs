using System.ComponentModel.Composition;
using LongBow.Common.Contracts;
using LongBow.Common.Interfaces.Notifications;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace LongBow.Notifications
{
	[Export(typeof(INotificationService))]
	public class NotificationService : INotificationService
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public NotificationService(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void PrintNotification(string header, string content)
		{
			var viewModel = ServiceLocator.Current.GetInstance<INotificationViewModel>();
			var view = new NotificationView();

			viewModel.Header = header;
			viewModel.Content = content;

			view.DataContext = viewModel;

			_regionManager.Regions[RegionNames.NotificationWindowRegion].Add(view);
		}
	}
}
