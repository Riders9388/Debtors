using Acr.UserDialogs;
using Debtors.Core.Extensions;
using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Debtors.Core.ViewModels
{
	public class DebtViewModel : BaseViewModel<Debt, bool>
	{
		public DebtViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override void Prepare(Debt parameter)
		{
			if (parameter == null)
			{
				Debt = new Debt();
				return;
			}

			Debt = parameter;
		}
		public override async Task Initialize()
		{
			await base.Initialize();
			Currencies = new MvxObservableCollection<Currency>(DatabaseService.GetCurrencies());
		}
		public override void ViewDestroy(bool viewFinishing = true)
		{
			base.ViewDestroy(viewFinishing);
			NavigationService.Close(this, true);
		}
		#endregion

		#region Properties
		private Debt debt;
		private MvxObservableCollection<Currency> currencies;

		public Debt Debt
		{
			get { return debt; }
			set
			{
				debt = value;
				RaisePropertyChanged(() => Debt);
			}
		}
		public MvxObservableCollection<Currency> Currencies
		{
			get { return currencies; }
			set
			{
				currencies = value;
				RaisePropertyChanged(() => Currencies);
			}
		}
		#endregion

		#region Commands
		private IMvxCommand deleteClickCommand;
		private IMvxCommand saveClickCommand;
		private IMvxCommand addDebtBackClickCommand;
		private IMvxCommand<Currency> itemSelectedCommand;

		public IMvxCommand DeleteClickCommand => deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteDebt);
		public IMvxCommand SaveClickCommand => saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebt);
		public IMvxCommand AddDebtBackClickCommand => addDebtBackClickCommand = addDebtBackClickCommand ?? new MvxCommand(AddDebtBack);
		public IMvxCommand<Currency> ItemSelectedCommand => itemSelectedCommand = itemSelectedCommand ?? new MvxCommand<Currency>(ItemSelected);
		#endregion

		#region Methods
		private void DeleteDebt()
		{
			if (Debt == null || Debt.Id <= 0)
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("debtIsNotSave"));
				return;
			}

			UserDialogs.Instance.Confirm(ResourceService.GetString("reallyDelete"),
				ResourceService.GetString("yes"),
				ResourceService.GetString("no"),
				(accepted) =>
				{
					if (!accepted || Debt == null)
						return;

					if (DatabaseService.RemoveDebt(Debt.Id))
						NavigationService.Close(this, true);
					else
						UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
				});
		}
		private void SaveDebt()
		{
			if (Debt.Value == null || Debt.Value <= decimal.Zero)
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("valueIsNotSet"));
				return;
			}
			else if (Debt.Currency == null)
			{
				UserDialogs.Instance.Alert(ResourceService.GetString("setCurrency"));
				return;
			}

			if (DatabaseService.InsertOrUpdateDebt(Debt))
				UserDialogs.Instance.ToastSucceed(ResourceService.GetString("saved"));
			else
				UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
		}
		private void AddDebtBack()
		{
			PromptConfig config = new PromptConfig();
			config.SetAction((result) =>
			{
				if (!result.Ok || Debt == null || string.IsNullOrWhiteSpace(result.Value))
					return;

				if (Debt.ValuesBack == null)
					Debt.ValuesBack = new List<DebtBack>();

				Debt.ValuesBack.Add(new DebtBack()
				{
					DebtId = Debt.Id,
					Value = Convert.ToDecimal(result.Value)
				});
				RaisePropertyChanged(() => Debt);
			});
			config.SetInputMode(InputType.Phone);
			config.SetMessage(ResourceService.GetString("setValue"));
			config.OkText = ResourceService.GetString("ok");
			config.CancelText = ResourceService.GetString("cancel");
			UserDialogs.Instance.Prompt(config);
		}
		private void ItemSelected(Currency currency)
		{
			if (debt.ValuesBack.IsNullOrEmpty())
				return;

			UserDialogs.Instance.Alert("Cannot change currency. There are return values");
			Debt.Currency = Currencies.FirstOrDefault(x => x.Id == Debt.CurrencyId);
		}
		#endregion
	}
}
