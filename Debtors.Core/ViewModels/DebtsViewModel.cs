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
	public class DebtsViewModel : BaseViewModel<Debtor, bool>, IProgressBar
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
		public override async Task Initialize()
		{
			await base.Initialize();
			await LoadDataAsync();
		}
		public override void ViewDestroy(bool viewFinishing = true)
		{
			base.ViewDestroy(viewFinishing);
			NavigationService.Close(this, true);
		}
		#endregion

		#region Properties
		private bool isVisible;
		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				isVisible = value;
				RaisePropertyChanged(() => IsVisible);
			}
		}

		private Debtor debtor;
		public Debtor Debtor
		{
			get { return debtor; }
			set
			{
				debtor = value;
				RaisePropertyChanged(() => Debtor);
			}
		}

		private MvxObservableCollection<Debt> debts;
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

			IsVisible = true;
			await Task.Run(() =>
			{
				Debts = new MvxObservableCollection<Debt>(DatabaseService.GetDebts(Debtor.Id));
			});
			IsVisible = false;
		}
		private async Task NavigateToDebtAsync()
		{
			await NavigationService.Navigate<DebtViewModel, Debt, bool>(new Debt() { DebtorId = Debtor.Id });
			await LoadDataAsync();
		}
		private void OnItemListClick(Debt debt)
		{
			if (IsVisible)
				return;

			ActionSheetConfig config = new ActionSheetConfig();
			config.Add(ResourceService.GetString("editAction"), async () =>
			{
				await NavigationService.Navigate<DebtViewModel, Debt, bool>(debt);
				await LoadDataAsync();
			});
			config.Add(ResourceService.GetString("deleteAction"), () =>
			{
				UserDialogs.Instance.Confirm(ResourceService.GetString("reallyDelete"),
					ResourceService.GetString("yes"),
					ResourceService.GetString("no"),
					async (accepted) =>
					{
						if (!accepted || debt == null)
							return;
							
						if (DatabaseService.RemoveDebt(debt.Id))
							await LoadDataAsync();
						else
							UserDialogs.Instance.ToastFailure(ResourceService.GetString("error"));
					});
			});
			config.Add(ResourceService.GetString("cancelAction"));
			UserDialogs.Instance.ActionSheet(config);
		}
		private void OnItemLongListClickAsync(Debt debt)
		{
			OnItemListClick(debt);
		}
		#endregion
	}
}
