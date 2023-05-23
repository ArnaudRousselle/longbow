using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Xml;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.File;
using LongBow.Common.Interfaces.Notifications;
using LongBow.Common.PersistentState;
using Microsoft.Practices.Prism.PubSubEvents;

namespace LongBow.Bll
{
	[Export(typeof(IBusinessContext))]
	public class BusinessContext : IBusinessContext
	{
		[Import] private INotificationService _notificationService;
		[Import] private IEventAggregator _eventAggregator;
		[Import] private IFileInfo _fileInfo;
		[Import] private IPersistentStateManager _persistentStateManager;

		[Import]
		public IBillingManager BillingManager { get; private set; }

		[Import]
		public IBillingCalculationManager BillingCalculationManager { get; private set; }

		[Import]
		public IRepetitiveBillingManager RepetitiveBillingManager { get; private set; }

		public void Save()
		{
			if(!_fileInfo.Exists(_persistentStateManager.LastOpenedFile))
				return;

			using (var writer = new XmlTextWriter(_persistentStateManager.LastOpenedFile, Encoding.UTF8))
			{
				writer.Formatting = Formatting.Indented;
				writer.Indentation = 4;

				writer.WriteStartElement("DocumentElement");

				BillingManager.Save(writer);
				RepetitiveBillingManager.Save(writer);

				writer.WriteEndElement();
			}

			_notificationService
				.PrintNotification("Sauvegarde effectuée",
					"La sauvegarde des données est terminée");
		}

		public void SaveAs(string filePath)
		{
			_persistentStateManager.LastOpenedFile = filePath;
			Save();
		}

		public void Load(string filePath, bool keepEntry)
		{
			if (!_fileInfo.Exists(filePath))
				return;

			var fileInfo = new FileInfo(filePath);

			if (keepEntry)
				_persistentStateManager
					.RecentFiles
					.Add(new FileItem
					     {
						     ShortName = fileInfo.Name,
						     FilePath = fileInfo.FullName,
					     });

			_persistentStateManager.LastOpenedFile = filePath;

			_eventAggregator
				.GetEvent<NewFileLoadedEvent>()
				.Publish(fileInfo);

			var xmlDocument = new XmlDocument();

			xmlDocument.Load(_persistentStateManager.LastOpenedFile);

			BillingManager.Load(xmlDocument);
			RepetitiveBillingManager.Load(xmlDocument);

			_notificationService
				.PrintNotification("Chargement terminé",
					"La chargement des données est terminé");
		}

		public bool IsDurty
		{
			get
			{
				return BillingManager.IsDurty
					|| RepetitiveBillingManager.IsDurty;
			}
		}
	}
}
