using System.ComponentModel.Composition;
using System.Windows.Controls;
using LongBow.Common.Contracts;
using Microsoft.Practices.Prism.Regions;

namespace LongBow.FileImport
{
	[Export(ViewNames.FileImportView), PartCreationPolicy(CreationPolicy.NonShared)]
	[ViewSortHint("3")]
	public partial class FileImportView : UserControl
	{
		[Import]
		public IFileImportViewModel ViewModel
		{
			get { return DataContext as IFileImportViewModel; }
			set { DataContext = value; }
		}

		public FileImportView()
		{
			InitializeComponent();
		}
	}
}
