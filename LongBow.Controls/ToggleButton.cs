using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LongBow.Controls
{
	public class ToggleButton : Image
	{
		private ImageSource _defaultImageSource;

		public ToggleButton()
		{
			AddHandler(LoadedEvent, new RoutedEventHandler(OnLoadedEvent));
			AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler(OnClickEvent));
		}

		private void OnLoadedEvent(object sender, RoutedEventArgs e)
		{
			_defaultImageSource = Source;
		}

		private void OnClickEvent(object sender, RoutedEventArgs e)
		{
			BoundStateProperty = !BoundStateProperty;
		}

		private void SynchronizeState()
		{
			Source = BoundStateProperty ? SecondSource : _defaultImageSource;
		}

		#region SecondSourceProperty

		public static readonly DependencyProperty SecondSourceProperty =
			DependencyProperty.Register(
				"SecondSource",
				typeof(ImageSource),
				typeof(ToggleButton),
				new PropertyMetadata(null));

		public ImageSource SecondSource
		{
			get { return (ImageSource)GetValue(SecondSourceProperty); }
			set { SetValue(SecondSourceProperty, value); }
		}

		#endregion

		#region BoundStatePropertyProperty

		public static readonly DependencyProperty BoundStatePropertyProperty =
			DependencyProperty.Register(
				"BoundStateProperty",
				typeof (bool),
				typeof (ToggleButton),
				new FrameworkPropertyMetadata(false, BoundStatePropertyPropertyChanged) {BindsTwoWayByDefault = true});

		public bool BoundStateProperty
		{
			get { return (bool)GetValue(BoundStatePropertyProperty); }
			set { SetValue(BoundStatePropertyProperty, value); }
		}

		private static void BoundStatePropertyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((ToggleButton)dependencyObject).SynchronizeState();
		}

		#endregion
	}
}
