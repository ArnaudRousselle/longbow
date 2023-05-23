using System.Collections.ObjectModel;
using LongBow.Common.Interfaces.File;

namespace LongBow.Common.PersistentState
{
	public interface IPersistentStateManager
	{
		string LastOpenedFile { get; set; }
		ObservableCollection<FileItem> RecentFiles { get; }
		void LoadState();
		void SaveState();
	}
}
