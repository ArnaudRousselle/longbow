using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LongBow.Common.Contracts;
using LongBow.Common.Menu;
using LongBow.Controls.Menus;
using LongBow.FileImport.Interfaces;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.FileImport
{
	[ModuleExport(typeof(FileImportModule))]
	class FileImportModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public FileImportModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(FileImportModuleMenu));
		}
	}

	[Export]
	[ViewSortHint("F")]
	public class FileImportModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public FileImportModuleMenu(IFileReaderFactory fileReaderFactory)
		{
			Label = "Importer";
			HotKeys = "F4";
			Command = MenuCommands.OpenFileImportCommand;
		}
	}
}
