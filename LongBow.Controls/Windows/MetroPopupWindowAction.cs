using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.Controls.Windows
{
	public class MetroPopupWindowAction : PopupWindowAction
	{
		protected override Window GetWindow(INotification notification)
		{
			Window wrapperWindow;

			if (WindowContent != null)
			{
				wrapperWindow = new MetroWindow
				                {
					                DataContext = notification,
					                Title = notification.Title,
					                Style = Application.Current.FindResource("MyMetroWindowStyle") as Style
				                };

				PrepareContentForWindow(notification, wrapperWindow);
			}
			else
			{
				if (notification is IConfirmation)
				{
					wrapperWindow = new DefaultMetroConfirmationWindow { Confirmation = (IConfirmation)notification };
				}
				else
				{
					wrapperWindow = new DefaultMetroNotificationWindow { Notification = notification };
				}
			}

			wrapperWindow.ResizeMode = ResizeMode.NoResize;

			return wrapperWindow;
		}

		
	}
}
