using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.ViewModels
{
    public abstract class BaseViewModel<TParameter, TResult> : BaseViewModelResult<TParameter>, IMvxViewModel<TParameter, TResult>, IMvxViewModel<TParameter>, IMvxViewModel, IMvxViewModelResult<TResult>
    {
        public BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService)
            : base(logProvider, navigationService) { }

        public abstract void Prepare(TParameter parameter);
    }
}
