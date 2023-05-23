using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
	[Export(typeof(IBillingManager))]
	public class BillingManager : IBillingManager
	{
		[Import] private IBillingRepository _billingRepository;
		[Import] private IEventAggregator _eventAggregator;
		[Import] private INotificationService _notificationService;

		public bool IsDurty
		{
			get { return _billingRepository.IsDurty; }
		}

		public void FindBilling(DateTime date, double amount, out List<Billing> perfectMatchings, out List<Billing> incompleteMatchings)
		{
			perfectMatchings = _billingRepository.FindAll(date, amount);
			incompleteMatchings = _billingRepository.FindAllWithExcludedDate(date, amount);
		}

		public void AddBilling(Billing billing)
		{
			_billingRepository.Add(billing);

			_eventAggregator
				.GetEvent<AddedBillingEvent>()
				.Publish(new AddedBillingEventArgs
				         {
					         AddedBilling = billing.Clone()
				         });

			_notificationService
				.PrintNotification("Ajout d'une ligne de facture",
					string.Format("La ligne d'identifiant {0} a été ajoutée",
						billing.Id));
		}

		public void RemoveBilling(int billingId)
		{
			_billingRepository.Remove(billingId);

			_eventAggregator
				.GetEvent<DeletedBillingEvent>()
				.Publish(new DeletedBillingEventArgs
				         {
							 BillingId = billingId
				         });

			_notificationService
				.PrintNotification("Suppression d'une ligne de facture",
					string.Format("La ligne d'identifiant {0} a été supprimée",
						billingId));
		}

		public void UpdateBilling(Billing billing)
		{
			_billingRepository.Update(billing);

			_eventAggregator
				.GetEvent<UpdatedBillingEvent>()
				.Publish(new UpdatedBillingEventArgs
				         {
							 UpdatedBilling = billing.Clone()
				         });

			_notificationService
				.PrintNotification("Mise à jour d'une ligne de facture",
					string.Format("La ligne d'identifiant {0} a été mise à jour",
						billing.Id));
		}

		public void UpdateChecked(int billingId, bool isChecked)
		{
			_billingRepository.UpdateChecked(billingId, isChecked);

			_eventAggregator
				.GetEvent<UpdateCheckedBillingEvent>()
				.Publish(new UpdateCheckedBillingEventArgs
				         {
					         BillingId = billingId,
					         Checked = isChecked,
				         });
		}

		public void UpdateDelayed(int billingId, bool isDelayed)
		{
			_billingRepository.UpdateDelayed(billingId, isDelayed);

			_eventAggregator
				.GetEvent<UpdateDelayedBillingEvent>()
				.Publish(new UpdateDelayedBillingEventArgs
				{
					BillingId = billingId,
					Delayed = isDelayed,
				});
		}

		public void UpdateArchived(int billingId, bool isArchived)
		{
			_billingRepository.UpdateArchived(billingId, isArchived);
		}

		public int MergeBillings(List<BillingForMerging> billings)
		{
			var title =
				string.Format("Fusion de {0} éléments en date du {1:dd/MM/yyyy} à {2:HH}h{3:mm}",
					billings.Count,
					DateTime.Now,
					DateTime.Now,
					DateTime.Now);

			var amount = billings.Sum(n => n.Amount*(n.Positive ? 1 : -1));

			var newBilling = new Billing
			                 {
				                 TransactionDate = billings.Max(n => n.TransactionDate),
				                 ValuationDate = billings.Max(n => n.ValuationDate),
				                 Title = title,
								 Amount = amount,
								 Positive = amount >= 0,
				                 Checked = billings.Any(n => n.Checked),
				                 Delayed = billings.Any(n => n.Delayed),
				                 Comment = title + "\r\n" + string.Join("\r\n", billings.Select(n =>
					                 string.Format("{0:dd/MM/yyyy} : {1} ({2})",
						                 n.ValuationDate,
						                 n.Title,
						                 n.Amount*(n.Positive ? 1 : -1)))),
			                 };

			billings.ForEach(n =>
			                 {
								 _billingRepository.Remove(n.BillingId);

								 _eventAggregator
									 .GetEvent<DeletedBillingEvent>()
									 .Publish(new DeletedBillingEventArgs
									 {
										 BillingId = n.BillingId
									 });
			                 });

			AddBilling(newBilling);

			return newBilling.Id;
		}

		public List<Billing> GetBillings()
		{
			return _billingRepository.GetAll();
		}

		public Billing GetBilling(int billingId)
		{
			return _billingRepository.Get(billingId);
		}

		public void Save(XmlTextWriter writer)
		{
			_billingRepository.Save(writer);
		}

		public void Load(XmlDocument xmlDocument)
		{
			_billingRepository.Load(xmlDocument);
		}
	}
}
