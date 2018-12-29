using Debtors.Core.Models;
using MvvmCross.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public class DebtorViewModel : BaseViewModel
    {
        public override void Start()
        {
            base.Start();
            Debtor = new Debtor();
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
