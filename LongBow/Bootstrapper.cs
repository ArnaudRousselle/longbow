using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using LongBow.Common.Logger;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;

namespace LongBow
{
	public class Bootstrapper : MefBootstrapper
	{
		protected override ILoggerFacade CreateLogger()
		{
			return new LongBowLogger();
		}

		//first
		protected override void ConfigureAggregateCatalog()
		{
			base.ConfigureAggregateCatalog();
			AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
			AggregateCatalog.Catalogs.Add(new DirectoryCatalog("./Modules"));
		}

		//second
		protected override DependencyObject CreateShell()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
			XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

			return Container.GetExportedValue<Shell>();
		}

		//third
		protected override void InitializeShell()
		{
			Application.Current.MainWindow = (Shell)Shell;
			Application.Current.MainWindow.Show();
		}
    }

}
