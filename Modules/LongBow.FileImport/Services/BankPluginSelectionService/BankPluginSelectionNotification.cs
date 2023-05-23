using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using LongBow.FileImport.Enums;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace LongBow.FileImport.Services.BankPluginSelectionService
{
	public class BankPluginSelectionNotification : Confirmation
	{
		public BankPluginSelectionNotification(IEnumerable<BankPlugin> availableBankPlugins)
		{
			AvailablePlugins = new ReadOnlyCollection<BankPlugin>(availableBankPlugins.ToList());
		}

		public ReadOnlyCollection<BankPlugin> AvailablePlugins { get; private set; }
		public BankPlugin? SelectedBankPlugin { get; set; }
	}
}
