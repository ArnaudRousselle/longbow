using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LongBow.Common.Contracts;
using LongBow.Common.Menu;
using LongBow.Controls.Menus;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.CalendarListing
{
	[ModuleExport(typeof(CalendarListingModule))]
	public class CalendarListingModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public CalendarListingModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof (CalendarListingModuleMenu));
		}
	}

	[Export]
	[ViewSortHint("D")]
	public class CalendarListingModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public CalendarListingModuleMenu(IRegionManager regionManager)
		{
			Label = "Echéancier";
			HotKeys = "F3";
			Command = MenuCommands.OpenCalendarListingCommand;
		}
	}
}
