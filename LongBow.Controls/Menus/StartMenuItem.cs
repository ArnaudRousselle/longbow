using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace LongBow.Controls.Menus
{
	public class StartMenuItem : ContentControl
	{
		static StartMenuItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(StartMenuItem), new FrameworkPropertyMetadata(typeof(StartMenuItem)));
			StyleProperty.OverrideMetadata(typeof(StartMenuItem), new FrameworkPropertyMetadata(GetDefautlStyle()));
		}

		public StartMenuItem()
		{
			AddHandler(MouseLeftButtonDownEvent,
				new RoutedEventHandler((sender, args) =>
				                       {
										   var menuItem = sender as StartMenuItem;

										   if (menuItem == null || menuItem.ContextMenu == null)
						                       return;

					                       menuItem.ContextMenu.PlacementTarget = menuItem;
										   menuItem.ContextMenu.Placement = PlacementMode.Bottom;
					                       menuItem.ContextMenu.IsOpen = true;
				                       }));
		}

		private static Style _defaultStyle;
		private static Style GetDefautlStyle()
		{
			return _defaultStyle ??
				   (_defaultStyle = Application.Current.FindResource(typeof(StartMenuItem)) as Style);
		}

		#region Label

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(string), typeof(StartMenuItem));

		#endregion
	}
}
