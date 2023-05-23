using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml;
using LongBow.Common.EventMessages;
using LongBow.Common.Interfaces.Bll;
using LongBow.Common.Interfaces.Notifications;
using LongBow.Dal;
using LongBow.Dal.Utilities;
using LongBow.Dom;
using LongBow.Dom.Constants;
using Microsoft.Practices.Prism.PubSubEvents;

namespace LongBow.Bll
{
	[Export(typeof(IRepetitiveBillingManager))]
	public class RepetitiveBillingManager : IRepetitiveBillingManager
	{
		[Import]
		private IRepetitiveBillingRepository _repetitiveBillingRepository;
		[Import]
		private IEventAggregator _eventAggregator;
		[Import]
		private INotificationService _notificationService;

		public bool IsDurty
		{
			get { return _repetitiveBillingRepository.IsDurty; }
		}

		public void AddRepetitiveBilling(RepetitiveBilling repetitiveBilling)
		{
			_repetitiveBillingRepository.Add(repetitiveBilling);

			_eventAggregator
				.GetEvent<AddedRepetitiveBillingEvent>()
				.Publish(new AddedRepetitiveBillingEventArgs
				         {
							 AddedRepetitiveBilling = repetitiveBilling.Clone()
				         });

			_notificationService
				.PrintNotification("Ajout d'une ligne dans l'échéancier",
					string.Format("La ligne d'identifiant {0} a été ajoutée",
						repetitiveBilling.Id));
		}

		public void RemoveRepetitiveBilling(int repetitiveBillingId)
		{
			_repetitiveBillingRepository.Remove(repetitiveBillingId);

			_eventAggregator
				.GetEvent<DeletedRepetitiveBillingEvent>()
				.Publish(new DeletedRepetitiveBillingEventArgs
				         {
					         RepetitiveBillingId = repetitiveBillingId
				         });

			_notificationService
				.PrintNotification("Suppression d'une ligne dans l'échéancier",
					string.Format("La ligne d'identifiant {0} a été supprimée",
						repetitiveBillingId));
		}

		public void UpdateRepetitiveBilling(RepetitiveBilling repetitiveBilling)
		{
			_repetitiveBillingRepository.Update(repetitiveBilling);

			_eventAggregator
				.GetEvent<UpdatedRepetitiveBillingEvent>()
				.Publish(new UpdatedRepetitiveBillingEventArgs
				         {
					         UpdatedRepetitiveBilling = repetitiveBilling.Clone()
				         });

			_notificationService
				.PrintNotification("Mise à jour d'une ligne dans l'échéancier",
					string.Format("La ligne d'identifiant {0} a été mise à jour",
						repetitiveBilling.Id));
		}

		public void ShiftRepetitiveBilling(int repetitiveBillingId)
		{
			var repetitiveBilling = _repetitiveBillingRepository.Get(repetitiveBillingId);

			var previousDateTime = repetitiveBilling.ValuationDate;

			repetitiveBilling.ShiftValuationDate();

			UpdateRepetitiveBilling(repetitiveBilling);

			_notificationService
				.PrintNotification("Mise à jour de la date d'une ligne d'échéancier",
					string.Format("La date de valeur de la ligne {0} est passée du {1:dd/MM/yyyy} au {2:dd/MM/yyyy}",
						repetitiveBillingId,
						previousDateTime,
						repetitiveBilling.ValuationDate));
		}

		public List<RepetitiveBilling> GetRepetitiveBillings()
		{
			return _repetitiveBillingRepository.GetAll();
		}

		public RepetitiveBilling GetRepetitiveBilling(int repetitiveBillingId)
		{
			return _repetitiveBillingRepository.Get(repetitiveBillingId);
		}

		public void Save(XmlTextWriter writer)
		{
			_repetitiveBillingRepository.Save(writer);
		}

		public void Load(XmlDocument xmlDocument)
		{
			_repetitiveBillingRepository.Load(xmlDocument);
		}
	}
}
