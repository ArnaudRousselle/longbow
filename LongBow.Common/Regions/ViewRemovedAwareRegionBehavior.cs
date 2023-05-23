using System.Collections.Specialized;
using System.Windows;
using LongBow.Common.Interfaces.Views;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Common.Regions
{
    public class ViewRemovedAwareRegionBehavior : RegionBehavior
    {
        public const string Key = "ViewRemovedAwareRegionBehavior";

        protected override void OnAttach()
        {
            Region.Views.CollectionChanged += ViewsOnCollectionChanged;
        }

        private void ViewsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    OnViewRemovedToRegion(e.OldItems[0]);
                    break;
            }
        }

        protected virtual void OnViewRemovedToRegion(object view)
        {
            var frameworkElement = view as FrameworkElement;
            var viewRemovedAware = frameworkElement?.DataContext as IViewRemovedAware;
            viewRemovedAware?.OnViewRemovedFromRegion();
        }
    }
}
