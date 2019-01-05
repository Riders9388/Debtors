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
        public override void Start()
        {
            base.Start();
        }

        public override async void Prepare(Debtor parameter)
        {
            if (parameter == null)
            {
                Debtor = new Debtor();
                return;
            }

            Debtor = parameter;
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
        public IMvxAsyncCommand AddClickCommand
        {
            get
            {
                addClickCommand = addClickCommand ?? new MvxAsyncCommand(NavigateToDebtAsync);
                return addClickCommand;
            }
        }
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
            await NavigationService.Navigate<DebtViewModel, Debt, bool>(null);
            await LoadDataAsync();
        }
        #endregion
    }
}
