using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using LongBow.Common.Menu;
using LongBow.Common.Contracts;
using LongBow.Controls.Menus;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Listing
{
	[ModuleExport(typeof (ListingModule))]
	public class ListingModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public ListingModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof (ListingModuleMenu));
		}
	}

	[Export]
	[ViewSortHint("C")]
	public class ListingModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public ListingModuleMenu(IRegionManager regionManager)
		{
			Label = "Liste";
			HotKeys = "F2";
			Command = MenuCommands.OpenListingCommand;
		}
	}
}
