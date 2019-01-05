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
    public class DebtViewModel : BaseViewModel<Debt, bool>
    {
        public DebtViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        #region Overwritten
        public override void Prepare(Debt parameter)
        {
            if (parameter == null)
            {
                Debt = new Debt();
                return;
            }

            Debt = parameter;
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            NavigationService.Close(this, true);
        }
        #endregion

        #region Properties
        private Debt debt;
        public Debt Debt
        {
            get { return debt; }
            set
            {
                debt = value;
                RaisePropertyChanged(() => Debt);
            }
        }
        #endregion

        #region Commands
        private IMvxCommand deleteClickCommand;
        public IMvxCommand DeleteClickCommand
        {
            get
            {
                deleteClickCommand = deleteClickCommand ?? new MvxCommand(DeleteDebt);
                return deleteClickCommand;
            }
        }

        private IMvxCommand saveClickCommand;
        public IMvxCommand SaveClickCommand
        {
            get
            {
                saveClickCommand = saveClickCommand ?? new MvxCommand(SaveDebt);
                return saveClickCommand;
            }
        }
        #endregion

        #region Methods
        private void DeleteDebt()
        {
            if (Debt == null || Debt.Id <= 0)
            {
                UserDialogs.Instance.Alert("Need save debt at first");
                return;
            }

            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to delete?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debt == null)
                    return;

                UserDialogs.Instance.Alert("Not implemented");
                return;

                DatabaseService.RemoveDebt(Debt.Id);
                NavigationService.Close(this, true);
            };
            UserDialogs.Instance.Confirm(config);
        }

        private void SaveDebt()
        {
            if (Debt.Value == null || Debt.Value <= decimal.Zero)
            {
                UserDialogs.Instance.Alert("Need set debt value at first");
                return;
            }

            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to save?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debt == null)
                    return;

                UserDialogs.Instance.Alert("Not implemented");
                return;

                DatabaseService.InsertOrUpdateDebt(Debt);

                ToastConfig toastConfig = new ToastConfig("Saved");
                UserDialogs.Instance.Toast(toastConfig);
            };
            UserDialogs.Instance.Confirm(config);
        }
        #endregion
    }
}
