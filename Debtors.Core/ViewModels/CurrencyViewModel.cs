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
    public class CurrencyViewModel : BaseViewModel<Currency, bool>
    {
        public CurrencyViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        #region Overwritten
        public override void Prepare(Currency parameter)
        {
            if (parameter == null)
            {
                Currency = new Currency();
                return;
            }
            Currency = parameter;
        }
        #endregion

        #region Properties
        private Currency currency;
        public Currency Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                RaisePropertyChanged(() => Currency);
            }
        }
        #endregion

        #region Commands

        #endregion

        #region Methods

        #endregion
    }
}
