using Acr.UserDialogs;
using Debtors.Core.Extensions;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.ViewModels
{
	public class CurrencyViewModel : BaseViewModel<Currency, bool>
	{
		public CurrencyViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override void Prepare(Currency parameter)
		{
			if (parameter == null)
			{
				Currency = new Currency();
				return;
			}

			Currency = parameter;
		}
		public override void ViewDestroy(bool viewFinishing = true)
		{
			base.ViewDestroy(viewFinishing);
			NavigationService.Close(this, true);
		}
		#endregion

		#region Properties
		private Currency currency;
		public Currency Currency
		{
			get { return currency; }
			set
			{
				currency = value;
				RaisePropertyChanged(() => Currency);
			}
		}
		#endregion

		#region Commands
		private IMvxCommand deleteClickCommand;
		private IMvxCommand saveClickCommand;

		public IMvxCommand DeleteClickCommand => deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteCurrency);
		public IMvxCommand SaveClickCommand => saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebtor);
		#endregion

		#region Methods
		private void DeleteCurrency()
		{
			if (Currency == null || Currency.Id <= 0)
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("debtorIsNotSave"));
				return;
			}

			if (DatabaseService.IsCurrencyInUse(Currency.Id))
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("currencyInUse"));
				return;
			}

			UserDialogs.Instance.Confirm(ResourceService.GetString("reallyDelete"),
				ResourceService.GetString("yes"),
				ResourceService.GetString("no"),
				(accepted) =>
				{
					if (!accepted || Currency == null)
						return;

					if (DatabaseService.RemoveCurrency(Currency.Id))
						NavigationService.Close(this, true);
					else
						UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
				});
		}
		private void SaveDebtor()
		{
			if (string.IsNullOrWhiteSpace(Currency.Symbol))
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("setSymbol"));
				return;
			}

			if (DatabaseService.InsertOrUpdateCurrency(Currency))
				UserDialogs.Instance.ToastSucceed(ResourceService.GetString("saved"));
			else
				UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
		}
		#endregion
	}
}
