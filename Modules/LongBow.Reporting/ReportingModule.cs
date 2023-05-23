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

namespace LongBow.Reporting
{
	[ModuleExport(typeof(ReportingModule))]
	public class ReportingModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public ReportingModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(ReportingModuleMenu));
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(LineChartModuleMenu));
			_regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(CalculatorMenu));
		}
	}

	[Export]
	[ViewSortHint("G")]
	public class ReportingModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public ReportingModuleMenu(IRegionManager regionManager)
		{
			Label = "Résultats";
			HotKeys = "F5";
			Command = MenuCommands.OpenReportingCommand;
		}
	}

	[Export]
	[ViewSortHint("H")]
	public class LineChartModuleMenu : LongBowMenuItem
	{
		[ImportingConstructor]
		public LineChartModuleMenu(IRegionManager regionManager)
		{
			Label = "Graphique";
			HotKeys = "F6";
			Command = MenuCommands.OpenLineChartCommand;
		}
	}

	[Export]
	[ViewSortHint("Z")]
	public class CalculatorMenu : LongBowMenuItem
	{
		public CalculatorMenu()
		{
			Label = "Calculatrice";
			HotKeys = "F7";
			Command = MenuCommands.OpenCalculator;
		}
	}
}
