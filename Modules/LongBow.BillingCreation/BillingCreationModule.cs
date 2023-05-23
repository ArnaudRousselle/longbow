using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LongBow.Common.Menu;
using LongBow.Common.Contracts;
using LongBow.Controls.Menus;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.BillingCreation
{
	[ModuleExport(typeof(BillingCreationModule))]
	public class BillingCreationModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public BillingCreationModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(BillingCreationModuleMenu));
		}
	}

	[Export]
	[ViewSortHint("A")]
	public class BillingCreationModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public BillingCreationModuleMenu(IRegionManager regionManager)
		{
			Label = "Ajouter";
			HotKeys = "F1";
			Command = MenuCommands.NewBillingCommand;
		}
	}
}
