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
	public class DebtsViewModel : BaseViewModel<Debtor, bool>
	{
		public DebtsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
			: base(logProvider, navigationService) { }

		#region Overwritten
		public override void Prepare(Debtor parameter)
		{
			if (parameter == null)
			{
				Debtor = new Debtor();
				return;
			}

			Debtor = parameter;
		}
		public override async void ViewAppeared()
		{
			base.ViewAppeared();
			await LoadDataAsync();
		}
		public override void ViewDestroy(bool viewFinishing = true)
		{
			base.ViewDestroy(viewFinishing);
			NavigationService.Close(this, true);
		}
		#endregion

		#region Properties
		private bool isRefreshing;
		private Debtor debtor;
		private MvxObservableCollection<Debt> debts;

		public bool IsRefreshing
		{
			get { return isRefreshing; }
			set
			{
				isRefreshing = value;
				RaisePropertyChanged(() => IsRefreshing);
			}
		}
		public Debtor Debtor
		{
			get { return debtor; }
			set
			{
				debtor = value;
				RaisePropertyChanged(() => Debtor);
			}
		}
		public MvxObservableCollection<Debt> Debts
		{
			get { return debts; }
			set
			{
				debts = value;
				RaisePropertyChanged(() => Debts);
			}
		}
		#endregion

		#region Commands
		private IMvxAsyncCommand addClickCommand;
		private IMvxCommand<Debt> itemListClickCommand;
		private IMvxCommand<Debt> itemListLongClickCommand;

		public IMvxAsyncCommand AddClickCommand => addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToDebtAsync);
		public IMvxCommand<Debt> ItemListClickCommand => itemListClickCommand = itemListClickCommand ?? new MvxCommand<Debt>(OnItemListClick);
		public IMvxCommand<Debt> ItemListLongClickCommand => itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Debt>(OnItemLongListClickAsync);
		#endregion

		#region Methods
		private async Task LoadDataAsync()
		{
			if (Debtor == null || Debtor.Id <= 0)
				return;

			IsRefreshing = true;
			await Task.Run(() =>
			{
				Debts = new MvxObservableCollection<Debt>(DatabaseService.GetDebts(Debtor.Id));
			});
			IsRefreshing = false;
		}
		private async Task NavigateToDebtAsync()
		{
			await NavigationService.Navigate<DebtViewModel, Debt, bool>(new Debt() { DebtorId = Debtor.Id });
		}
		private void OnItemListClick(Debt debt)
		{
			if (IsRefreshing)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(AppStrings.editAction, async () =>
			{
				await NavigationService.Navigate<DebtViewModel, Debt, bool>(debt);
			});
			config.Add(AppStrings.deleteAction, () =>
			{
				UserDialogs.Instance.Confirm(AppStrings.reallyDelete, AppStrings.yes, AppStrings.no,
					async (accepted) =>
					{
						if (!accepted || debt == null)
							return;
							
						if (!DatabaseService.RemoveDebt(debt.Id))
							UserDialogs.Instance.ToastFailure(AppStrings.error);

						await LoadDataAsync();
					});
			});
			config.Add(AppStrings.cancelAction);
			UserDialogs.Instance.ActionSheet(config);
		}
		private void OnItemLongListClickAsync(Debt debt)
		{
			OnItemListClick(debt);
		}
		#endregion
	}
}
