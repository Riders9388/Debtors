using Acr.UserDialogs;
using Debtors.Core.Extensions;
using Debtors.Core.Models;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
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

        private IMvxCommand addDebtBackClickCommand;
        public IMvxCommand AddDebtBackClickCommand
        {
            get
            {
                addDebtBackClickCommand = addDebtBackClickCommand ?? new MvxCommand(AddDebtBack);
                return addDebtBackClickCommand;
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

                if(DatabaseService.RemoveDebt(Debt.Id))
                    NavigationService.Close(this, true);
                else
                    UserDialogs.Instance.ToastFailure();
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
            else if (string.IsNullOrWhiteSpace(Debt.Currency))
            {
                UserDialogs.Instance.Alert("Need set debt value currency at first");
                return;
            }

            ConfirmConfig config = new ConfirmConfig();
            config.Message = "Do you really want to save?";
            config.OnAction = (accepted) =>
            {
                if (!accepted || Debt == null)
                    return;

                if (DatabaseService.InsertOrUpdateDebt(Debt))
                    UserDialogs.Instance.ToastSucceed();
                else
                    UserDialogs.Instance.ToastFailure();
            };
            UserDialogs.Instance.Confirm(config);
        }

        private void AddDebtBack()
        {
            PromptConfig config = new PromptConfig();
            config.SetAction((result) =>
            {
                if (!result.Ok || Debt == null || string.IsNullOrWhiteSpace(result.Value))
                    return;

                if (Debt.ValuesBack == null)
                    Debt.ValuesBack = new List<DebtBack>();

                Debt.ValuesBack.Add(new DebtBack()
                {
                    DebtId = Debt.Id,
                    Value = Convert.ToDecimal(result.Value)
                });
                RaisePropertyChanged(() => Debt);
            });
            config.SetInputMode(InputType.Phone);
            config.SetMessage("Set return value");
            UserDialogs.Instance.Prompt(config);
        }
        #endregion
    }
}
