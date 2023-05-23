using System.Windows;
using LongBow.Common.Interfaces.Tabulation;
using MahApps.Metro.Controls;

namespace LongBow.Common.Regions
{
	public class NewWindowRegionBehavior : WindowRegionBehavior
	{
		public const string Key = "NewWindowRegionBehavior";

		protected override Window GetNewWindow()
		{
			return new MetroWindow
			       {
				       Style = Application.Current.FindResource("MyMetroWindowStyle") as Style
			       };
		}

		protected override void WindowShowing(Window sender, object dataContext)
		{
			var iTab = dataContext as ITab;

			var windowName = iTab != null ? iTab.HeaderTab : "Nouvelle fenêtre";

			sender.Title = windowName;
			sender.SizeToContent = SizeToContent.WidthAndHeight;
			sender.MinWidth = 350;
		}

		protected override void WindowShowed(Window sender, object dataContext)
		{
			sender.Focus();
		}

	}
}
