using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace LongBow.Common.Interfaces.File.Implementation
{
	[Export(typeof(IFileOpenDialogService))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class FileOpenDialogService : IFileOpenDialogService
	{
		public void OpenFile(string filter, Action<FileOpenDialogServiceResult> callBackAction)
		{
			ChooseFile<OpenFileDialog>(filter, callBackAction);
		}

		public void SaveFile(string filter, Action<FileOpenDialogServiceResult> callBackAction)
		{
			ChooseFile<SaveFileDialog>(filter, callBackAction);
		}

		private void ChooseFile<T>(string filter, Action<FileOpenDialogServiceResult> callBackAction) where T : FileDialog, new()
		{
			if (callBackAction == null)
				throw new ArgumentNullException("callBackAction");

			var serviceResult = new FileOpenDialogServiceResult();

			using (var dialog = new T())
			{
				dialog.Filter = filter;

				var result = dialog.ShowDialog();

				if (result == DialogResult.OK)
				{
					serviceResult.Confirmed = true;
					serviceResult.FileName = dialog.FileName;
				}
				else
				{
					serviceResult.Confirmed = false;
					serviceResult.FileName = null;
				}
			}

			callBackAction(serviceResult);
		}
	}

}
