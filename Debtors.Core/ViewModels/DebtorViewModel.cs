using Acr.UserDialogs;
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

        #region Overwritten
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
        #endregion

        #region Properties
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
        #endregion

        #region Commands
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
        #endregion

        #region Methods
        private void DeleteDebtor()
        {
            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to delete?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debtor == null)
                    return;

                DatabaseService.RemoveDebtor(Debtor);
                NavigationService.Close(this, true);
            };
            UserDialogs.Instance.Confirm(config);
        }

        private void SaveDebtor()
        {
            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to save?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debtor == null)
                    return;

                DatabaseService.InsertOrUpdateDebtor(Debtor);
                UserDialogs.Instance.Toast("Saved");
            };
            UserDialogs.Instance.Confirm(config);
        } 
        #endregion
    }
}
