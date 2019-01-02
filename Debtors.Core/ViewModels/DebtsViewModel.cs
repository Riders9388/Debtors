using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override void Prepare(Debtor parameter)
        {
            if (parameter == null)
            {
                Debtor = new Debtor();
                return;
            }

            Debtor = parameter;
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
        #endregion

        #region Commands

        #endregion

        #region Methods

        #endregion
    }
}
