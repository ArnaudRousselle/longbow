using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using LongBow.Common.Interfaces.File;
using Microsoft.Practices.Prism.Mvvm;

namespace LongBow.Common.PersistentState.Implementation
{
	[Export(typeof (IPersistentStateManager))]
	public class PersistentStateManager : BindableBase, IPersistentStateManager
	{
		private readonly string _fileName;

		private readonly IFileInfo _fileInfo;

		private string _lastOpenedFile;

		[ImportingConstructor]
		public PersistentStateManager(IFileInfo fileInfo, IExecutionContext executionContext)
		{
			_fileInfo = fileInfo;
			_fileName = executionContext.CurrentExecutionPath + "\\config.xml";

			RecentFiles = new ObservableCollection<FileItem>();
		}

		public string LastOpenedFile
		{
			get { return _lastOpenedFile; }
			set { SetProperty(ref _lastOpenedFile, value); }
		}

		public ObservableCollection<FileItem> RecentFiles { get; private set; }

		public void LoadState()
		{
			if (!_fileInfo.Exists(_fileName))
				return;

			PersistentState persistentState;

			var serializer = new XmlSerializer(typeof(PersistentState));

			using (var reader = new StreamReader(_fileName))
			{
				persistentState = serializer.Deserialize(reader) as PersistentState;
			}

			if (persistentState == null)
				return;

			LastOpenedFile = persistentState.LastOpenedFile;
			RecentFiles = new ObservableCollection<FileItem>(persistentState.RecentFiles);
		}

		public void SaveState()
		{
			var persistentState = new PersistentState
			                      {
				                      LastOpenedFile = LastOpenedFile,
				                      RecentFiles = RecentFiles.ToList(),
			                      };

			var serializer = new XmlSerializer(typeof(PersistentState));

			using (var writer = new StreamWriter(_fileName))
			{
				serializer.Serialize(writer, persistentState);
			}
		}
	}

	public class PersistentState
	{
		public string LastOpenedFile { get; set; }
		public List<FileItem> RecentFiles { get; set; }
	}

}
