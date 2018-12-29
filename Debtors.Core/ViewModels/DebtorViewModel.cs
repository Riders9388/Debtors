using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public class DebtorViewModel : BaseViewModel<Debtor, bool>
    {
        public DebtorViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public override void Start()
        {
            base.Start();
        }

        public override void Prepare(Debtor parameter)
        {
            if (parameter == null)
            {
                Debtor = new Debtor();
                return;
            }

            Debtor = parameter;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            NavigationService.Close(this, true);
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

        private IMvxCommand deleteClickCommand;
        public IMvxCommand DeleteClickCommand
        {
            get
            {
                deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteDebtor);
                return deleteClickCommand;
            }
        }

        private IMvxCommand saveClickCommand;
        public IMvxCommand SaveClickCommand
        {
            get
            {
                saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebtor);
                return saveClickCommand;
            }
        }

        private void DeleteDebtor()
        {
            if (Debtor == null)
                return;

            DatabaseService.RemoveDebtor(Debtor);
        }

        private void SaveDebtor()
        {
            if (Debtor == null)
                return;

            DatabaseService.InsertOrUpdateDebtor(Debtor);
        }
    }
}
