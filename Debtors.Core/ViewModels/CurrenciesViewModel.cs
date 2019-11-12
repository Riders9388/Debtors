using Acr.UserDialogs;
using Debtors.Core.Extensions;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using Debtors.Core.Resources.Strings;
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
	public class CurrenciesViewModel : BaseViewModel, IProgressBar
	{
		public CurrenciesViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override async Task Initialize()
		{
			await base.Initialize();
			await LoadDataAsync();
		}
		#endregion

		#region Properties
		private bool isVisible;
		private MvxObservableCollection<Currency> currencies;

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				isVisible = value;
				RaisePropertyChanged(() => IsVisible);
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
		private IMvxAsyncCommand addClickCommand;
		private IMvxCommand<Currency> itemListClickCommand;
		private IMvxCommand<Currency> itemListLongClickCommand;

		public IMvxAsyncCommand AddClickCommand => addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToCurienciesAsync);
		public IMvxCommand<Currency> ItemListClickCommand => itemListClickCommand = itemListClickCommand ?? new MvxCommand<Currency>(OnItemListClick);
		public IMvxCommand<Currency> ItemListLongClickCommand => itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Currency>(OnItemLongListClickAsync);
		#endregion

		#region Methods
		private async Task LoadDataAsync()
		{
			IsVisible = true;
			await Task.Run(() =>
			{
				Currencies = new MvxObservableCollection<Currency>(DatabaseService.GetCurrencies());
			});
			IsVisible = false;
		}
		private async Task NavigateToCurienciesAsync()
		{
			await NavigationService.Navigate<CurrencyViewModel, Currency, bool>(null);
			await LoadDataAsync();
		}
		private void OnItemListClick(Currency currency)
		{
			if (IsVisible)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(AppStrings.editAction, async () =>
			{
				await NavigationService.Navigate<CurrencyViewModel, Currency, bool>(currency);
				await LoadDataAsync();
			});
			config.Add(AppStrings.deleteAction, () =>
			{
				if (DatabaseService.IsCurrencyInUse(currency.Id))
				{
					UserDialogs.Instance.Alert(AppStrings.currencyInUse);
					return;
				}

				UserDialogs.Instance.Confirm(AppStrings.reallyDelete, AppStrings.yes, AppStrings.no,
					async (accepted) =>
					{
						if (!accepted || currency == null)
							return;

						if (DatabaseService.RemoveCurrency(currency.Id))
							await LoadDataAsync();
						else
							UserDialogs.Instance.ToastFailure(AppStrings.error);
					});
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void OnItemLongListClickAsync(Currency debtor)
		{
			OnItemListClick(debtor);
		}
		#endregion
	}
}
