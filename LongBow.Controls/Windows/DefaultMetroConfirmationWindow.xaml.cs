using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.Controls.Windows
{
	/// <summary>
	/// Logique d'interaction pour DefaultMetroConfirmationWindow.xaml
	/// </summary>
	public partial class DefaultMetroConfirmationWindow
	{
		public DefaultMetroConfirmationWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Sets or gets the <see cref="IConfirmation"/> shown by this window./>
		/// </summary>
		public IConfirmation Confirmation
		{
			get
			{
				return DataContext as IConfirmation;
			}
			set
			{
				DataContext = value;
			}
		}

		private void OkButton_OnClick(object sender, RoutedEventArgs e)
		{
			Confirmation.Confirmed = true;
			Close();
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Confirmation.Confirmed = false;
			Close();
		}
	}
}
