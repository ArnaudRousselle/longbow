using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LongBow.Common.Contracts;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.File;
using LongBow.Common.Menu;
using LongBow.Common.PersistentState;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.Save
{
	[ModuleExport(typeof(SaveModule))]
	public class SaveModule : IModule
	{
		private readonly IRegionManager _regionManager;

		[ImportingConstructor]
		public SaveModule(IRegionManager regionManager)
		{
			_regionManager = regionManager;
		}

		public void Initialize()
		{
			_regionManager.RegisterViewWithRegion(RegionNames.RibbonApplicationMenuRegion, typeof(OpenMenuItem));
            _regionManager.RegisterViewWithRegion(RegionNames.RibbonApplicationMenuRegion, typeof(RecentFilesMenuItem));
            _regionManager.RegisterViewWithRegion(RegionNames.RibbonApplicationMenuRegion, typeof(SaveMenuItem));
			_regionManager.RegisterViewWithRegion(RegionNames.RibbonApplicationMenuRegion, typeof(SaveAsMenuItem));
            
            _regionManager.RegisterViewWithRegion(RegionNames.LeftTitleBarRegion, typeof(OpenImageMenuItem));
            _regionManager.RegisterViewWithRegion(RegionNames.LeftTitleBarRegion, typeof(SaveImageMenuItem));
        }
	}

	[Export]
	[ViewSortHint("z1")]
	public class OpenMenuItem : MenuItem
	{
		public OpenMenuItem()
		{
			Header = "Ouvrir";
			Icon = new Image { Source = Application.Current.FindResource("OpenFolderSource") as ImageSource };
			Command = MenuCommands.OpenCommand;
		}
	}

    [Export]
    [ViewSortHint("z2")]
    public class RecentFilesMenuItem : MenuItem
    {
        private readonly IPersistentStateManager _persistentStateManager;
        private readonly IFileInfo _fileInfo;
        private readonly IBusinessContext _businessContext;
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public RecentFilesMenuItem(IPersistentStateManager persistentStateManager,
            IFileInfo fileInfo,
            IBusinessContext businessContext, IEventAggregator eventAggregator)
        {
            _persistentStateManager = persistentStateManager;
            _fileInfo = fileInfo;
            _businessContext = businessContext;
            _eventAggregator = eventAggregator;

            Header = "Fichiers récents";
            Icon = new Image { Source = Application.Current.FindResource("OpenFolderSource") as ImageSource };

            _persistentStateManager.RecentFiles.CollectionChanged +=
                (sender, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (var newItem in args.NewItems.OfType<FileItem>())
                            {
                                AddNewFileItem(newItem);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (var oldItem in args.OldItems.OfType<FileItem>())
                            {
                                foreach (var menuItem in Items.OfType<MenuItem>())
                                {
                                    if (menuItem.Tag != oldItem)
                                        continue;

                                    Items.Remove(menuItem);
                                    return;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            Items.Clear();
                            break;
                    }
                };

            foreach (var newItem in _persistentStateManager.RecentFiles)
            {
                AddNewFileItem(newItem);
            }
        }

        private void AddNewFileItem(FileItem fileItem)
        {
            Items.Add(new MenuItem
                      {
                          Header = fileItem.CompleteName,
                          Tag = fileItem,
                          Command = new DelegateCommand<FileItem>(
                              fi =>
                              {
                                  if (!_fileInfo.Exists(fileItem.FilePath))
                                  {
                                      _persistentStateManager.RecentFiles.Remove(fileItem);

                                      _eventAggregator
                                          .GetEvent<LongBowNotificationEvent>()
                                          .Publish(new LongBowNotificationEventArgs
                                                   {
                                                       Title = "Fichier introuvable",
                                                       Content = "Le fichier spécifié est introuvable"
                                                   });
                                      return;
                                  }

                                  _businessContext.Load(fileItem.FilePath, false);
                              }),
                          CommandParameter = fileItem
                      });
        }
    }

    [Export]
	[ViewSortHint("z3")]
	public class SaveMenuItem : MenuItem
	{
		public SaveMenuItem()
		{
			Header = "Sauvegarder";
			Icon = new Image { Source = Application.Current.FindResource("SaveSource") as ImageSource };
			Command = MenuCommands.SaveCommand;
		}
	}

	[Export]
	[ViewSortHint("z4")]
	public class SaveAsMenuItem : MenuItem
	{
		public SaveAsMenuItem()
		{
			Header = "Sauvegarder sous";
			Icon = new Image { Source = Application.Current.FindResource("SaveAsSource") as ImageSource };
			Command = MenuCommands.SaveAsCommand;
		}
	}

    [Export]
    [ViewSortHint("y1")]
    public class OpenImageMenuItem : Button
    {
        public OpenImageMenuItem()
        {
            Tag = new VisualBrush(Application.Current.TryFindResource("appbar_folder_open") as Canvas);
            Command = MenuCommands.OpenCommand;
        }
    }

    [Export]
    [ViewSortHint("y2")]
    public class SaveImageMenuItem : Button
    {
        public SaveImageMenuItem()
        {
            Tag = new VisualBrush(Application.Current.TryFindResource("appbar_save") as Canvas);
            Command = MenuCommands.SaveCommand;
        }
    }
}
