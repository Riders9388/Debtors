using Debtors.Core.Models;
using MvvmCross.Commands;
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
        public override void Start()
        {
            base.Start();
            Debtors = new MvxObservableCollection<Debtor>(DatabaseService.GetDebtors());
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

        private async Task NavigateToDebtorAsync()
        {
            bool a = await NavigationService.Navigate<DebtorViewModel>();
        }

        private async Task OnItemListClick(Debtor debtor)
        {
            bool a = await NavigationService.Navigate<DebtorViewModel>();
        }

        private void OnItemLongListClick(Debtor debtor)
        {
            DatabaseService.RemoveDebtor(debtor);
            Debtors.Remove(debtor);
            RaisePropertyChanged(() => Debtors);
        }
    }
}
