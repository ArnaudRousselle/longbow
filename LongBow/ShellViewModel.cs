using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using LongBow.Common.Contracts;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.PersistentState;
using LongBow.Common.Regions;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow
{
    //todo: essayer d'améliorer pour mieux gérer la navigation entre les régions Left et NewWindow (pour éviter les doublons) : s'inspirer de ce qui est fait dans Athena
    [Export(typeof(IShellViewModel))]
    public class ShellViewModel : BindableBase, IShellViewModel
    {
        private readonly IBusinessContext _businessContext;
        private readonly IPersistentStateManager _persistentStateManager;

        private DelegateCommand<object> _closingCommand;
        private DelegateCommand _toggleFullScreenModeCommand;
        private bool _fullScreen;
        private bool _isMenuMinimized;

        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager,
            IBusinessContext businessContext,
            IEventAggregator eventAggregator,
            IPersistentStateManager persistentStateManager)
        {
            _businessContext = businessContext;
            _persistentStateManager = persistentStateManager;

            NotificationRequest = new InteractionRequest<INotification>();

            regionManager.Regions.CollectionChanged += RegionsCollectionChanged;

            IRegion region = new AllActiveRegion();
            regionManager.Regions.Add(RegionNames.NewWindowRegion, region);

            region = new AllActiveRegion();
            regionManager.Regions.Add(RegionNames.NotificationWindowRegion, region);

            _persistentStateManager.LoadState();

            if (_persistentStateManager.LastOpenedFile != null)
                businessContext.Load(_persistentStateManager.LastOpenedFile, false);

            eventAggregator
                .GetEvent<LongBowNotificationEvent>()
                .Subscribe(args =>
                           {
                               NotificationRequest.Raise(new Notification
                               {
                                   Title = args.Title,
                                   Content = args.Content
                               });
                           });
        }

        private void RegionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            foreach (var region in e.NewItems.Cast<Region>())
            {
                if (region.Name == RegionNames.MainTabRegion
                    || region.Name == RegionNames.LeftDockRegion
                    || region.Name == RegionNames.RightDockRegion
                    || region.Name == RegionNames.NewWindowRegion)
                {
                    region.Behaviors.Add(ClosableAndSwitchableRegionBehavior.Key, new ClosableAndSwitchableRegionBehavior());
                    region.Behaviors.Add(NewFileLoadedAwareRegion.Key, new NewFileLoadedAwareRegion());
                }

                if (region.Name == RegionNames.NewWindowRegion)
                {
                    region.Behaviors.Add(NewWindowRegionBehavior.Key, new NewWindowRegionBehavior());
                }

                if (region.Name == RegionNames.NotificationWindowRegion)
                {
                    region.Behaviors.Add(NewWindowRegionBehavior.Key, new NotificationWindowRegionBehavior());
                }

                if (region.Name == RegionNames.MainTabRegion)
                {
                    region.Behaviors.Add(SpecificCommandsRegionBehavior.Key, new SpecificCommandsRegionBehavior());
                }

                region.Behaviors.Add(ViewRemovedAwareRegionBehavior.Key, new ViewRemovedAwareRegionBehavior());
            }
        }

        public DelegateCommand<object> ClosingCommand
        {
            get
            {
                return _closingCommand ?? (_closingCommand =
                    new DelegateCommand<object>(ClosingCommandMethod));
            }
        }

        private void ClosingCommandMethod(object arg)
        {
            var cancelEventArgs = arg as CancelEventArgs;

            if (cancelEventArgs == null)
                return;

            var canSaveState = true;

            if (_businessContext.IsDurty)
            {
                NotificationRequest.Raise(
                    new Confirmation
                    {
                        Title = "Attention",
                        Content = "Il y a des modifications en cours, voulez-vous vraiment quitter ?"
                    },
                    notification =>
                    {
                        var confirmation = (Confirmation)notification;
                        cancelEventArgs.Cancel = !confirmation.Confirmed;
                        canSaveState = confirmation.Confirmed;
                    });
            }

            if (canSaveState)
                _persistentStateManager.SaveState();
        }

        public DelegateCommand ToggleFullScreenModeCommand
        {
            get
            {
                return _toggleFullScreenModeCommand ??
                       (_toggleFullScreenModeCommand = new DelegateCommand(() =>
                       {
                           FullScreen = !FullScreen;
                       }));
            }
        }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        public bool FullScreen
        {
            get { return _fullScreen; }
            private set { SetProperty(ref _fullScreen, value); }
        }

        public bool IsMenuMinimized
        {
            get { return _isMenuMinimized; }
            set { SetProperty(ref _isMenuMinimized, value); }
        }
    }
}
