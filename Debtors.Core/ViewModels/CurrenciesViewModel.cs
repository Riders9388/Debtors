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
    public class CurrenciesViewModel : BaseViewModel, IProgressBar
    {
        public CurrenciesViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
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

        private MvxObservableCollection<Currency> currencies;
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
        public IMvxAsyncCommand AddClickCommand
        {
            get
            {
                addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToCurienciesAsync);
                return addClickCommand;
            }
        }

        private IMvxCommand<Currency> itemListClickCommand;
        public IMvxCommand<Currency> ItemListClickCommand
        {
            get
            {
                itemListClickCommand = itemListClickCommand ?? new MvxCommand<Currency>(OnItemListClick);
                return itemListClickCommand;
            }
        }

        private IMvxCommand<Currency> itemListLongClickCommand;
        public IMvxCommand<Currency> ItemListLongClickCommand
        {
            get
            {
                itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Currency>(OnItemLongListClickAsync);
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
                Currencies = new MvxObservableCollection<Currency>(DatabaseService.GetCurrencies());
            });
            IsVisible = false;
        }

        private async Task NavigateToCurienciesAsync()
        {
            await NavigationService.Navigate<CurrencyViewModel, Currency, bool>(null);
            await LoadDataAsync();
        }

        private void OnItemListClick(Currency debtor)
        {
            if (IsVisible)
                return;
        }

        private void OnItemLongListClickAsync(Currency debtor)
        {
            OnItemListClick(debtor);
        }
        #endregion
    }
}
