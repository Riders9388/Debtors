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
    public class DebtorsViewModel : BaseViewModel
    {
        public DebtorsViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public override void Start()
        {
            base.Start();
            LoadData();
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

        private IMvxAsyncCommand<Debtor> itemListClickCommand;
        public IMvxAsyncCommand<Debtor> ItemListClickCommand
        {
            get
            {
                itemListClickCommand = itemListClickCommand ?? new MvxAsyncCommand<Debtor>(OnItemListClick);
                return itemListClickCommand;
            }
        }

        private IMvxCommand<Debtor> itemListLongClickCommand;
        public IMvxCommand<Debtor> ItemListLongClickCommand
        {
            get
            {
                itemListLongClickCommand = itemListLongClickCommand ?? new MvxCommand<Debtor>(OnItemLongListClick);
                return itemListLongClickCommand;
            }
        }

        private void LoadData()
        {
            Debtors = new MvxObservableCollection<Debtor>(DatabaseService.GetDebtors());
        }

        private async Task NavigateToDebtorAsync()
        {
            await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(null);
            LoadData();
        }

        private async Task OnItemListClick(Debtor debtor)
        {
            await NavigationService.Navigate<DebtorViewModel, Debtor, bool>(debtor);
            LoadData();
        }

        private void OnItemLongListClick(Debtor debtor)
        {
            DatabaseService.RemoveDebtor(debtor);
            LoadData();
        }
    }
}
