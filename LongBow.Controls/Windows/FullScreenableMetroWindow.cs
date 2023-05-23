using System.Windows;
using MahApps.Metro.Controls;

namespace LongBow.Controls.Windows
{
	public class FullScreenableMetroWindow : MetroWindow
	{
		#region IsFullScreenProperty

		public static readonly DependencyProperty IsFullScreenProperty =
			DependencyProperty.Register(
				"IsFullScreen",
				typeof(bool),
				typeof(FullScreenableMetroWindow),
				new PropertyMetadata(false, IsFullScreenPropertyChangedCallback));

		public bool IsFullScreen
		{
			get { return (bool)GetValue(IsFullScreenProperty); }
			set { SetValue(IsFullScreenProperty, value); }
		}

		private static void IsFullScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
		{
			var window = (FullScreenableMetroWindow) dependencyObject;

			if ((bool) args.NewValue)
			{
				window.IgnoreTaskbarOnMaximize = true;
				window.WindowState = WindowState.Maximized;
				window.ResizeMode = ResizeMode.NoResize;
				window.ShowCloseButton = false;
			}
			else
			{
				window.IgnoreTaskbarOnMaximize = false;
				window.WindowState = WindowState.Normal;
				window.ResizeMode = ResizeMode.CanResize;
				window.ShowCloseButton = true;
			}
		}

		#endregion
	}
}
