using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LongBow.Common.Contracts;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.File;
using LongBow.Common.PersistentState;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

namespace LongBow.Common.Menu
{
	public static class MenuCommands
	{
		private static readonly IRegionManager RegionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
		private static readonly IBusinessContext BusinessContext = ServiceLocator.Current.GetInstance<IBusinessContext>();
		private static readonly IFileOpenDialogService FileOpenDialogService = ServiceLocator.Current.GetInstance<IFileOpenDialogService>();
		private static readonly IPersistentStateManager PersistentStateManager = ServiceLocator.Current.GetInstance<IPersistentStateManager>();
		private static readonly IFileInfo FileInfo = ServiceLocator.Current.GetInstance<IFileInfo>();

		private static DelegateCommand _newBillingCommand;
		private static DelegateCommand _openListingCommand;
		private static DelegateCommand _openCalendarListingCommand;
		private static DelegateCommand _openReportingCommand;
		private static DelegateCommand _openCalculator;
		private static DelegateCommand _openFileImportCommand;
		private static DelegateCommand _openCommand;
		private static DelegateCommand _saveCommand;
		private static DelegateCommand _saveAsCommand;
		private static DelegateCommand _openLineChartCommand;
	    private static DelegateCommand _openMailBoxCommand;

	    public static DelegateCommand NewBillingCommand
		{
			get
			{
				return _newBillingCommand ?? (_newBillingCommand =
					new DelegateCommand(() =>
					                    {
						                    var uri = new Uri(ViewNames.EditingView,
							                    UriKind.Relative);
											RegionManager.RequestNavigate(
							                    RegionNames.LeftDockRegion, uri);
					                    }));
			}
		}

        public static DelegateCommand OpenListingCommand
        {
            get
            {
                return _openListingCommand ?? (_openListingCommand =
                    new DelegateCommand(() =>
                    {
                        var uri = new Uri(ViewNames.ListingView, UriKind.Relative);
                        RegionManager.RequestNavigate(RegionNames.MainTabRegion, uri);
                    }));
            }
        }

        public static DelegateCommand OpenCalendarListingCommand
		{
			get
			{
				return _openCalendarListingCommand ?? (_openCalendarListingCommand =
					new DelegateCommand(() =>
					{
						var uri = new Uri(ViewNames.CalendarListingView, UriKind.Relative);
						RegionManager.RequestNavigate(RegionNames.MainTabRegion, uri);
					}));
			}
		}

		public static DelegateCommand OpenReportingCommand
		{
			get
			{
				return _openReportingCommand ?? (_openReportingCommand =
					new DelegateCommand(() =>
					{
						object view;

						if ((view = RegionManager
							.Regions[RegionNames.RightDockRegion]
							.Views
							.FirstOrDefault()) != null)
						{
							RegionManager
								.Regions[RegionNames.RightDockRegion]
								.Remove(view);
						}
						else
						{
							var uri = new Uri(ViewNames.ReportingView, UriKind.Relative);
							RegionManager.RequestNavigate(RegionNames.RightDockRegion, uri);
						}
					}));
			}
		}

		public static DelegateCommand OpenLineChartCommand
		{
			get
			{
				return _openLineChartCommand ?? (_openLineChartCommand =
					new DelegateCommand(() =>
					{
						var uri = new Uri(ViewNames.LineChartView, UriKind.Relative);
						RegionManager.RequestNavigate(RegionNames.MainTabRegion, uri);
					}));
			}
		}

		public static DelegateCommand OpenCalculator
		{
			get
			{
				return _openCalculator ?? (_openCalculator =
					new DelegateCommand(() => System.Diagnostics.Process.Start("calc.exe")));
			}
		}

		public static DelegateCommand OpenFileImportCommand
		{
			get
			{
				return _openFileImportCommand ?? (_openFileImportCommand =
					new DelegateCommand(() =>
					{
						var uri = new Uri(ViewNames.FileImportView, UriKind.Relative);
						RegionManager.RequestNavigate(RegionNames.MainTabRegion, uri);
					}));
			}
		}

		public static DelegateCommand OpenCommand
		{
			get
			{
				return _openCommand ?? (_openCommand =
					new DelegateCommand(() => FileOpenDialogService.OpenFile(
						"Fichiers LongBow (*.lbw)|*.lbw",
						result =>
						{
							if (!result.Confirmed)
								return;

							BusinessContext.Load(result.FileName, true);
						})));
			}
		}

		public static DelegateCommand SaveCommand
		{
			get
			{
				return _saveCommand ?? (_saveCommand =
					new DelegateCommand(() =>
					                    {
						                    if (string.IsNullOrWhiteSpace(PersistentStateManager.LastOpenedFile)
						                        || !FileInfo.Exists(PersistentStateManager.LastOpenedFile))
							                    SaveAsCommand.Execute();
						                    else
							                    BusinessContext.Save();
					                    }));
			}
		}

		public static DelegateCommand SaveAsCommand
		{
			get
			{
				return _saveAsCommand ?? (_saveAsCommand =
					new DelegateCommand(() => FileOpenDialogService.SaveFile(
						"Fichiers LongBow (*.lbw)|*.lbw",
						result =>
						{
							if (!result.Confirmed)
								return;

							BusinessContext.SaveAs(result.FileName);
						})));
			}
		}
	}
}
