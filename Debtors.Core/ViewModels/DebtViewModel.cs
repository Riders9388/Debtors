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
                UserDialogs.Instance.Alert(ResourceService.GetText("debtIsNotSave"));
                return;
            }

            UserDialogs.Instance.Confirm(ResourceService.GetText("reallyDelete"),
                ResourceService.GetText("yes"),
                ResourceService.GetText("no"),
                (accepted) =>
                {
                    if (!accepted || Debt == null)
                        return;

                    if (DatabaseService.RemoveDebt(Debt.Id))
                        NavigationService.Close(this, true);
                    else
                        UserDialogs.Instance.ToastFailure(ResourceService.GetText("error"));
                });
        }

        private void SaveDebt()
        {
            if (Debt.Value == null || Debt.Value <= decimal.Zero)
            {
                UserDialogs.Instance.Alert(ResourceService.GetText("valueIsNotSet"));
                return;
            }
            else if (string.IsNullOrWhiteSpace(Debt.Currency))
            {
                UserDialogs.Instance.Alert(ResourceService.GetText("setCurrency"));
                return;
            }

            if (DatabaseService.InsertOrUpdateDebt(Debt))
                UserDialogs.Instance.ToastSucceed(ResourceService.GetText("saved"));
            else
                UserDialogs.Instance.ToastFailure(ResourceService.GetText("error"));
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
            config.SetMessage(ResourceService.GetText("setValue"));
            config.OkText = ResourceService.GetText("ok");
            config.CancelText = ResourceService.GetText("cancel");
            UserDialogs.Instance.Prompt(config);
        }
        #endregion
    }
}
