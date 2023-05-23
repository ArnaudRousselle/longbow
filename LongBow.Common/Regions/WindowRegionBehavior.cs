using System.Collections.Specialized;
using System.Windows;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Common.Regions
{
	public abstract class WindowRegionBehavior : RegionBehavior
	{
		protected override void OnAttach()
		{
            Region.Views.CollectionChanged += ViewsOnCollectionChanged;
		}

		private void ViewsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					OnViewAddedToRegion(e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Remove:
					OnViewRemovedFromRegion(e.OldItems[0]);
					break;
			}
		}

		protected virtual void OnViewAddedToRegion(object view)
		{
			var frameworkElement = view as FrameworkElement;

			if (frameworkElement == null)
				return;

			var window = GetNewWindow();

			window.Content = view;

			window.Closed += (sender, args) =>
			{
				var contentView = ((Window)sender).Content;

				if (contentView != null)
				{
					// si on entre dans ce IF, c'est que la fenêtre a été fermée sans passer par l'action Close du ViewModel
					// sinon, c'est que l'action Close du ViewModel (qui implémente ITab) a été appelée et dans ce cas,
					// la vue a déjà été supprimée de la région
					Region.Remove(contentView);
				}
			};

			Shortcut.Shortcut.AddShortcuts(window.InputBindings);

			WindowShowing(window, frameworkElement.DataContext);

			window.Show();

			WindowShowed(window, frameworkElement.DataContext);
		}

		protected virtual void OnViewRemovedFromRegion(object view)
		{
			var frameworkElement = view as FrameworkElement;

			if (frameworkElement == null)
				return;

			var window = frameworkElement.Parent as Window;

			if (window == null)
				return;

			window.Content = null;

			window.Close();
		}

		protected abstract Window GetNewWindow();

		protected abstract void WindowShowing(Window sender, object dataContext);

		protected abstract void WindowShowed(Window sender, object dataContext);
	}
}
