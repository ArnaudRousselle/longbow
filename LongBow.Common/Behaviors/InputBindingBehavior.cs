using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LongBow.Common.Behaviors
{
	public static class InputBindingBehavior
	{
		public static bool GetMoveInputBindingsInParentWindow(DependencyObject obj)
		{
			return (bool)obj.GetValue(MoveInputBindingsInParentWindowProperty);
		}

		public static void SetMoveInputBindingsInParentWindow(DependencyObject obj, bool value)
		{
			obj.SetValue(MoveInputBindingsInParentWindowProperty, value);
		}

		public static readonly DependencyProperty MoveInputBindingsInParentWindowProperty =
			DependencyProperty.RegisterAttached("MoveInputBindingsInParentWindow",
				typeof(bool),
				typeof(FrameworkElement),
				new PropertyMetadata(OnMoveInputBindingsInParentWindowChanged));

		private static void OnMoveInputBindingsInParentWindowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(sender))
				return;

			if (!(bool)e.NewValue)
				return;

			((FrameworkElement)sender).Loaded += FrameworkElementLoaded;
		}

		private static void FrameworkElementLoaded(object sender, RoutedEventArgs e)
		{
			var frameworkElement = (FrameworkElement)sender;

			frameworkElement.Loaded -= FrameworkElementLoaded;

			var window = Window.GetWindow(frameworkElement);

			if (window == null)
				throw new Exception("Window was not found");

			while (frameworkElement.InputBindings.Count > 0)
			{
				var inputBinding = frameworkElement.InputBindings[0];
				frameworkElement.InputBindings.RemoveAt(0);

				var keyBinding = inputBinding as KeyBinding;

                if (keyBinding != null)
                    window
                        .InputBindings
                        .OfType<KeyBinding>()
                        .Where(n => n.Key == keyBinding.Key
                                    && n.Modifiers == keyBinding.Modifiers)
                        .ToList()
                        .ForEach(n => window.InputBindings.Remove(n));

                window.InputBindings.Add(inputBinding);
			}
		}
	}
}
