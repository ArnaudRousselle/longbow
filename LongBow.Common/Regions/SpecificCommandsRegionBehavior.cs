using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LongBow.Controls.Menus;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using LongBow.Common.Contracts;

namespace LongBow.Common.Regions
{
	public class SpecificCommandsRegionBehavior : RegionBehavior
	{
		public const string Key = "SpecificCommandsRegionBehavior";

		private IRegionManager _regionManager;

		private IRegion _specificCommandsRegion;
		private IRegion SpecificCommandsRegion
		{
			get
			{
				return _specificCommandsRegion ??
					   (_specificCommandsRegion = _regionManager.Regions[RegionNames.SpecificCommandsRegion]);
			}
		}


		protected override void OnAttach()
		{
			_regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();

			Region.ActiveViews.CollectionChanged += ViewsOnCollectionChanged;
		}

		private void ViewsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					OnViewAddedToRegion(e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Remove:
					OnViewRemovedToRegion(e.OldItems[0]);
					break;
			}
		}

		protected virtual void OnViewAddedToRegion(object view)
		{
			DeleteViewsFromSpecificCommandsRegion();

			var frameworkElement = view as FrameworkElement;

			if (frameworkElement == null)
				return;

			var specificCommands = frameworkElement.DataContext as ISpecificCommands;

			if (specificCommands == null)
				return;

			foreach (var simpleCommand in specificCommands.Commands)
			{
				SpecificCommandsRegion.Add(new SmallLongBowMenuItem
				                           {
					                           Label = simpleCommand.Label,
					                           Command = simpleCommand.Command,
				                           });
			}
		}

		protected virtual void OnViewRemovedToRegion(object view)
		{
			if (!Region.Views.Any())
				DeleteViewsFromSpecificCommandsRegion();
		}

		private void DeleteViewsFromSpecificCommandsRegion()
		{
			foreach (var v in SpecificCommandsRegion.Views.ToArray())
			{
				SpecificCommandsRegion.Remove(v);
			}
		}
	}

	public interface ISpecificCommands
	{
		IEnumerable<SimpleCommand> Commands { get; }
	}

	public class SimpleCommand
	{
		public DelegateCommand Command { get; set; }
		public string Label { get; set; }
		public string IconResourceName { get; set; }
	}
}
