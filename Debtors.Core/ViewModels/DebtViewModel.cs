using Acr.UserDialogs;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public class DebtViewModel : BaseViewModel
    {
        public DebtViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        #region Overwritten

        #endregion

        #region Properties

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
            UserDialogs.Instance.Alert("Not implemented");
        }

        private void SaveDebt()
        {
            UserDialogs.Instance.Alert("Not implemented");
        }
        #endregion
    }
}
