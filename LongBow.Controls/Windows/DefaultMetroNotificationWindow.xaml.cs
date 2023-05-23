using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.Controls.Windows
{
	/// <summary>
	/// Logique d'interaction pour DefaultMetroNotificationWindow.xaml
	/// </summary>
	public partial class DefaultMetroNotificationWindow
	{
		public DefaultMetroNotificationWindow()
		{
			InitializeComponent();
		}

		public INotification Notification
		{
			get
			{
				return DataContext as INotification;
			}
			set
			{
				DataContext = value;
			}
		}

		private void OKButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
