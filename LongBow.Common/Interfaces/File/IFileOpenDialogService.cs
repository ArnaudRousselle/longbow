using System;

namespace LongBow.Common.Interfaces.File
{
	public interface IFileOpenDialogService
	{
		void OpenFile(string filter, Action<FileOpenDialogServiceResult> callBackAction);
		void SaveFile(string filter, Action<FileOpenDialogServiceResult> callBackAction);
	}

	public class FileOpenDialogServiceResult
	{
		public bool Confirmed { get; internal set; }
		public string FileName { get; internal set; }
	}
}
