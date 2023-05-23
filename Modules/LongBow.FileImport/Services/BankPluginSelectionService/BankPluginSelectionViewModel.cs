using System;
using System.Collections.ObjectModel;
using System.Linq;
using LongBow.FileImport.Enums;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;

namespace LongBow.FileImport.Services.BankPluginSelectionService
{
	public class BankPluginSelectionViewModel : BindableBase, IInteractionRequestAware
	{
		private BankPluginSelectionNotification _notification;
		private BankPluginItem _selectedBankPlugin;
		private DelegateCommand _validateCommand;
		private DelegateCommand _cancelCommand;

		public BankPluginSelectionViewModel()
		{
			BankPlugins = new ObservableCollection<BankPluginItem>();
		}

		public Action FinishInteraction { get; set; }

		public INotification Notification
		{
			get
			{
				return _notification;
			}
			set
			{
				if (!(value is BankPluginSelectionNotification))
					return;

				// To keep the code simple, this is the only property where we are raising the PropertyChanged event,
				// as it's required to update the bindings when this property is populated.
				// Usually you would want to raise this event for other properties too.
				_notification = value as BankPluginSelectionNotification;
				OnPropertyChanged(() => Notification);
				
				BankPlugins.Clear();
				BankPlugins.AddRange(
					_notification
						.AvailablePlugins
						.Select(n => new BankPluginItem(n)));

				if (BankPlugins.Count > 0)
					SelectedBankPlugin = BankPlugins[0];
			}
		}

		public ObservableCollection<BankPluginItem> BankPlugins { get; set; }

		public BankPluginItem SelectedBankPlugin
		{
			get { return _selectedBankPlugin; }
			set { SetProperty(ref _selectedBankPlugin, value); }
		}

		public DelegateCommand ValidateCommand
		{
			get
			{
				return _validateCommand ??
					   (_validateCommand = new DelegateCommand(() =>
					   {
						   _notification.Confirmed = true;
						   _notification.SelectedBankPlugin = SelectedBankPlugin.Value;
						   FinishInteraction();
					   }));
			}
		}

		public DelegateCommand CancelCommand
		{
			get
			{
				return _cancelCommand ??
					   (_cancelCommand = new DelegateCommand(() =>
					   {
						   _notification.Confirmed = false;
						   _notification.SelectedBankPlugin = null;
						   FinishInteraction();
					   }));
			}
		}
	}

	public class BankPluginItem
	{
		public BankPluginItem(BankPlugin bankPlugin)
		{
			Text = bankPlugin.ToString();
			Value = bankPlugin;
		}

		public string Text { get; private set; }
		public BankPlugin Value { get; private set; }
	}
}
