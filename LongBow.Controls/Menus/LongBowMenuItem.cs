using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LongBow.Controls.Menus
{
	public class LongBowMenuItem : ContentControl
	{
		static LongBowMenuItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LongBowMenuItem), new FrameworkPropertyMetadata(typeof(LongBowMenuItem)));
			StyleProperty.OverrideMetadata(typeof(LongBowMenuItem), new FrameworkPropertyMetadata(GetDefautlStyle()));
		}

		public LongBowMenuItem()
		{
			AddHandler(MouseLeftButtonDownEvent,
				new RoutedEventHandler((sender, args) =>
				                       {
					                       var menuItem = sender as LongBowMenuItem;

										   if (menuItem == null || menuItem.Command == null)
						                       return;

										   menuItem.Command.Execute(null);
				                       }));
		}

		private static Style _defaultStyle;
		private static Style GetDefautlStyle()
		{
			return _defaultStyle ??
				   (_defaultStyle = Application.Current.FindResource(typeof(LongBowMenuItem)) as Style);
		}

		#region Label

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty =
			DependencyProperty.Register("Label", typeof(string), typeof(LongBowMenuItem));

		#endregion

		#region HotKeys

		public string HotKeys
		{
			get { return (string)GetValue(HotKeysProperty); }
			set { SetValue(HotKeysProperty, value); }
		}

		public static readonly DependencyProperty HotKeysProperty =
			DependencyProperty.Register("HotKeys", typeof(string), typeof(LongBowMenuItem));

		#endregion

		#region Command

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(LongBowMenuItem));

		#endregion
	}
}
