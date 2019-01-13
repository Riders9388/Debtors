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
    public class DebtorsViewModel : BaseViewModel, IProgressBar
    {
        public DebtorsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        #region Overwritten
        public override async void Start()
        {
            base.Start();
            await LoadDataAsync();
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

        private MvxObservableCollection<Debtor> debtors;
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
        public IMvxAsyncCommand AddClickCommand
        {
            get
            {
                addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToDebtorAsync);
                return addClickCommand;
            }
        }

        private IMvxAsyncCommand aboutClickCommand;
        public IMvxAsyncCommand AboutClickCommand
        {
            get
            {
                aboutClickCommand = aboutClickCommand ?? new MvxAsyncCommand(() => NavigationService.Navigate<AboutViewModel>());
                return aboutClickCommand;
            }
        }

        private IMvxAsyncCommand currencyClickCommand;
        public IMvxAsyncCommand CurrencyClickCommand
        {
            get
            {
                currencyClickCommand = currencyClickCommand ?? new MvxAsyncCommand(() => NavigationService.Navigate<CurrenciesViewModel>());
                return currencyClickCommand;
            }
        }

        private IMvxCommand<Debtor> itemListClickCommand;
        public IMvxCommand<Debtor> ItemListClickCommand
        {
            get
            {
                itemListClickCommand = itemListClickCommand ?? new MvxCommand<Debtor>(OnItemListClick);
                return itemListClickCommand;
            }
        }

        private IMvxCommand<Debtor> itemListLongClickCommand;
        public IMvxCommand<Debtor> ItemListLongClickCommand
        {
            get
            {
                itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Debtor>(OnItemLongListClickAsync);
                return itemListLongClickCommand;
            }
        }
        #endregion

        #region Methods
        private async Task LoadDataAsync()
        {
            IsVisible = true;
            await Task.Run(() =>
            {
                Debtors = new MvxObservableCollection<Debtor>(DatabaseService.GetDebtors());
            });
            IsVisible = false;
        }

        private async Task NavigateToDebtorAsync()
        {
            await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(null);
            await LoadDataAsync();
        }

        private void OnItemListClick(Debtor debtor)
        {
            if (IsVisible)
                return;

            ActionSheetConfig config = new ActionSheetConfig();
            config.Add(ResourceService.GetText("debtsAction"), async () => 
            {
                await NavigationService.Navigate<DebtsViewModel, Debtor, bool>(debtor);
                await LoadDataAsync();
            });
            config.Add(ResourceService.GetText("editAction"), async () =>
            {
                await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(debtor);
                await LoadDataAsync();
            });
            config.Add(ResourceService.GetText("deleteAction"), () =>
            {
                UserDialogs.Instance.Confirm(ResourceService.GetText("reallyDelete"),
                    ResourceService.GetText("yes"),
                    ResourceService.GetText("no"),
                    async (accepted) =>
                    {
                        if (!accepted || debtor == null)
                            return;

                        if (DatabaseService.RemoveDebtor(debtor.Id))
                            await LoadDataAsync();
                        else
                            UserDialogs.Instance.ToastFailure(ResourceService.GetText("error"));
                    });
            });
            config.Add(ResourceService.GetText("cancelAction"));
            UserDialogs.Instance.ActionSheet(config);
        }

        private void OnItemLongListClickAsync(Debtor debtor)
        {
            OnItemListClick(debtor);
        } 
        #endregion
    }
}
