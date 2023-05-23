using System.Globalization;
using System.Windows;

namespace LongBow
{
	/// <summary>
	/// Logique d'interaction pour App.xaml
	/// </summary>
	public partial class App
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = CultureInfo.InvariantCulture;

			// The boostrapper will create the Shell instance, so the App.xaml does not have a StartupUri.
			var bootstrapper = new Bootstrapper();
			bootstrapper.Run();
		}
	}
}
