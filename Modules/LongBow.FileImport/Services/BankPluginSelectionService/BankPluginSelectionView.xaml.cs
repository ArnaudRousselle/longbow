using System.Windows.Controls;

namespace LongBow.FileImport.Services.BankPluginSelectionService
{
	/// <summary>
	/// Logique d'interaction pour BankPluginSelectionView.xaml
	/// </summary>
	public partial class BankPluginSelectionView : UserControl
	{
		public BankPluginSelectionView()
		{
			DataContext = new BankPluginSelectionViewModel();
			InitializeComponent();
		}
	}
}
