using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LongBow.Common.Behaviors
{
	public class ListViewWithScrollingBehavior : Behavior<ListView>
	{
		protected override void OnAttached()
		{
			AssociatedObject.SelectionChanged += OnSelectionChanged;
		}

		protected override void OnDetaching()
		{
			AssociatedObject.SelectionChanged -= OnSelectionChanged;
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var listView = sender as ListView;

			if (listView != null
			    && e.AddedItems != null
			    && e.AddedItems.Count > 0)
				listView.ScrollIntoView(e.AddedItems[0]);
		}
	}
}
