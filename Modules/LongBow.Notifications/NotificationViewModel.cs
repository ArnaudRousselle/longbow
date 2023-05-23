using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;

namespace LongBow.Notifications
{
	[Export(typeof (INotificationViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class NotificationViewModel : INotificationViewModel
	{
		~NotificationViewModel()
		{
			_loggerFacade.Log("NotificationViewModel garbage collected", Category.Debug, Priority.Low);
		}

		private readonly ILoggerFacade _loggerFacade;

		[ImportingConstructor]
		public NotificationViewModel(ILoggerFacade loggerFacade)
		{
			_loggerFacade = loggerFacade;
		}

		public string Header { get; set; }
		public string Content { get; set; }
	}
}
