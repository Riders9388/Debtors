using Acr.UserDialogs;
using Debtors.Core.Extensions;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.UI;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Debtors.Core.ViewModels
{
	public class DebtorsViewModel : BaseViewModel
	{
		public DebtorsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override async Task Initialize()
		{
			await base.Initialize();
			await LoadDataAsync();
		}
		#endregion

		#region Properties
		private bool isRefreshing;
		private MvxObservableCollection<Debtor> debtors;

		public bool IsRefreshing
		{
			get { return isRefreshing; }
			set
			{
				isRefreshing = value;
				RaisePropertyChanged(() => IsRefreshing);
			}
		}
		public MvxObservableCollection<Debtor> Debtors
		{
			get { return debtors; }
			set
			{
				debtors = value;
				RaisePropertyChanged(() => Debtors);
			}
		} 
		#endregion

		#region Commands
		private IMvxAsyncCommand addClickCommand;
		private IMvxAsyncCommand aboutClickCommand;
		private IMvxAsyncCommand settingsClickCommand;
		private IMvxAsyncCommand currencyClickCommand;
		private IMvxAsyncCommand pullRefreshCommand;
		private IMvxCommand<Debtor> itemListClickCommand;
		private IMvxCommand<Debtor> itemListLongClickCommand;

		public IMvxAsyncCommand AddClickCommand => addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToDebtorAsync);
		public IMvxAsyncCommand AboutClickCommand => aboutClickCommand = aboutClickCommand ?? new MvxAsyncCommand(() => NavigationService.Navigate<AboutViewModel>());
		public IMvxAsyncCommand SettingsClickCommand => settingsClickCommand = settingsClickCommand ?? new MvxAsyncCommand(() => NavigationService.Navigate<SettingsViewModel>());
		public IMvxAsyncCommand CurrencyClickCommand => currencyClickCommand = currencyClickCommand ?? new MvxAsyncCommand(() => NavigationService.Navigate<CurrenciesViewModel>());
		public IMvxAsyncCommand PullRefreshCommand => pullRefreshCommand = pullRefreshCommand ?? new MvxAsyncCommand(LoadDataAsync);
		public IMvxCommand<Debtor> ItemListClickCommand => itemListClickCommand = itemListClickCommand ?? new MvxAsyncCommand<Debtor>(OnItemListClickAsync);
		public IMvxCommand<Debtor> ItemListLongClickCommand => itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Debtor>(OnItemLongListClickAsync);
		#endregion

		#region Methods
		private async Task LoadDataAsync()
		{
			IsRefreshing = true;
			await Task.Run(() =>
			{
				Debtors = new MvxObservableCollection<Debtor>(DatabaseService.GetDebtors());
			});
			IsRefreshing = false;
		}
		private async Task NavigateToDebtorAsync()
		{
			await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(null);
			await LoadDataAsync();
		}
		private async Task OnItemListClickAsync(Debtor debtor)
		{
			if (IsRefreshing)
				return;

			await NavigationService.Navigate<DebtorDetailsViewModel, Debtor, bool>(debtor);
			await LoadDataAsync();
		}
		private void OnItemLongListClickAsync(Debtor debtor)
		{
			if (IsRefreshing)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(ResourceService.GetString("debtsAction"), async () =>
			{
				await NavigationService.Navigate<DebtsViewModel, Debtor, bool>(debtor);
				await LoadDataAsync();
			});
			config.Add(ResourceService.GetString("detailsAction"), async () =>
			{
				await NavigationService.Navigate<DebtorDetailsViewModel, Debtor, bool>(debtor);
				await LoadDataAsync();
			});
			config.Add(ResourceService.GetString("editAction"), async () =>
			{
				await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(debtor);
				await LoadDataAsync();
			});
			config.Add(ResourceService.GetString("deleteAction"), () =>
			{
				UserDialogs.Instance.Confirm(ResourceService.GetString("reallyDelete"),
					ResourceService.GetString("yes"),
					ResourceService.GetString("no"),
					async (accepted) =>
					{
						if (!accepted || debtor == null)
							return;

						if (DatabaseService.RemoveDebtor(debtor.Id))
							await LoadDataAsync();
						else
							UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
					});
			});
			config.Add(ResourceService.GetString("cancelAction"));
			UserDialogs.Instance.ActionSheet(config);
		} 
		#endregion
	}
}
