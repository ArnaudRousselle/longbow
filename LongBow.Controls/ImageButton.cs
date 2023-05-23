using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LongBow.Controls
{
	public class ImageButton : Image
	{
		public ImageButton()
		{
			AddHandler(MouseDownEvent, new RoutedEventHandler(OnMouseDownEvent));
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(ImageButton), new PropertyMetadata());

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageButton), new PropertyMetadata());

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		private void OnMouseDownEvent(object sender, RoutedEventArgs routedEventArgs)
		{
			if (Command != null)
				Command.Execute(CommandParameter);
		}

	}
}
